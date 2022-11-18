module Client.AppLayout.AppHeader

open Client.Routing
open Fable.Core.JS
open Fake.Core
open Feliz
open Feliz.AntdReact

let view (url: Url) =
    let offset = 24 / 8

    let items =
        topLevelRoutes
        |> List.map (fun (rUrl, key, name) ->
            let textProps =
                if rUrl.Equals url then
                    [style.fontWeight 700]
                else
                     []
            MenuItemType.MenuItemType(
                {| danger = Some false
                   disabled = Some false
                   icon = None
                   key = key
                   label = Html.span [
                       prop.style textProps
                       prop.text name
                   ]
                   title = name |}
            ))

    Antd.row [
        row.children [
            Antd.col [
                col.span (24 - offset)
                col.offset offset
                col.children [
                    Antd.menu [
                        menu.theme MenuTheme.Dark
                        menu.mode MenuMode.Horizontal
                        menu.items items
                        menu.selectedKeys [
                            urlToString url
                        ]
                    ]
                ]
            ]
        ]
    ]