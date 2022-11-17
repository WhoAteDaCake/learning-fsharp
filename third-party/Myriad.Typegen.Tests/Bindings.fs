namespace Generated.Feliz

open Myriad.Plugins
open Fable.Core.JsInterop
open Fable.Core
open Feliz


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


[<Generator.ModuleRoot>]
module RootModule =
    [<Generator.Included>]
    type Layout =
        { Headersss: obj
          Content: obj
          Footer: obj }

    [<Generator.Included>]
    type ColFlex =
        | Number of value: int
        | None
        | Auto
        | String of value: string


    [<Generator.Component;
      Generator.ExtendsMethods(typeof<WithClass>,
                               //
                               // typeof<WithClassTest>,
                               typeof<WithChildren>)>]
    type button() =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (import "Layout" "antd", createObj !!properties)

        static member inline disabled(value: bool) = Interop.attr "disabled" value

        static member inline flex(value: ColFlex) =
            let attr = "flex"
            let fn = Interop.attr

            match value with
            | None -> fn attr "none"
            | Auto -> fn attr "auto"
            | String value -> fn attr value
            | Number value -> fn attr value




[<Generator.LibraryRoot>]
type Antd =
    class
    end
