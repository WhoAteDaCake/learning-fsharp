namespace Myriad.Plugins

open System
open System.IO
open FSharp.Compiler
open Myriad.Core
open System
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open Myriad.Core
open Myriad.Core.Ast
open FSharp.Compiler.Text.Range
open FSharp.Compiler.SyntaxTrivia
open Feliz.Antd.Myriad.Plugins.Typegen
open Feliz.Antd.Myriad.Plugins.Typegen.Extensions


[<MyriadGenerator("Feliz.Antd.typegen")>]
type Example() =
    interface IMyriadGenerator with
        member _.ValidInputExtensions = seq { ".fs" }

        member _.Generate(context: GeneratorContext) =
            let ast, _ =
                fromFilename context.InputFilename
                |> Async.RunSynchronously
                |> Array.head

            let included, methods, components =
                (extractTypeDefn ast)
                |> List.fold Core.sortTypes ([], [], [])

            let methodMap =
                Map(
                    methods
                    |> List.map (fun m -> (Core.typeName m, m))
                )

            // let interopModule = SynModuleDecl.CreateNestedModule()

            let filledComponents =
                components
                |> List.map (fun c ->
                    let cmp = Core.extendComponent methodMap c
                    let name = Core.typeName cmp

                    // TODO: append to list to accumulate
                    let cmpInterface =
                        SynTypeDefn.Simple($"I{name.ToUpper()}Property")
                    // TODO: new declaration
                    let attr =
                        Core.createAttr ($"I{name.ToUpper()}Attr")

                    cmp)

            let componentInfo =
                SynComponentInfo.Create [ Ident.Create "example1" ]

            let allTypes =
                (List.map Core.removeAttribute<Generator.IncludedAttribute> included)
                @ filledComponents

            let nestedModule =
                SynModuleDecl.CreateNestedModule(
                    componentInfo,
                    allTypes
                    |> List.map (fun t -> SynModuleDecl.Types([ t ], Range.Zero))
                )

            let namespaceOrModule =
                SynModuleOrNamespace.CreateNamespace(Ident.CreateLong "hello", decls = [ nestedModule ])

            Output.Ast [ namespaceOrModule ]
