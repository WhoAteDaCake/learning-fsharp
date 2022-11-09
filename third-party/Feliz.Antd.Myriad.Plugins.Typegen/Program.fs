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

            let filledComponents = components |> List.map (Core.extendComponent methodMap)
            let test1 =
                Core.modifyStaticMembers (fun m -> m |> List.map Core.replaceInteropInMember) methods[1]

            let componentInfo =
                SynComponentInfo.Create [ Ident.Create "example1" ]

            let nestedModule =
                SynModuleDecl.CreateNestedModule(componentInfo, [ SynModuleDecl.Types(filledComponents, Range.Zero) ])

            let namespaceOrModule =
                SynModuleOrNamespace.CreateNamespace(Ident.CreateLong "hello", decls = [ nestedModule ])

            Output.Ast [ namespaceOrModule ]
