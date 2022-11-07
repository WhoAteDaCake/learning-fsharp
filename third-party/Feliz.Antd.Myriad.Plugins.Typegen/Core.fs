module Feliz.Antd.Myriad.Plugins.Typegen.Core

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
open Myriad.Plugins


let typeName (SynTypeDefn (typeInfo, _, _, _, _, _)) =
    match typeInfo with
    | SynComponentInfo (_, _, _, longId, _, _, _, _) -> longId[0].idText

let sortTypes acc (_, types) =
    types
    |> List.fold
        (fun (included, methods, components) typeDef ->
            if hasAttribute<Generator.ComponentAttribute> (typeDef) then
                (included, methods, typeDef :: components)
            elif hasAttribute<Generator.MethodsAttribute> (typeDef) then
                (included, typeDef :: methods, components)
            elif hasAttribute<Generator.IncludedAttribute> (typeDef) then
                (typeDef :: included, methods, components)
            else
                (included, methods, components))
        acc
