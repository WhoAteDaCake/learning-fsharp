namespace Generated.Feliz

open System
open Browser.Types
open Myriad.Plugins
open Fable.Core.JsInterop
open Fable.Core
open Feliz

[<Generator.Methods>]
type WithStyle =
    static member inline style(properties: #IStyleAttribute seq) =
        Interop.attr "style" (createObj !!(properties |> Array.ofSeq))

[<Generator.Methods>]
type WithClass =
    static member inline className(value: string) = Interop.attr "className" value

    static member inline className(names: seq<string>) =
        Interop.attr "className" (String.concat " " names)


[<Generator.ModuleRoot>]
module rec AntdIconsReact =
    [<Generator.Component; Generator.ExtendsMethods(typeof<WithStyle>, typeof<WithClass>)>]
    type folderOutlined() =
        static member inline create(properties: Interop.inlined seq) =
            Interop.reactApi.createElement (import "FolderOutlined" "@ant-design/icons", createObj !!(Array.ofSeq properties))

        static member inline disabled(value: bool) = Interop.attr "disabled" value
        static member inline label(value: string) = Interop.attr "label" value

[<Generator.LibraryRoot>]
type AntdIcons =
    class
    end
