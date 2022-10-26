module Client.Pages.Home

open Elmish

type Graph =
    | Nodes of GraphNode list
    | Leaf of string

and GraphNode = {
    id: string
    name: string
    graph: Graph
}

type Model = {
    bookmarks: Graph
}

type Msg =
    | AppendNode of string list * GraphNode

let init () : Model * Cmd<Msg> =
    let model = { bookmarks = Leaf("hello") }
    let cmd = Cmd.none

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | AppendNode _ -> model, Cmd.none