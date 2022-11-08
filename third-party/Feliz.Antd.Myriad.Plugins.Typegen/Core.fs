module Feliz.Antd.Myriad.Plugins.Typegen.Core

open System
open System.IO
open FSharp.Compiler
open Fable.AST.Fable
open Myriad.Core
open System
open FSharp.Compiler.Syntax
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

    SynTypeDefn(synComponentInfo, synTypeDefnRepr, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia)


let isInteropCall =
    function
    | (LongIdentWithDots (Syntax.Ident ("Interop", _) :: [ "attr" ], _)) -> true
    | _ -> false


let rec replaceInteropInExpr =
    function
    | SynExpr.App (exprAtomicFlag, isInfix, funcExpr, argExpr, range) ->
        SynExpr.App(exprAtomicFlag, isInfix, replaceInteropInExpr funcExpr, argExpr, range)
    | SynExpr.LongIdent (isOptional, longDotId, altNameRefCall, range) ->
        if isInteropCall longDotId then
            SynExpr.LongIdent(isOptional, longDotId, altNameRefCall, range)
        else
            SynExpr.LongIdent(isOptional, longDotId, altNameRefCall, range)
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
        replaceInteropInExpr synExpr,
        range,
        debugPointAtBinding,
        synBindingTrivia
    ))

let replaceInteropInMember =
    function
    | SynMemberDefn.Member (dnf, range) -> SynMemberDefn.Member(replaceInteropInSynBinding dnf, range)
    | item -> item
