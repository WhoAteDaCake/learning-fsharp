module Index

open Client
open Elmish
open Fable.Core
open Fable.Builders.AntDesign
open Client.Pages
open Client.Routing
open Feliz
open Feliz.Router

type Page =
    | Home of Home.Model
    | NotFound

type Model = { page: Page; url: Url;  }

type Msg =
    | HomeMsg of Home.Msg
    | UrlChanged of string list

let init () : Model * Cmd<Msg> =
    let initialUrl = parseUrl (Router.currentUrl())
    let page, cmd =
        match initialUrl with
        | Url.Home ->
            let model, cmd = Home.init ()
            Home model, Cmd.map HomeMsg cmd
        | Url.NotFound ->
            NotFound, Cmd.none
    { page = page; url = initialUrl; }, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg, model.page with
    | HomeMsg msg, Home state ->
        let data, cmd = Home.update msg state
        { model with page = Home data }, Cmd.map HomeMsg cmd
    | UrlChanged parts, _ ->
        let url = Routing.parseUrl parts
        { model with url = url }, Cmd.none
    | _, _ -> model, Cmd.none

JsInterop.importAll "${outDir}/../styles/styles.less"

let view (model: Model) (dispatch: Msg -> unit) =
    let currentPage =
        match model.page with
        | Home state -> Home.view state (HomeMsg >> dispatch)
        | NotFound -> str "Page not found"

    let layout =
        Layout {
            Header {
                str "Header here"
            }
            Content {
                currentPage
            }
        }

    React.router [
        router.onUrlChanged (UrlChanged >> dispatch)
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