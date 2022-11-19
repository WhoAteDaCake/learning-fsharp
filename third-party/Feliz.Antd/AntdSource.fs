namespace Generated.Feliz

open Browser.Types
open Myriad.Plugins
open Fable.Core.JsInterop
open Fable.Core
open Feliz


[<Generator.Methods>]
type WithStyle =
    static member inline style(properties: #IStyleAttribute list) =
        Interop.attr "style" (createObj !!properties)



[<Generator.Methods>]
type WithClass =
    static member inline classNames(value: string) = Interop.attr "className" value

    static member inline className(names: seq<string>) =
        Interop.attr "className" (String.concat " " names)

[<Generator.Methods>]
type WithChildren =
    static member inline children(elements: Fable.React.ReactElement list) =
        unbox<Interop.inlined> (prop.children elements)

[<Generator.ModuleRoot>]
module rec AntdReact =
    [<Generator.Included>]
    type Layout =
        { Header: obj
          Content: obj
          Footer: obj }

    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>)>]
    type button() =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (import "Button" "antd", createObj !!properties)

        static member inline disabled(value: bool) = Interop.attr "disabled" value
        static member inline label(value: string) = Interop.attr "label" value


    [<Generator.Included>]
    [<StringEnum>]
    type RowAlign =
        | Top
        | Middle
        | Bottom
        | Stretch

    [<Generator.Included>]
    [<StringEnum>]
    type RowJustify =
        | Start
        | End
        | Center
        | [<CompiledName("spaced-around")>] SpaceAround
        | [<CompiledName("spaced-between")>] SpaceBetween
        | [<CompiledName("spaced-evenly")>] SpaceEvenly

    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type row =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (import "Row" "antd", createObj !!properties)

        static member inline align(value: RowAlign) = Interop.attr "align" value
        static member inline justify(value: RowJustify) = Interop.attr "justify" value

        static member inline wrap(value: bool) = Interop.attr "wrap" value

    [<Generator.Included; RequireQualifiedAccess>]
    type ColFlex =
        | Number of value: int
        | None
        | Auto
        | String of value: string

    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type col =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (import "Col" "antd", createObj !!properties)

        static member inline span(value: int) = Interop.attr "span" value
        static member inline pull(value: int) = Interop.attr "pull" value
        static member inline push(value: int) = Interop.attr "push" value
        static member inline offset(value: int) = Interop.attr "offset" value

        static member inline flex(value: ColFlex) =
            let output: obj =
                match value with
                | ColFlex.None -> "none"
                | ColFlex.Auto -> "auto"
                | ColFlex.String value -> value
                | ColFlex.Number value -> value

            Interop.attr "flex" output


    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type layout =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (import "Layout" "antd", createObj !!properties)

        static member inline hasSider(value: bool) = Interop.attr "hasSider" value


    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type layoutHeader =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement ((import<AntdReact.Layout> "Layout" "antd").Header, createObj !!properties)

        static member inline hasSider(value: bool) = Interop.attr "hasSider" value


    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type layoutContent =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement ((import<AntdReact.Layout> "Layout" "antd").Content, createObj !!properties)

    [<Generator.Included>]
    type MenuItemType =
        | MenuItemType of
            {| danger: bool option
               disabled: bool option
               icon: ReactElement option
               key: string
               label: ReactElement
               title: string |}

    [<Generator.Included; StringEnum;RequireQualifiedAccess>]
    type MenuMode =
        | Horizontal
        | Inline
        | [<CompiledName("vertical-left")>] VerticalLeft
        | [<CompiledName("vertical-right")>] VerticalRight


    [<Generator.Included; StringEnum;RequireQualifiedAccess>]
    type MenuTheme =
        | Dark
        | Light

    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type menu =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (import "Menu" "antd", createObj !!properties)

        static member inline defaultOpenKeys(value: string seq) = Interop.attr "defaultOpenKeys" (Array.ofSeq value)

        static member inline defaultSelectedKeys(value: string seq) =
            Interop.attr "defaultSelectedKeys" (Array.ofSeq value)

        static member inline selectedKeys(value: string seq) =
            Interop.attr "selectedKeys" (Array.ofSeq value)


        static member inline mode(value: MenuMode) = Interop.attr "mode" value

        static member inline theme(value: MenuTheme) = Interop.attr "theme" value

        static member inline items(value: MenuItemType list) =
            let attrs =
                List.map
                    (function
                    | MenuItemType obj -> obj)
                    value

            Interop.attr "items" (Array.ofSeq attrs)


    [<Generator.Included>]
    type BreadcrumbImport = { Item: obj }

    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type breadcrumb =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (import "Breadcrumb" "antd", createObj !!properties)

    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type breadcrumbItem =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (
                (import<AntdReact.BreadcrumbImport> "Breadcrumb" "antd")
                    .Item,
                createObj !!properties
            )

        static member inline href(value: string) = Interop.attr "href" value

        static member inline onClick(value: MouseEvent -> unit) = Interop.attr "onClick" value



[<Generator.LibraryRoot>]
type Antd =
    class
    end
