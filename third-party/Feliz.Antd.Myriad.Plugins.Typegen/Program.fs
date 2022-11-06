namespace Myriad.Plugins

open System
open System.IO
open FSharp.Compiler
open Myriad.Core
open Myriad.Core.Ast
open System
open FSharp.Compiler.Syntax
open FSharp.Compiler.Xml
open Myriad.Core
open Myriad.Core.Ast
open FSharp.Compiler.Text.Range
open FSharp.Compiler.SyntaxTrivia

[<MyriadGenerator("Feliz.Antd.typegen")>]
type Example() =
    interface IMyriadGenerator with
        member _.ValidInputExtensions = seq {".fs"}
        member _.Generate(context: GeneratorContext) =

            let letPattern = SynPat.CreateNamed (Ident.Create "fortyTwo")

            let let42 =
                SynModuleDecl.CreateLet
                    [SynBinding.Let(pattern = letPattern, expr = SynExpr.CreateConst(SynConst.Int32 42))]

            let componentInfo = SynComponentInfo.Create [ Ident.Create "example1" ]
            let nestedModule = SynModuleDecl.CreateNestedModule(componentInfo, [ let42 ])

            let namespaceOrModule =
                SynModuleOrNamespace.CreateNamespace(Ident.CreateLong "hello", decls = [ nestedModule ])

            Output.Ast [namespaceOrModule]