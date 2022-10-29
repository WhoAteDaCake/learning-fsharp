namespace Feliz.AntdReact

open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type layout =
    inherit Children<ILayoutProperty>
    static member inline className(value: string) = Interop.mkLayoutAttr "className" value

    static member inline className(names: seq<string>) =
        Interop.mkLayoutAttr "className" (String.concat " " names)

    static member inline hasSider(value: bool) = Interop.mkLayoutAttr "hasSider" value

    static member inline style(properties: #Feliz.IStyleAttribute list) =
        Interop.mkLayoutAttr "style" (createObj !!properties)
