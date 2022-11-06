﻿namespace Myriad.Plugins

open System
open System.IO
open FSharp.Compiler
open Myriad.Core
open System
open FSharp.Compiler.Syntax
open FSharp.Compiler.Xml
open Myriad.Core
open Myriad.Core.Ast
open FSharp.Compiler.Text.Range
open FSharp.Compiler.SyntaxTrivia

[<RequireQualifiedAccess>]
module Generator =
    type Component() =
        inherit Attribute()

[<MyriadGenerator("Feliz.Antd.typegen")>]
type Example() =
    interface IMyriadGenerator with
        member _.ValidInputExtensions = seq {".fs"}
        member _.Generate(context: GeneratorContext) =
            let ast, _ =
                fromFilename context.InputFilename
                |> Async.RunSynchronously
                |> Array.head

            let namespaceAndrecords =
                extractRecords ast
                |> List.choose (fun (ns, types) ->
                    match types |> List.filter hasAttribute<Generator.Component> with
                    | [] -> None
                    | types -> Some (ns, types))
            let letPattern = SynPat.CreateNamed (Ident.Create "fortyTwo")

            let let42 =
                SynModuleDecl.CreateLet
                    [SynBinding.Let(pattern = letPattern, expr = SynExpr.CreateConst(SynConst.Int32 42))]

            let componentInfo = SynComponentInfo.Create [ Ident.Create "example1" ]
            let nestedModule = SynModuleDecl.CreateNestedModule(componentInfo, [ let42 ])

            let namespaceOrModule =
                SynModuleOrNamespace.CreateNamespace(Ident.CreateLong "hello", decls = [ nestedModule ])

            Output.Ast [namespaceOrModule]