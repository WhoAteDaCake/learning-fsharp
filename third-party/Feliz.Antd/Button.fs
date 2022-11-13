namespace Feliz.AntdReact
//
// open Fable.Core
// open Feliz
//
// [<Erase>]
// type button =
//     static member inline className(value: string) = Interop.mkButtonAttr "className" value
//
//     static member inline className(names: seq<string>) =
//         Interop.mkButtonAttr "className" (String.concat " " names)
//
//     static member inline disabled(value: bool) = Interop.mkButtonAttr "disabled" value
//     static member inline label(value: string) = Interop.mkButtonAttr "label" value
//
//     static member inline children(elements: ReactElement list) =
//         unbox<IButtonProperty> (prop.children elements)
