module Client.Pages.Home

open Client
open Client.Deferred
open Elmish
open Fable.React
open Feliz.Antd

type Graph =
    | Nodes of
        {| Id: int
           Name: string
           Graph: Graph |} list
    | Leaf of {| Id: int; Name: string; Url: string |}

type Model =
    { Bookmarks: Deferred<Result<Graph, string>> }

type Msg =
    | Append of Graph
    | Load of AsyncOperationStatus<Result<Graph, string>>

let fakeGraph =
    Nodes [ {| Id = 1
               Name = "test"
               Graph =
                Leaf
                    {| Id = 1
                       Name = "my_url"
                       Url = "http://google.com" |} |} ]

let fakeAsyncLoad () =
    async { return AsyncOperationStatus.Finished(Result.Ok fakeGraph) }

let init () : Model * Cmd<Msg> =
    let model = { Bookmarks = HasNotStartedYet }

    let cmd =
        Cmd.batch [ Cmd.OfAsync.perform fakeAsyncLoad () Load
                    Cmd.ofMsg (Load Started) ]

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Append _ -> model, Cmd.none
    //
    | Load Started -> { model with Bookmarks = InProgress }, Cmd.none
    | Load (Finished result) -> { model with Bookmarks = Resolved result }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    Antd.button [ button.disabled false
                  button.children [ str "Click" ] ]
