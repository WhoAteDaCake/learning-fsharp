module Client.AppLayout.AppHeader

open Browser.Types
open Client.Routing
open Fable.Core.JS
open Fake.Core
open Feliz
open Feliz.Router
open Feliz.AntdReact
open Elmish

type Model = { CurrentUrl : Url }
type Msg =
    | NavigateTo of string

let update msg state =
  match msg with
  | NavigateTo href -> state, Cmd.navigatePath(href)

let goToUrl (dispatch: Msg -> unit) (href: string) (e: MouseEvent) =
    // disable full page refresh
    e.preventDefault()
    // dispatch msg
    dispatch (NavigateTo href)

let init (url: Url) : Model * Cmd<Msg> =
    {CurrentUrl = url}, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    let offset = 24 / 8
    let items =
        topLevelRoutes
        |> List.map (fun (rUrl, key, name) ->
            let color =
                if rUrl = model.CurrentUrl then
                    style.color "#1677ff"
                else
                    style.color "#fff"
            let href = Router.formatPath key
            MenuItemType.MenuItemType(
                {| danger = Some false
                   disabled = Some false
                   icon = None
                   key = key
                   label = Html.a [
                       prop.onClick (goToUrl dispatch href)
                       prop.href href
                       prop.style [color]
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
                    Html.div [
                        prop.children [
                            Antd.menu [
                                menu.theme MenuTheme.Dark
                                menu.mode MenuMode.Horizontal
                                menu.items items
                                menu.selectedKeys [
                                    urlToString model.CurrentUrl
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]