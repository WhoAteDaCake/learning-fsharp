module Myriad.Typegen.Tests

open Myriad.Typegen.Tests.Base
open Myriad.Plugins


[<Generator.Methods>]
type WithClass =
    static member inline className(value: string) = Interop.attr "className" value

    static member inline className(names: seq<string>) =
        Interop.attr "className" (String.concat " " names)

[<Generator.Methods>]
type WithChildren =
    static member inline children(elements: string list) =
        unbox<Interop.inlined> (prop.children elements)

[<Generator.Component("button")>]
type ButtonMethods() =
    inherit Interop.Extends(
        typedefof<WithClass>,
        typedefof<WithChildren>
    )
    static member inline disabled(value: bool) = Interop.attr "disabled" value
