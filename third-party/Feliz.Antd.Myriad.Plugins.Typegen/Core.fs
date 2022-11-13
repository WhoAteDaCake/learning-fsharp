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
open Feliz.Antd.Myriad.Plugins.Typegen.Extensions
open Feliz.Antd.Myriad.Plugins.Typegen.Standard


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


let getStaticMembers
    (SynTypeDefn (synComponentInfo, synTypeDefnRepr, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia))
    =
    match synTypeDefnRepr with
    | SynTypeDefnRepr.ObjectModel (kind, members, range) -> members
    | _ -> []


let modifyStaticMembers
    fn
    (SynTypeDefn (synComponentInfo, synTypeDefnRepr, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia))
    =
    let newTypeDefn =
        match synTypeDefnRepr with
        | SynTypeDefnRepr.ObjectModel (kind, members, range) -> SynTypeDefnRepr.ObjectModel(kind, fn members, range)
        | _ -> synTypeDefnRepr

    SynTypeDefn(synComponentInfo, newTypeDefn, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia)

let modifyStaticMembers2<'a>
    (fn: SynMemberDefns -> 'a option * SynMemberDefns)
    (SynTypeDefn (synComponentInfo, synTypeDefnRepr, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia))
    =
    let ret, newTypeDefn =
        match synTypeDefnRepr with
        | SynTypeDefnRepr.ObjectModel (kind, members, range) ->
            let ret, members = fn members
            ret, SynTypeDefnRepr.ObjectModel(kind, members, range)
        | _ -> None, synTypeDefnRepr

    ret, SynTypeDefn(synComponentInfo, newTypeDefn, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia)

let longIndentWithDotsToLs =
    function
    | (LongIdentWithDots (ls, _)) -> ls |> List.map (fun x -> x.idText)

let isInteropCall ld =
    match longIndentWithDotsToLs ld with
    | "Interop" :: [ "attr" ] -> true
    | _ -> false

let rec replaceInteropInExpr newCall =
    function
    | SynExpr.App (exprAtomicFlag, isInfix, funcExpr, argExpr, range) ->
        SynExpr.App(exprAtomicFlag, isInfix, replaceInteropInExpr newCall funcExpr, argExpr, range)
    | SynExpr.LongIdent (isOptional, longDotId, altNameRefCall, range) ->
        let longDotId =
            if isInteropCall longDotId then
                LongIdentWithDots.Create([ "Interop"; newCall ])
            else
                longDotId

        SynExpr.LongIdent(isOptional, longDotId, altNameRefCall, range)
    | item -> item

let replaceInteropInSynBinding
    newCall
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
        replaceInteropInExpr newCall synExpr,
        Range.Zero,
        debugPointAtBinding,
        synBindingTrivia
    ))

let replaceInteropInMember newCall =
    function
    | SynMemberDefn.Member (dnf, range) -> SynMemberDefn.Member(replaceInteropInSynBinding newCall dnf, Range.Zero)
    | item -> item

let extractArgNames =
    function
    | SynExpr.TypeApp (synExpr, lessRange, typeArgs, commaRanges, greaterRange, typeArgsRange, range) ->
        typeArgs
        |> List.collect (fun x ->
            match x with
            | SynType.LongIdent (longDotId) -> longIndentWithDotsToLs longDotId
            | _ -> [])
    | _ -> []

let rec getExtensions (attr: SynAttribute) =
    match attr.ArgExpr with
    | SynExpr.Paren (synExpr, _, _, _) ->
        match synExpr with
        | SynExpr.Tuple (_, exprs, _, _) -> exprs |> List.map (extractArgNames >> List.head)
        | _ -> []
    | _ -> []

let extendComponent (lookup: Map<string, SynTypeDefn>) cmp =
    match getAttribute<Generator.ExtendsMethodsAttribute> cmp with
    | None -> cmp
    | Some (attr) ->
        let exts = getExtensions attr

        List.fold
            (fun cmp ext ->
                match lookup.TryFind(ext) with
                | Some (tp) ->
                    let members = getStaticMembers tp
                    modifyStaticMembers (List.append members) cmp
                | None -> cmp)
            (cmp.removeAttribute<Generator.ExtendsMethodsAttribute> ())
            exts

let findAndRemove (name: string) (ls: SynMemberDefns) =
    let findCreate (m: SynMemberDefn) =
        match m.Ident() with
        | Some ([ foundName ]) -> foundName = name
        | _ -> false

    let found, others =
        List.splitWhen findCreate ls

    found, others

let bindingToModuleLet binding =
    SynModuleDecl.Let(
        false,
        [
             binding
        ],
        range0
    )
