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
    | Bookmark of Bookmarks.Domain.Model
    | Import of Import.Domain.Model

type Model =
    { Page: Page
      Url: Url
      Header: AppHeader.Model }

type Msg =
    | HomeMsg of Home.Msg
    | HeaderMsg of AppHeader.Msg
    | BookmarkMsg of Bookmarks.Domain.Msg
    | ImportMsg of Import.Domain.Msg
    | UrlChanged of Url

let onPageChange = function
| Url.Home ->
    let model, cmd = Home.init ()
    Page.Home model, Cmd.map HomeMsg cmd
| Url.Bookmarks url ->
    let model, cmd = Bookmarks.State.init url
    Page.Bookmark model, Cmd.map BookmarkMsg cmd
| Url.Import url ->
    let model, cmd = Import.State.init url
    Page.Import model, Cmd.map ImportMsg cmd
| Url.NotFound -> Page.NotFound, Cmd.none

let onUrlChange (pageUrl: Url) (model: Model) =
    match pageUrl, model.Url, model.Page with
    | Url.Bookmarks newUrl, Url.Bookmarks oldUrl, Page.Bookmark state ->
        match Bookmarks.State.onUrlChange (newUrl, oldUrl) with
        | Bookmarks.Domain.Intent.Update msg ->
            let data, cmd = Bookmarks.State.update msg state
            { model with Page = Page.Bookmark data; Url = pageUrl }, Cmd.map BookmarkMsg cmd
        | Bookmarks.Domain.Intent.NoAction -> { model with Url = pageUrl }, Cmd.none
    | _ ->
        let page, pageCmd = onPageChange pageUrl
        // Ensure Header is informed
        let headerModel, headerCmd =
            AppHeader.init pageUrl

        let cmd =
            Cmd.batch [
                pageCmd
                Cmd.map HeaderMsg headerCmd
            ]

        { model with
            Page = page
            Url = pageUrl
            Header = headerModel },
        cmd

let init () : Model * Cmd<Msg> =
    let initialUrl =
        parseUrl (Router.currentPath ())

    let page, pageCmd = onPageChange initialUrl

    let appHeader, appCmd =
        AppHeader.init initialUrl

    let cmd =
        Cmd.batch [
            pageCmd
            Cmd.map HeaderMsg appCmd
        ]

    { Page = page
      Url = initialUrl
      Header = appHeader },
    cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg, model.Page with
    | HomeMsg msg, Page.Home state ->
        let data, cmd = Home.update msg state
        { model with Page = Page.Home data }, Cmd.map HomeMsg cmd
    | BookmarkMsg msg, Page.Bookmark state ->
        let data, cmd = Bookmarks.State.update msg state
        { model with Page = Page.Bookmark data }, Cmd.map BookmarkMsg cmd
    | ImportMsg msg, Page.Import state ->
        let data, cmd = Import.State.update msg state
        { model with Page = Page.Import data }, Cmd.map ImportMsg cmd
    | HeaderMsg msg, _ ->
        let data, cmd =
            AppHeader.update msg model.Header
        { model with Header = data }, Cmd.map HeaderMsg cmd
    | UrlChanged newUrl, _ ->
        onUrlChange newUrl model
    //
    | _, _ -> model, Cmd.none

JsInterop.importAll "${outDir}/../styles/styles.less"
// JsInterop.importAll "${outDir}/../styles/tailwind.css"

let view (model: Model) (dispatch: Msg -> unit) =
    let currentPage =
        match model.Page with
        | Page.Home state -> Home.view state (HomeMsg >> dispatch)
        | Page.NotFound -> Html.div [ prop.text "Not found" ]
        | Page.Bookmark state -> Bookmarks.View.view state (BookmarkMsg >> dispatch)
        | Page.Import state -> Import.View.view state (ImportMsg >> dispatch)

    let layout =
        Antd.layout [
            layout.children [
                Antd.layoutHeader [
                    layoutHeader.children [
                        AppHeader.view model.Header (HeaderMsg >> dispatch)
                    ]
                ]
                Antd.layoutContent [
                    layoutContent.children [
                        Html.div [
                            prop.classes ["m-4"]
                            prop.children [currentPage]
                        ]
                    ]
                ]
            ]
        ]

    Feliz.React.router [
        router.pathMode
        router.onUrlChanged (parseUrl >> UrlChanged >> dispatch)
        router.children [ layout ]
    ]
