module Feliz.Antd.Myriad.Plugins.Typegen.Extensions

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open Myriad.Core.Ast
open Myriad.Core

let removeAttribute<'a> (attrs: SynAttributes): SynAttributes =
    attrs
    |> List.map (fun n ->
        let newAttrs =
            n.Attributes
            |> List.filter (typeNameMatches typeof<'a> >> (not))

        { n with Attributes = newAttrs })

let appendAttribute (newAttr: string) (attrs: SynAttributes): SynAttributes =
    let newAttr = SynAttributeList.Create(SynAttribute.Create(newAttr))
    newAttr :: attrs

let hasAttributeIn<'a> (attrs: SynAttributes)  =
    attrs
    |> List.collect (fun n -> n.Attributes)
    |> List.exists (typeNameMatches typeof<'a>)

type Ident with
    static member Create(name) = Ident(name, Range.Zero)

type SynTypeDefnRepr with
    static member Create(name) = Ident(name, Range.Zero)

type SynArgPats with
    static member Empty = SynArgPats.Pats []

type SynComponentInfo with
    static member Create(longId, ?typeParams, ?preferPostFix, ?accessibility, ?attributes, ?constrains) =
        SynComponentInfo(
            defaultArg attributes [],
            typeParams,
            defaultArg constrains [],
            longId,
            PreXmlDoc.Empty,
            defaultArg preferPostFix false,
            accessibility,
            Range.Zero
        )

    member this.hasAttribute<'a>() =
        let (SynComponentInfo (attributes,
                               _typeParams,
                               _constraints,
                               _recordIdent,
                               _doc,
                               _preferPostfix,
                               _access,
                               _ciRange)) =
            this

        hasAttributeIn<'a> attributes

    member this.changeAttributes(fn: SynAttributes -> SynAttributes) =
        let (SynComponentInfo (attributes,
                               _typeParams,
                               _constraints,
                               _recordIdent,
                               _doc,
                               _preferPostfix,
                               _access,
                               _ciRange)) =
            this

        SynComponentInfo(
            fn attributes,
            _typeParams,
            _constraints,
            _recordIdent,
            _doc,
            _preferPostfix,
            _access,
            _ciRange
        )

    member this.appendAttribute(name: string) =
        this.changeAttributes (appendAttribute name)

    member this.removeAttribute<'a>() =
        this.changeAttributes removeAttribute<'a>

type SynTypeDefn with
    static member Simple(name: string) =
        SynTypeDefn.Create(
            SynComponentInfo.Create(Ident.Create(name) :: []),
            SynTypeDefnRepr.ObjectModel(SynTypeDefnKind.Interface, [], Range.Zero)
        )

    static member Create(typeInfo, typeRepr, ?members, ?implicitConstructor, ?trivia) =
        SynTypeDefn(
            typeInfo,
            typeRepr,
            defaultArg members [],
            implicitConstructor,
            Range.Zero,
            defaultArg trivia SynTypeDefnTrivia.Zero
        )

    member this.removeAttribute<'a>() =
        let (SynTypeDefn (synComponentInfo, _typeDefRepr, _memberDefs, _implicitCtor, _range, _trivia)) =
            this

        (SynTypeDefn(
            synComponentInfo.changeAttributes removeAttribute<'a>,
            _typeDefRepr,
            _memberDefs,
            _implicitCtor,
            _range,
            _trivia
        ))

type SynModuleOrNamespace with
    member this.decls() =
        let (SynModuleOrNamespace (_, _, _, decls, _, _, _, _)) =
            this
        decls

    member this.removeAttribute<'a>() =
        let (SynModuleOrNamespace (longId, isRecursive, kind, decls, xmlDoc, attributes, accessibility, range)) =
            this

        SynModuleOrNamespace(
            longId,
            isRecursive,
            kind,
            decls,
            xmlDoc,
            removeAttribute<'a> attributes,
            accessibility,
            range
        )

    member this.appendAttribute (name: string) =
        let (SynModuleOrNamespace (longId, isRecursive, kind, decls, xmlDoc, attributes, accessibility, range)) =
            this

        SynModuleOrNamespace(
            longId,
            isRecursive,
            kind,
            decls,
            xmlDoc,
            appendAttribute name attributes,
            accessibility,
            range
        )

    member this.toNested () =
        let (SynModuleOrNamespace (longId, _, _, decls, _, _, _, _)) =
            this
        SynModuleDecl.CreateNestedModule(
            ci=SynComponentInfo.Create(longId),
            decls=decls,
            isRec=false,
            isCont=false
        )

type SynModuleDecl with

    member this.appendAttribute (name: string) =
        match this with
        | SynModuleDecl.NestedModule(moduleInfo, isRecursive, delcs, isContinuing, range, trivia) ->
            SynModuleDecl.NestedModule(moduleInfo.appendAttribute(name), isRecursive, delcs, isContinuing, range, trivia)
        | item -> item

    member this.removeAttribute<'a> () =
        match this with
        | SynModuleDecl.NestedModule(moduleInfo, isRecursive, delcs, isContinuing, range, trivia) ->
            SynModuleDecl.NestedModule(moduleInfo.removeAttribute<'a>(), isRecursive, delcs, isContinuing, range, trivia)
        | item -> item