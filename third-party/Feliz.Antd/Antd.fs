namespace Feliz

open Feliz
open Fable.Core
open Fable.Core.JsInterop
open Feliz.AntdReact

[<Erase>]
type Antd =
    static member inline button(properties: IButtonProperty list) =
        Feliz.Interop.reactApi.createElement (import "Button" "antd", createObj !!properties)

    static member inline layout(properties: ILayoutProperty list) =
        Feliz.Interop.reactApi.createElement (import "Layout" "antd", createObj !!properties)
