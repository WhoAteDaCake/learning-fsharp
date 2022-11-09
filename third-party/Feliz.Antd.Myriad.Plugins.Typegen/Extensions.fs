module Feliz.Antd.Myriad.Plugins.Typegen.Extensions

open FSharp.Compiler.Syntax

type SynComponentInfo with
    member this.changeAttributes (fn: SynAttributes -> SynAttributes) =
        let (SynComponentInfo(attributes, _typeParams, _constraints, _recordIdent, _doc, _preferPostfix, _access, _ciRange)) = this
        SynComponentInfo(fn attributes, _typeParams, _constraints, _recordIdent, _doc, _preferPostfix, _access, _ciRange)

type SynAttributeList with
    member this.changeAttributes fn =
        { this with Attributes = fn this.Attributes }