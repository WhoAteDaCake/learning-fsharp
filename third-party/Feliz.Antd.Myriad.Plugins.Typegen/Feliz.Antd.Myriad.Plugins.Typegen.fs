namespace MyriadFeliz.Antd.Myriad.Plugins.Typegen

open System
open System.IO
open FSharp.Compiler
open Myriad.Core
open FSharp.Compiler.SyntaxTree
open FsAst

[<MyriadGenerator("example")>]
type Example() =
    interface IMyriadGenerator with
        member _.ValidInputExtensions = seq {".fs"}
        member _.Generate(context: GeneratorContext) =

            let exampleNamespace =
                context.ConfigKey
                |> Option.map context.ConfigGetter
                |> Option.bind (Seq.tryPick (fun (n,v) -> if n = "namespace" then Some (v :?> string) else None ))
                |> Option.defaultValue "UnknownNamespace"

            let letHelloWorld =
                SynModuleDecl.CreateLet
                    [ { SynBindingRcd.Let with
                            Pattern = SynPatRcd.CreateLongIdent(LongIdentWithDots.CreateString "hello", [])
                            Expr = SynExpr.CreateConst(SynConst.CreateString "world!") } ]


            let myModule =
                File.ReadAllLines context.InputFilename
                |> Seq.map (fun moduleName ->
                                    let componentInfo = SynComponentInfoRcd.Create [ Ident.Create (moduleName) ]
                                    let module' = SynModuleDecl.CreateNestedModule(componentInfo, [ letHelloWorld ])
                                    module')
                |> Seq.toList

            let namespaceOrModule =
                { SynModuleOrNamespaceRcd.CreateNamespace(Ident.CreateLong exampleNamespace)
                    with Declarations = myModule }

            [namespaceOrModule]