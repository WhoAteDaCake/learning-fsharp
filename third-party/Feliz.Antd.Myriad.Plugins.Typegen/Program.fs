namespace Myriad.Plugins

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

module Interop =
    let attr (key: string) (value: obj) = unbox (key, value)

    type inlined = obj

    type Extends([<ParamArray>] classes: Type array) =
        member this.classes = classes

[<RequireQualifiedAccess>]
module Generator =
    type Component(name: string) =
        inherit Attribute()

    /// Mark a class as method only, this way we can re-use the code
    /// These classes will be removed in the final produced file
    type Methods() =
        inherit Attribute()

    /// A marker to include an additional type
    type Included() =
        inherit Attribute()

module Core =
    let sortTypes acc (_, types) =
        types
        |> List.fold
            (fun (included, methods, components) typeDef ->
                if hasAttribute<Generator.Component> (typeDef) then
                    (included, methods, typeDef :: components)
                elif hasAttribute<Generator.Methods> (typeDef) then
                    (included, typeDef :: methods, components)
                elif hasAttribute<Generator.Included> (typeDef) then
                    (typeDef :: included, methods, components)
                else
                    (included, methods, components))
            acc


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

            let namespaceAndrecords =
                extractRecords ast
                |> List.choose (fun (ns, types) ->
                    match types
                          |> List.filter hasAttribute<Generator.Methods>
                        with
                    | [] -> None
                    | types -> Some(ns, types))

            let letPattern =
                SynPat.CreateNamed(Ident.Create "fortyTwo")

            let let42 =
                SynModuleDecl.CreateLet [ SynBinding.Let(
                                              pattern = letPattern,
                                              expr = SynExpr.CreateConst(SynConst.Int32 42)
                                          ) ]

            let componentInfo =
                SynComponentInfo.Create [ Ident.Create "example1" ]

            let nestedModule =
                SynModuleDecl.CreateNestedModule(componentInfo, [ let42 ])

            let namespaceOrModule =
                SynModuleOrNamespace.CreateNamespace(Ident.CreateLong "hello", decls = [ nestedModule ])

            Output.Ast [ namespaceOrModule ]
