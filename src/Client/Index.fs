module Index

open Feliz
open Feliz.Router
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
        parseUrl (Router.currentUrl ())

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
        Html.div [
            prop.children [ currentPage ]
        ]

    Feliz.React.router [
        router.onUrlChanged (parseUrl >> UrlChanged >> dispatch)
        router.children [ layout ]
    ]

// let view (model: Model) (dispatch: Msg -> unit) =
//     Bulma.hero [
//         hero.isFullHeight
//         color.isPrimary
//         prop.style [
//             style.backgroundSize "cover"
//             style.backgroundImageUrl "https://unsplash.it/1200/900?random"
//             style.backgroundPosition "no-repeat center center fixed"
//         ]
//         prop.children [
//             Bulma.heroHead [
//                 Bulma.navbar [
//                     Bulma.container [ navBrand ]
//                 ]
//             ]
//             Bulma.heroBody [
//                 Bulma.container [
//                     Bulma.column [
//                         column.is6
//                         column.isOffset3
//                         prop.children [
//                             Bulma.title [
//                                 text.hasTextCentered
//                                 prop.text "SafeApp"
//                             ]
//                             containerBox model dispatch
//                         ]
//                     ]
//                 ]
//             ]
//         ]
//     ]
