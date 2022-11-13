namespace Generated.Feliz

open Myriad.Plugins
open Fable.Core.JsInterop
open Fable.Core
open Feliz


[<Generator.Methods>]
type WithClass =
    static member inline classNames(value: string) = Interop.attr "className" value

    static member inline className(names: seq<string>) =
        Interop.attr "className" (String.concat " " names)

[<Generator.Methods>]
type WithChildren =
    static member inline children(elements: Fable.React.ReactElement list) =
        unbox<Interop.inlined> (prop.children elements)

[<Generator.Included>]
type Layout =
    { Header: obj
      Content: obj
      Footer: obj }

[<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>)>]
type button() =
    static member inline create(properties: Interop.inlined list) =
        Interop.reactApi.createElement (import "Layout" "antd", createObj !!properties)

    static member inline disabled(value: bool) = Interop.attr "disabled" value


[<Generator.LibraryRoot>]
type Antd =
    class
    end
