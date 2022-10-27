module Index

open Client
open Elmish
open Fable.Core
open Fable.Builders.AntDesign
open Shared
open Client.Pages

type Model = { Todos: Todo list; Input: string; home: Home.Model  }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo
    | Home of Home.Msg

let init () : Model * Cmd<Msg> =
    let home, homeCmd = Home.init ()
    let model = { Todos = []; Input = ""; home = home }

    // let baseCmd = Cmd.OfAsync.perform todosApi.getTodos () GotTodos
    let cmd = Cmd.batch [
        Cmd.map Home homeCmd;
        // baseCmd
    ]

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | GotTodos todos -> { model with Todos = todos }, Cmd.none
    | SetInput value -> { model with Input = value }, Cmd.none
    | AddTodo ->
        let todo = Todo.create model.Input

        let cmd = Cmd.OfAsync.perform Api.todos.addTodo todo AddedTodo

        { model with Input = "" }, cmd
    | AddedTodo todo -> { model with Todos = model.Todos @ [ todo ] }, Cmd.none
    | Home msg ->
        let res, cmd = Home.update msg model.home
        { model with home = res }, Cmd.map Home cmd

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