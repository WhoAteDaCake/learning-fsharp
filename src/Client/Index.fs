module Index

open Elmish
open Fable.Core
open Fable.Remoting.Client
open Shared
open Client.Pages

type Model = { Todos: Todo list; Input: string; home: Home.Model  }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo
    | HomeMsg of Home.Msg

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

let init () : Model * Cmd<Msg> =
    let home, homeCmd = Home.init ()
    let model = { Todos = []; Input = ""; home = home }

    let baseCmd = Cmd.OfAsync.perform todosApi.getTodos () GotTodos
    let cmd = Cmd.batch [
        Cmd.map HomeMsg homeCmd;
        baseCmd
    ]

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | GotTodos todos -> { model with Todos = todos }, Cmd.none
    | SetInput value -> { model with Input = value }, Cmd.none
    | AddTodo ->
        let todo = Todo.create model.Input

        let cmd = Cmd.OfAsync.perform todosApi.addTodo todo AddedTodo

        { model with Input = "" }, cmd
    | AddedTodo todo -> { model with Todos = model.Todos @ [ todo ] }, Cmd.none
    | HomeMsg msg ->
        let res, cmd = Home.update msg model.home
        { model with home = res }, Cmd.map HomeMsg cmd
//
// open Feliz
// open Feliz.Bulma
//
// let navBrand =
//     Bulma.navbarBrand.div [
//         Bulma.navbarItem.a [
//             prop.href "https://safe-stack.github.io/"
//             navbarItem.isActive
//             prop.children [
//                 Html.img [
//                     prop.src "/favicon.png"
//                     prop.alt "Logo"
//                 ]
//             ]
//         ]
//     ]
//
// let containerBox (model: Model) (dispatch: Msg -> unit) =
//     Bulma.box [
//         Bulma.content [
//             Html.ol [
//                 for todo in model.Todos do
//                     Html.li [ prop.text todo.Description ]
//             ]
//         ]
//         Bulma.field.div [
//             field.isGrouped
//             prop.children [
//                 Bulma.control.p [
//                     control.isExpanded
//                     prop.children [
//                         Bulma.input.text [
//                             prop.value model.Input
//                             prop.placeholder "What needs to be done?"
//                             prop.onChange (fun x -> SetInput x |> dispatch)
//                         ]
//                     ]
//                 ]
//                 Bulma.control.p [
//                     Bulma.button.a [
//                         color.isPrimary
//                         prop.disabled (Todo.isValid model.Input |> not)
//                         prop.onClick (fun _ -> dispatch AddTodo)
//                         prop.text "Add"
//                     ]
//                 ]
//             ]
//         ]
//     ]

open Fable.Builders.AntDesign

JsInterop.importAll "${outDir}/../styles/styles.less"

let view (model: Model) (dispatch: Msg -> unit) =
    Content {
        PageHeader {
            title (str "Login ")
            subTitle (str "Please log-in to enter.")
        }
    }

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