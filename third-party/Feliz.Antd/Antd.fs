namespace Feliz.Antd

open Feliz
open Fable.Core.JsInterop
open Feliz.Antd

module Antd =
    let inline button (properties: IButtonProperty list) =
        Feliz.Interop.reactApi.createElement (import "Button" "antd", createObj !!properties)
