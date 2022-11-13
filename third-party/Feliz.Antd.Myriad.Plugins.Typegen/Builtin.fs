module Feliz.Antd.Myriad.Plugins.Typegen.Builtin

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