namespace Generated.Feliz

open Myriad.Plugins
open Fable.Core.JsInterop
open Fable.Core
open Feliz


[<Generator.Methods>]
type WithStyle =
    static member inline style(properties: #IStyleAttribute list) =
        Interop.attr "style" (createObj !!properties)



[<Generator.Methods>]
type WithClass =
    static member inline classNames(value: string) = Interop.attr "className" value

    static member inline className(names: seq<string>) =
        Interop.attr "className" (String.concat " " names)

[<Generator.Methods>]
type WithChildren =
    static member inline children(elements: Fable.React.ReactElement list) =
        unbox<Interop.inlined> (prop.children elements)

// Components

[<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>)>]
type button() =
    static member inline create(properties: Interop.inlined list) =
        Interop.reactApi.createElement (import "Button" "antd", createObj !!properties)

    static member inline disabled(value: bool) = Interop.attr "disabled" value
    static member inline label(value: string) = Interop.attr "label" value



[<Generator.Included>]
type Layout =
    { Header: obj
      Content: obj
      Footer: obj }

[<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
type layout =
    static member inline create(properties: Interop.inlined list) =
        Interop.reactApi.createElement (import "Layout" "antd", createObj !!properties)

    static member inline hasSider(value: bool) = Interop.attr "hasSider" value


[<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
type layoutHeader =
    static member inline create(properties: Interop.inlined list) =
        Interop.reactApi.createElement ((import<Layout> "Layout" "antd").Header, createObj !!properties)

    static member inline hasSider(value: bool) = Interop.attr "hasSider" value


[<Generator.LibraryRoot>]
type Antd =
    class
    end
