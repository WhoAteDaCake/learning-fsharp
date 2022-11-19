module Index

open Client.AppLayout
open Feliz
open Feliz.Router
open Feliz.AntdReact
open Elmish
open Fable.Core
open Client.Pages
open Client.Routing


[<RequireQualifiedAccess>]
type Page =
    | Home of Home.Model
    | NotFound
    | Bookmark

type Model = { Page: Page; Url: Url }

type Msg =
    | HomeMsg of Home.Msg
    | UrlChanged of Url

let init () : Model * Cmd<Msg> =
    let initialUrl =
        parseUrl (Router.currentPath ())

    let page, cmd =
        match initialUrl with
        | Url.Home ->
            let model, cmd = Home.init ()
            Page.Home model, Cmd.map HomeMsg cmd
        | Url.NotFound -> Page.NotFound, Cmd.none
        | Url.Bookmarks -> Page.Bookmark, Cmd.none

    { Page = page; Url = initialUrl }, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg, model.Page with
    | HomeMsg msg, Page.Home state ->
        let data, cmd = Home.update msg state
        { model with Page = Page.Home data }, Cmd.map HomeMsg cmd
    | UrlChanged newUrl, _ ->
        let show page =
            { model with Page = page; Url = newUrl }
        match newUrl with
        | Url.Home ->
            let model, cmd = Home.init ()
            show (Page.Home model), Cmd.map HomeMsg cmd
        | Url.NotFound -> show Page.NotFound, Cmd.none
        | Url.Bookmarks -> show Page.Bookmark, Cmd.none
    //
    | _, _ -> model, Cmd.none

JsInterop.importAll "${outDir}/../styles/styles.less"

let view (model: Model) (dispatch: Msg -> unit) =
    let currentPage =
        match model.Page with
        | Page.Home state -> Home.view state (HomeMsg >> dispatch)
        | Page.NotFound -> Html.div [ prop.text "Not found" ]
        | Page.Bookmark -> Html.div [ prop.text "Bookmarks" ]

    let layout =
        Antd.layout [
            layout.children [
                Antd.layoutHeader [
                    layoutHeader.children [
                        AppHeader.view model.Url
                    ]
                ]
                currentPage
            ]
        ]

    Feliz.React.router [
        router.pathMode
        router.onUrlChanged (parseUrl >> UrlChanged >> dispatch)
        router.children [ layout ]
    ]