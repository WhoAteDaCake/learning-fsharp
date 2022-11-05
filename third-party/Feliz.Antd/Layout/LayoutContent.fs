namespace Feliz.AntdReact

open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type layoutContent =
    inherit Children<ILayoutContentProperty>
    static member inline className(value: string) = Interop.mkLayoutContentAttr "className" value

    static member inline className(names: seq<string>) =
        Interop.mkLayoutContentAttr "className" (String.concat " " names)

    static member inline style(properties: #Feliz.IStyleAttribute list) =
        Interop.mkLayoutContentAttr "style" (createObj !!properties)
