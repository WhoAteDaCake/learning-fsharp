module Feliz.Antd.Myriad.Plugins.Typegen.Extensions

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text
open FSharp.Compiler.Xml


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

type SynAttributeList with
    member this.changeAttributes fn =
        { this with Attributes = fn this.Attributes }

type SynTypeDefn with
    static member CreateOfName(name) = SynTypeDefn.Create(SynComponentInfo())

    static member Create(typeInfo, typeRepr, ?members, ?implicitConstructor, ?trivia) =
        let trivia =
            defaultArg trivia SynTypeDefnTrivia.Zero

        SynTypeDefn(typeInfo, typeRepr, defaultArg members [], implicitConstructor, Range.Zero, trivia)
