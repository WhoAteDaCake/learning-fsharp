namespace Generated.Feliz

open System
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

    [<Generator.Included;StringEnum;RequireQualifiedAccess>]
    type RowAlign =
        | Top
        | Middle
        | Bottom
        | Stretch

    [<Generator.Included;StringEnum;RequireQualifiedAccess>]
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

    // Tree START
    [<Generator.Included;StringEnum; RequireQualifiedAccess>]
    type DirectoryTreeExpandAction =
        | Click
        | DoubleClick

    [<Generator.Included>]
    type TreeData =
        {
            key: U2<string, int>
            title: U2<ReactElement, Func<TreeData, ReactElement>>
            icon: U2<ReactElement, Func<TreeData, ReactElement>> option
            children: TreeData array
            disabled: bool
            selectable: bool
        }
    [<Generator.Included>]
    type TreeMouseEvent<'T> =
        {
            event: React
            node: 'T
        }
    [<Generator.Included>]
    type TreeDropEvent<'T> =
        {
            event: Event
            node: 'T
            dragNode: 'T
            dragNodesKeys: string array
            dropPosition: int
            dropToGap: bool
        }
    [<Generator.Included>]
    type TreeSelectedEvent<'T> =
        {
            selected: bool
            selectedNodes: 'T array
            node: 'T
            event: Event
        }
    [<Generator.Included>]
    type TreeExpandEvent<'T> =
        {
            expanded: bool
            node: 'T
        }
    [<Generator.Included>]
    type TreeCheckEvent<'T> =
        {
            [<CompiledName("checked")>]
            isChecked: bool
            checkedNodes: 'T array
            event: Event
            halfCheckedKeys: string array
        }

    [<Generator.Component; Generator.ExtendsMethods(typeof<WithClass>, typeof<WithChildren>, typeof<WithStyle>)>]
    type tree =
        static member inline create(properties: Interop.inlined list) =
            Interop.reactApi.createElement (
                (import "Tree" "antd"),
                createObj !!properties
            )
        static member inline autoExpandParent (v: bool) = Interop.attr "autoExpandParent" v
        static member inline blockNode (?v: bool) = Interop.attr "blockNode" (Option.defaultValue true v)
        static member inline checkable (?v: bool) = Interop.attr "checkable" (Option.defaultValue true v)
        static member inline checkedKeys (v: string seq) = Interop.attr "checkedKeys" (Array.ofSeq v)
        static member inline checkStrictly (?v: bool) = Interop.attr "checkStrictly" (Option.defaultValue true v)
        static member inline defaultCheckedKeys (v: string seq) = Interop.attr "defaultCheckedKeys" (Array.ofSeq v)
        static member inline defaultExpandAll (?v: bool) = Interop.attr "defaultExpandAll" (Option.defaultValue true v)
        static member inline defaultExpandedKeys (v: string seq) = Interop.attr "defaultExpandedKeys" (Array.ofSeq v)
        static member inline defaultExpandParent (?v: bool) = Interop.attr "defaultExpandParent" (Option.defaultValue true v)
        static member inline defaultSelectedKeys (v: string seq) = Interop.attr "defaultSelectedKeys" (Array.ofSeq v)
        static member inline disabled (?v: bool) = Interop.attr "disabled" (Option.defaultValue true v)
        static member inline draggable (?v: bool) = Interop.attr "draggable" (Option.defaultValue true v)
        static member inline expandedKeys (v: string seq) = Interop.attr "expandedKeys" (Array.ofSeq v)
        static member inline filterTreeNode (v: 'TEntity -> bool) = Interop.attr "filterTreeNode" v
        static member inline height (v: float) = Interop.attr "height" v
        static member inline icon (v: ReactElement) = Interop.attr "icon" v
        static member inline loadData (v: 'T -> unit) = Interop.attr "loadData" v
        static member inline loadedKeys (v: string seq) = Interop.attr "loadedKeys" (Array.ofSeq v)
        static member inline multiple (?v: bool) = Interop.attr "multiple" (Option.defaultValue true v)
        static member inline selectable (?v: bool) = Interop.attr "selectable" (Option.defaultValue true v)
        static member inline selectedKeys (v: string seq) = Interop.attr "selectedKeys" (Array.ofSeq v)
        static member inline showIcon (?v: bool) = Interop.attr "showIcon" (Option.defaultValue true v)
        static member inline showLine (?v: bool) = Interop.attr "showLine" (Option.defaultValue true v)
        static member inline switcherIcon (v: ReactElement) = Interop.attr "switcherIcon" v
        static member inline titleRender (v: Func<'TEntity, ReactElement>) = Interop.attr "titleRender" v
        static member inline treeData (v: TreeData seq) = Interop.attr "treeData" (Array.ofSeq v)
        static member inline virtualize (?v: bool) = Interop.attr "virtual" (Option.defaultValue true v)
        static member inline onCheck (v: Func<string array, TreeCheckEvent<'T>, unit>) = Interop.attr "onCheck" v
        static member inline onDragEnd (v: TreeMouseEvent<'TEntity> -> unit) = Interop.attr "onDragEnd" v
        static member inline onDragEnter (v: TreeMouseEvent<'TEntity> -> unit) = Interop.attr "onDragEnter" v
        static member inline onDragLeave (v: TreeMouseEvent<'TEntity> -> unit) = Interop.attr "onDragLeave" v
        static member inline onDragOver (v: TreeMouseEvent<'TEntity> -> unit) = Interop.attr "onDragOver" v
        static member inline onDragStart (v: TreeMouseEvent<'TEntity> -> unit) = Interop.attr "onDragStart" v
        static member inline onDrop (v: TreeDropEvent<'TEntity> -> unit) = Interop.attr "onDrop" v
        static member inline onExpand<'T> (v: Func<string array, TreeExpandEvent<'T>, unit>) = Interop.attr "onExpand" v
        static member inline onLoad (v: Func<string array, TreeMouseEvent<'T>, unit>) = Interop.attr "onLoad" v
        static member inline onRightClick (v: TreeMouseEvent<'TEntity> -> unit) = Interop.attr "onRightClick" v
        static member inline onSelect (v: Func<string array, TreeSelectedEvent<'T>, unit>) = Interop.attr "onSelect" v


[<Generator.LibraryRoot>]
type Antd =
    class
    end
