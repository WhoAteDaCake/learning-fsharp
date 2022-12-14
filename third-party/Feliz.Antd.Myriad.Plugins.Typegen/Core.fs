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
    | SynExpr.LetOrUse (isRecursive, isUse, bindings, body, range1, synExprLetOrUseTrivia) ->
        SynExpr.LetOrUse(
            isRecursive,
            isUse,
            List.map (replaceInteropInSynBinding newCall) bindings,
            replaceInteropInExpr newCall body,
            range1,
            synExprLetOrUseTrivia
        )
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

and replaceInteropInSynBinding
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

let getComponentModuleName cmp =
    match getAttribute<Generator.LibraryRootAttribute> cmp with
    | Some (attr) ->
        match attr.ArgExpr with
        | SynExpr.Paren(expr = SynExpr.Const(constant = SynConst.String (text = text))) -> text
        | _ -> failwith "Couldn't extract root"
    | _ -> failwith "Couldn't extract root"



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
    SynModuleDecl.Let(false, [ binding ], range0)

let isInteropType (ls: Ident list) =
    match List.map (fun (i: Ident) -> i.idText) ls with
    | "Interop" :: [ "inlined" ] -> true
    | _ -> false

let rec replaceInteropTypeInSynType newType =
    function
    | SynType.App (typeName, rangeOption, typeArgs, commaRanges, greaterRange, isPostfix, range) ->
        let newTypeArgs =
            List.map (replaceInteropTypeInSynType newType) typeArgs

        SynType.App(typeName, rangeOption, newTypeArgs, commaRanges, greaterRange, isPostfix, range)
    | SynType.LongIdent (LongIdentWithDots (idents, dotRanges)) ->
        if isInteropType idents then
            SynType.LongIdent(LongIdentWithDots.CreateString newType)
        else
            SynType.LongIdent(LongIdentWithDots(idents, dotRanges))
    | item -> item

let rec replaceInteropTypeInExpr newType =
    function
    | SynExpr.TypeApp (synExpr, lessRange, typeArgs, commaRanges, greaterRange, typeArgsRange, range) ->
        let newArgs =
            typeArgs
            |> List.map (replaceInteropTypeInSynType newType)

        SynExpr.TypeApp(synExpr, lessRange, newArgs, commaRanges, greaterRange, typeArgsRange, range)
    | SynExpr.App (exprAtomicFlag, isInfix, funcExpr, argExpr, range) ->
        SynExpr.App(exprAtomicFlag, isInfix, replaceInteropTypeInExpr newType funcExpr, argExpr, range)
    | item -> item


let rec replaceInteropTypeInSynPat newType =
    function
    | SynPat.Typed (pat, targetType, range) -> SynPat.Typed(pat, replaceInteropTypeInSynType newType targetType, range)
    | SynPat.Paren (synPat, range1) -> SynPat.Paren(replaceInteropTypeInSynPat newType synPat, range1)
    | SynPat.LongIdent (longIdentWithDots,
                        propertyKeywordOption,
                        identOption,
                        synValTyparDeclsOption,
                        argPats,
                        synAccessOption,
                        range) ->
        let newArgsPats =
            match argPats with
            | SynArgPats.Pats pats -> SynArgPats.Pats(List.map (replaceInteropTypeInSynPat newType) pats)
            | item -> item

        SynPat.LongIdent(
            longIdentWithDots,
            propertyKeywordOption,
            identOption,
            synValTyparDeclsOption,
            newArgsPats,
            synAccessOption,
            range
        )
    | item -> item

let replaceInteropTypeInBinding
    newType
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
        replaceInteropTypeInSynPat newType headPat,
        synBindingReturnInfoOption,
        replaceInteropTypeInExpr newType synExpr,
        range,
        debugPointAtBinding,
        synBindingTrivia
    ))

let replaceInteropTypeInMember newType =
    function
    | SynMemberDefn.Member (memberDefn, range) ->
        SynMemberDefn.Member(replaceInteropTypeInBinding newType memberDefn, range)
    | item -> item
