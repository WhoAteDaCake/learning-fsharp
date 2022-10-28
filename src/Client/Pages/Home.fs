module Client.Pages.Home

open Client
open Client.Deferred
open Elmish
open Fable.React

type Graph =
    | Nodes of GraphNode list
    | Leaf of GraphLeaf

and GraphNode = { id: int; name: string; graph: Graph }
and GraphLeaf = { id: int; name: string; url: string }

type Model =
    { bookmarks: Deferred<Result<Graph, string>> }

type Msg =
    | AppendNode of string list * GraphNode
    | Load of Deferred.AsyncOperationStatus<Result<Graph, string>>

let fakeGraph =
    Nodes [ { id = 1
              name = "test"
              graph =
                Leaf
                    { id = 1
                      name = "my_url"
                      url = "http://google.com" } } ]

let fakeAsyncLoad () =
    async { return AsyncOperationStatus.Finished(Result.Ok fakeGraph) }

let init () : Model * Cmd<Msg> =
    let model = { bookmarks = HasNotStartedYet }

    let cmd =
        Cmd.batch [ Cmd.OfAsync.perform fakeAsyncLoad () Load
                    Cmd.ofMsg (Load Started) ]

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | AppendNode _ -> model, Cmd.none
    //
    | Load Started -> { model with bookmarks = InProgress }, Cmd.none
    | Load (Finished (result)) -> { model with bookmarks = Resolved result }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) = str "Home page here"