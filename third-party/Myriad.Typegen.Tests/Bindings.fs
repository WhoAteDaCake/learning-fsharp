module Myriad.Typegen.Tests.Bindings

open Myriad.Typegen.Tests.Base
open Myriad.Plugins


[<Generator.Methods>]
type WithClass =
    static member inline classNames(value: string) = Interop.attr "className" value

    static member inline className(names: seq<string>) =
        Interop.attr "className" (String.concat " " names)

[<Generator.Methods>]
type WithClassTest =
    static member inline classNames(value: string) = Interop.attr "className" value

    static member inline className(names: seq<string>) =
        Interop.attr "className" (String.concat " " names)

[<Generator.Methods>]
type WithChildren =
    static member inline children(elements: string list) =
        unbox<Interop.inlined> (prop.children elements)

[<
    Generator.Component("button");
    Generator.ExtendsMethods(
        typeof<WithClass>,
        // typeof<WithClassTest>,
        typeof<WithChildren>
    )
>]
type ButtonMethods() =
    static member inline disabled(value: bool) = Interop.attr "disabled" value
