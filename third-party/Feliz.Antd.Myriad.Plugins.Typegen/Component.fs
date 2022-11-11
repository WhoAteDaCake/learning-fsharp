module Feliz.Antd.Myriad.Plugins.Typegen.Component

open Feliz.Antd.Myriad.Plugins.Typegen.Extensions
open FSharp.Compiler.Syntax

let makeProperty name =
    SynTypeDefn.Simple()
