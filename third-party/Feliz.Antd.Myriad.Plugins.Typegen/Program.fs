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


            let rootModule =
                let found =
                    match Core.findRoot ast with
                    | Some(n) -> n
                    | None -> failwith "Could not find Root module"

                found
                    .appendAttribute("Erase")
                    .removeAttribute<Generator.RootModuleAttribute>()

            let included, methods, components =
                (extractTypeDefn ast)
                |> List.fold Core.sortTypes ([], [], [])

            let methodMap =
                Map(
                    methods
                    |> List.map (fun m -> (Core.typeName m, m))
                )

            let filledComponents, interfaces, attributes, creations =
                components
                |> List.fold (fun (components, interfaces, attributes, creations) c ->
                    let cmp = (Core.extendComponent methodMap c)

                    // Extract creation method here
                    let action, cmp = Core.modifyStaticMembers2 (Core.findAndRemove "create") cmp
                    let name = Core.typeName cmp

                    let creation =
                        match Option.map (fun (m: SynMemberDefn) -> m.Rename name) action with
                        | Some(mDef) -> mDef
                        | None -> failwith $"Could not detect creation method for {c}"

                    let newName = name.Substring(0, 1).ToUpper() + name.Substring(1)
                    let propertyName = $"I{newName}Property"

                    let cmpInterface =
                        SynTypeDefn.Simple(propertyName)

                    let attrName = $"mk{newName}Attr"
                    let attr =
                        Builtin.createAttr propertyName attrName

                    let cmp = Core.modifyStaticMembers (List.map (Core.replaceInteropInMember attrName)) cmp

                    (cmp :: components, cmpInterface :: interfaces, attr :: attributes, creation :: creations)
                ) ([], [], [], [])

            let interfaceDecl =
                SynModuleDecl.Types(interfaces, range0)

            let interopModule =
                SynModuleDecl.CreateNestedModule(
                    SynComponentInfo.Create [ Ident.Create "Interop" ],
                    attributes
                )
            let componentInfo =
                SynComponentInfo.Create [ Ident.Create "example1" ]

            let allTypes =
                (included |> List.map (fun n -> n.removeAttribute<Generator.IncludedAttribute>()))
                @ filledComponents

            let nestedModule =
                SynModuleDecl.CreateNestedModule(
                    componentInfo,
                    allTypes
                    |> List.map (fun t -> SynModuleDecl.Types([ t ], Range.Zero))
                )

            let namespaceOrModule =
                SynModuleOrNamespace.CreateNamespace(Ident.CreateLong "hello", decls = [
                    interfaceDecl; interopModule; nestedModule; rootModule
                ])

            Output.Ast [ namespaceOrModule ]
