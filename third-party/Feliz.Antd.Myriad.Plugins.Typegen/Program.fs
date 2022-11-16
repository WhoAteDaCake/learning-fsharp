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

            let filledComponents, interfaces, attributes, creations =
                components
                |> List.fold
                    (fun (components, interfaces, attributes, creations) c ->
                        let cmp =
                            (Core.extendComponent methodMap c)
                                .appendAttribute("Erase")
                                .removeAttribute<Generator.ComponentAttribute> ()

                        let name = Core.typeName cmp

                        let newName =
                            name.Substring(0, 1).ToUpper() + name.Substring(1)

                        let propertyName = $"I{newName}Property"

                        let cmpInterface =
                            SynTypeDefn.Simple(propertyName)

                        let attrName = $"mk{newName}Attr"

                        let attr =
                            Builtin.createAttr propertyName attrName

                        // TODO: Make sure it's replaced in arguments as well.
                        let modifier =
                            (Core.replaceInteropTypeInMember propertyName)
                            >> (Core.replaceInteropInMember attrName)

                        let cmp =
                            Core.modifyStaticMembers (List.map modifier) cmp


                        // Extract creation method here
                        let action, cmp =
                            Core.modifyStaticMembers2 (Core.findAndRemove "create") cmp

                        let creation =
                            match Option.map (fun (m: SynMemberDefn) -> m.Rename name) action with
                            | Some (mDef) -> mDef
                            | None -> failwith $"Could not detect creation method for {c}"

                        (cmp :: components, cmpInterface :: interfaces, attr :: attributes, creation :: creations))
                    ([], [], [], [])

            let rootType =
                let found =
                    (extractTypeDefn ast)
                    |> List.collect (fun (_, x) -> x)
                    |> List.find (fun x -> hasAttribute<Generator.LibraryRootAttribute> x)

                found
                    .appendAttribute("Erase")
                    .removeAttribute<Generator.LibraryRootAttribute>()
                    .addStaticMembers (creations)

            let interfaceDecl =
                SynModuleDecl.Types(interfaces, range0)

            let interopModule =
                SynModuleDecl
                    .CreateNestedModule(SynComponentInfo.Create [ Ident.Create "Interop" ], attributes)
                    .appendAttribute("Erase")
                    .appendAttribute ("RequireQualifiedAccess")

            let allTypes =
                (included
                 |> List.map (fun n -> n.removeAttribute<Generator.IncludedAttribute> ()))
                @ filledComponents

            // Create separate types to avoid "and" like type declaration
            let typeDecls =
                List.map (fun t -> SynModuleDecl.Types([ t ], Range.Zero)) allTypes

            let rootNsDecls, rootNsName =
                let extracted =
                    match ast with
                    | ParsedInput.ImplFile (ParsedImplFileInput (_, _, _, _, _, modules, _)) -> modules[0]
                    | _ -> failwith "Could not find root module"

                extracted.Decls(), extracted.LongId()

            let opens =
                rootNsDecls
                |> List.filter (function
                    | SynModuleDecl.Open (SynOpenDeclTarget.ModuleOrNamespace (longIdent, _), _) ->
                        match List.map (fun (x: Ident) -> x.idText) longIdent with
                        | "Myriad" :: _ -> false
                        | _ -> true
                    | _ -> false)

            let componentModuleName =
                rootNsDecls
                |> List.pick (fun (d: SynModuleDecl) ->
                    if d.hasAttribute<Generator.ModuleRootAttribute> () then
                        Some(d.Name())
                    else
                        None)

            let componentModule =
                SynModuleDecl
                    .CreateNestedModule(SynComponentInfo.Create(componentModuleName), typeDecls)
                    .appendAttribute ("Erase")


            let namespaceOrModule =
                SynModuleOrNamespace.CreateNamespace(
                    List.filter (fun (i: Ident) -> i.idText <> "Generated") rootNsName,
                    decls =
                        opens
                        @ [ interfaceDecl
                            interopModule
                            componentModule
                            SynModuleDecl.Types([ rootType ], Range.Zero) ]
                )

            Output.Ast [ namespaceOrModule ]
