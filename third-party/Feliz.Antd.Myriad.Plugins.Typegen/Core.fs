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

let replaceInteropInSynBinding newCall
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

let removeAttribute<'a> (SynTypeDefn (synComponentInfo, _typeDefRepr, _memberDefs, _implicitCtor, _range, _trivia)) =
    let newAttrs (attrs: SynAttributes) =
        attrs
        |> List.map (fun n -> n.changeAttributes (List.filter (typeNameMatches typeof<'a> >> (not))))

    (SynTypeDefn(synComponentInfo.changeAttributes newAttrs, _typeDefRepr, _memberDefs, _implicitCtor, _range, _trivia))

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
            (removeAttribute<Generator.ExtendsMethodsAttribute> cmp)
            exts


// let inline mkButtonAttr (key: string) (value: obj) : IButtonProperty = unbox (key, value)
let createAttr (returnTypeName: string) (fnName: string) =
    let returnType =
        SynType.CreateLongIdent(returnTypeName)

    let returnInfo =
        SynBindingReturnInfo.Create(returnType)

    let keyType =
        SynExpr.CreateIdentString("key")

    let valueType =
        SynExpr.CreateIdentString("value")

    let expr =
        SynExpr.Typed(
            SynExpr.CreateApp(
                SynExpr.CreateIdentString("unbox"),
                SynExpr.CreateParen(SynExpr.CreateTuple([ keyType; valueType ]))
            ),
            returnType,
            range0
        )

    let valData =
        let keyInfo =
            SynArgInfo.CreateIdString "key"

        let valueInfo =
            SynArgInfo.CreateIdString "value"

        let valInfo =
            SynValInfo.SynValInfo([ [ keyInfo; valueInfo ] ], SynArgInfo.Empty)

        SynValData.SynValData(None, valInfo, None)

    let pattern =
        let makeArgType (argName: string, argType: string) =
            SynPat.CreateParen(
                SynPat.CreateTyped(
                    SynPat.CreateNamed(Ident(argName, range0)),
                    SynType.CreateLongIdent argType
                )
            )
        SynPat.CreateLongIdent(
            id = LongIdentWithDots.Create([fnName]),
            args = ([("key", "string"); ("value", "obj")] |> List.map makeArgType)
        )

    SynModuleDecl.Let(
        false,
        [ SynBinding.Let(isInline = true, returnInfo = returnInfo, expr = expr, valData = valData, pattern=pattern) ],
        range0
    )
