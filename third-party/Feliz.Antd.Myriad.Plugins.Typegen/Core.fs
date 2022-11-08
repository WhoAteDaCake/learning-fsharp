module Feliz.Antd.Myriad.Plugins.Typegen.Core

open System
open System.IO
open FSharp.Compiler
open Fable.AST.Fable
open Myriad.Core
open System
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open Myriad.Core
open Myriad.Core.Ast
open FSharp.Compiler.Text.Range
open FSharp.Compiler.SyntaxTrivia
open Myriad.Plugins


let typeName (SynTypeDefn (typeInfo, _, _, _, _, _)) =
    match typeInfo with
    | SynComponentInfo (_, _, _, longId, _, _, _, _) -> longId[0].idText

let sortTypes acc (_, types) =
    types
    |> List.fold
        (fun (included, methods, components) typeDef ->
            if hasAttribute<Generator.ComponentAttribute> (typeDef) then
                (included, methods, typeDef :: components)
            elif hasAttribute<Generator.MethodsAttribute> (typeDef) then
                (included, typeDef :: methods, components)
            elif hasAttribute<Generator.IncludedAttribute> (typeDef) then
                (typeDef :: included, methods, components)
            else
                (included, methods, components))
        acc


let modifyStaticMembers
    fn
    (SynTypeDefn (synComponentInfo, synTypeDefnRepr, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia))
    =
    let newTypeDefn =
        match synTypeDefnRepr with
        | SynTypeDefnRepr.ObjectModel (kind, members, range) -> SynTypeDefnRepr.ObjectModel(kind, fn members, range)
        | _ -> synTypeDefnRepr

    SynTypeDefn(synComponentInfo, newTypeDefn, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia)

// let isInteropMethod )

let isInteropCall =
    function
    | (LongIdentWithDots (moduleName :: [methodName], _)) ->
        moduleName.idText = "Interop" && methodName.idText = "attr"
    | _ -> false


let rec replaceInteropInExpr =
    function
    | SynExpr.App (exprAtomicFlag, isInfix, funcExpr, argExpr, range) ->
        let newFuncExpr = replaceInteropInExpr funcExpr
        SynExpr.App(exprAtomicFlag, isInfix, newFuncExpr, argExpr, range)
    | SynExpr.LongIdent (isOptional, longDotId, altNameRefCall, range) ->
        let output =
            if isInteropCall longDotId then
                let newAttr = LongIdentWithDots.Create(["CustomInterop"; "test"])
                SynExpr.LongIdent(isOptional, newAttr, altNameRefCall, range)
            else
                SynExpr.LongIdent(isOptional, longDotId, altNameRefCall, range)
        output
    | item -> item

let replaceInteropInSynBinding
    (SynBinding (synAccessOption,
                 synBindingKind,
                 isInline,
                 isMutable,
                 synAttributeLists,
                 preXmlDoc,
                 synValData,
                 headPat,
                 synBindingReturnInfoOption,
                 synExpr,
                 range,
                 debugPointAtBinding,
                 synBindingTrivia))
    =
    let newExpr = replaceInteropInExpr synExpr
    (SynBinding(
        synAccessOption,
        synBindingKind,
        isInline,
        isMutable,
        synAttributeLists,
        preXmlDoc,
        synValData,
        headPat,
        synBindingReturnInfoOption,
        newExpr,
        Range.Zero,
        debugPointAtBinding,
        synBindingTrivia
    ))

let replaceInteropInMember =
    function
    | SynMemberDefn.Member (dnf, range) ->
        let newMember = replaceInteropInSynBinding dnf
        SynMemberDefn.Member(newMember, Range.Zero)
    | item -> item
