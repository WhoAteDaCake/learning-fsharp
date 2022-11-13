namespace Feliz

open Myriad.Plugins
open Fable.Core.JsInterop
open Fable.Core


[<Generator.Methods>]
type WithClass =
    static member inline classNames(value: string) = Interop.attr "className" value

    static member inline className(names: seq<string>) =
        Interop.attr "className" (String.concat " " names)

[<Generator.Methods>]
type WithClassTest =
    static member inline classNames1(value: string) = Interop.attr "className" value

    static member inline className2(names: seq<string>) =
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

[<Generator.Component;
  Generator.ExtendsMethods(typeof<WithClass>,
                           //
                           // typeof<WithClassTest>,
                           typeof<WithChildren>)>]
type button() =
    static member inline create(properties: Interop.inlined list) =
        Interop.reactApi.createElement (import "Layout" "antd", createObj !!properties)
    static member inline disabled(value: bool) = Interop.attr "disabled" value


[<Generator.LibraryRoot>]
type Antd = class end