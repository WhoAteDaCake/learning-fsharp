//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------



namespace hello

type IButtonProperty =
    interface
    end

module Interop =
    let inline mkButtonAttr (key: string) (value: obj) : IButtonProperty = unbox (key, value)

module example1 =

    type Layout =
        { Header: obj
          Content: obj
          Footer: obj }

    [<Generator.Component>]
    type button() =
        static member inline children(elements: string list) =
            unbox<Interop.inlined> (prop.children elements)

        static member inline classNames(value: string) = Interop.mkButtonAttr "className" value

        static member inline className(names: seq<string>) =
            Interop.mkButtonAttr "className" (String.concat " " names)

        static member inline classNames(value: string) = Interop.mkButtonAttr "className" value

        static member inline className(names: seq<string>) =
            Interop.mkButtonAttr "className" (String.concat " " names)

        static member inline create() = (import<Layout> "Layout" "antd").Header
        static member inline disabled(value: bool) = Interop.mkButtonAttr "disabled" value

[<Erase>]

module Antd =


