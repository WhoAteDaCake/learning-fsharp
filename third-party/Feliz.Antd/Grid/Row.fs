namespace Feliz.AntdReact

open Fable.Core
open Fable.Core.JsInterop

// TODO: generator should allow raw passing of these types.
type RowAlign =
| Top
| Middle
| Bottom

type RowJustify =
| Start
| End
| Center
| [<CompiledName("space-around")>] SpaceAround
| [<CompiledName("space-between")>] SpaceBetween
| [<CompiledName("space-evenly")>] SpaceEvenly

[<Erase>]
type row =
    inherit Children<IRowProperty>
    static member inline className(value: string) = Interop.mkRowAttr "className" value

    static member inline className(names: seq<string>) =
        Interop.mkRowAttr "className" (String.concat " " names)

    static member inline style(properties: #Feliz.IStyleAttribute list) =
        Interop.mkRowAttr "style" (createObj !!properties)

    static member inline justify(value: RowJustify) =
        Interop.mkRowAttr "justify" value

    static member inline align(value: RowAlign) =
        Interop.mkRowAttr "align" value