namespace Feliz

open Fable.Core
open Fable.Core.JsInterop
open Feliz.AntdReact

type Layout =
    { Header: obj
      Content: obj
      Footer: obj }

[<Erase>]
type Antd =
    static member inline button(properties: IButtonProperty list) =
        Feliz.Interop.reactApi.createElement (import "Button" "antd", createObj !!properties)

    static member inline layout(properties: ILayoutProperty list) =
        Feliz.Interop.reactApi.createElement (import "Layout" "antd", createObj !!properties)

    static member inline layoutHeader(properties: ILayoutHeaderProperty list) =
        Feliz.Interop.reactApi.createElement ((import<Layout> "Layout" "antd").Header, createObj !!properties)
