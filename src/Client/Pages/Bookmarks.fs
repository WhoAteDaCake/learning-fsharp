module Client.Pages.Bookmarks

open Client
open Client.Deferred
open Client.Pages.Home
open Elmish
open Fable.React
open Feliz
open Feliz.AntdReact
open Fable.Core

type Model =
    { Bookmarks: Deferred<Result<TreeData, string>> }

type Msg =
    | Append of TreeData
    | Load of AsyncOperationStatus<Result<TreeData, string>>

let fakeGraph: TreeData =
    { title = U2.Case1 (Html.text "parent 1")
      key = U2.Case1 "0-0"
      icon = None
      disabled = false
      selectable = true
      children =
        [| { title = U2.Case1 (Html.text "parent 1-0")
             key = U2.Case1 "0-0-0"
             disabled = true
             icon = None
             selectable = true
             children =
               [| { title = U2.Case1 (Html.text "leaf")
                    key = U2.Case1 "0-0-0-0"
                    icon = None
                    disabled = false
                    selectable = true
                    children = [||] } |] } |] }

let fakeAsyncLoad () =
    async { return AsyncOperationStatus.Finished(Result.Ok fakeGraph) }

let init () : Model * Cmd<Msg> =
    let model = { Bookmarks = HasNotStartedYet }

    let cmd =
        Cmd.batch [
            Cmd.OfAsync.perform fakeAsyncLoad () Load
            Cmd.ofMsg (Load Started)
        ]

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Append _ -> model, Cmd.none
    | Load Started -> { model with Bookmarks = InProgress }, Cmd.none
    | Load (Finished result) -> { model with Bookmarks = Resolved result }, Cmd.none
    | _ -> model, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    let bookmarks =
        match model.Bookmarks with
        | Resolved (Ok result) -> Antd.tree [tree.treeData [result]]
        | _ -> Html.text "Failed to load"

    Html.div [
        prop.classes [ "mt-1" ]
        prop.children [ bookmarks ]
    ]
