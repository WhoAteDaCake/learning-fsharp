module Client.Pages.Bookmarks

open Client
open Client.Deferred
open Client.Pages.Home
open Elmish
open Fable.React
open Feliz

type GraphLeaf =
    | Link of {| Id: int; Name: string; Url: string |}
    | Folder of {| Id: int; Name: string |}

type Graph =
    | Nodes of GraphNode list
    | Leaf of GraphLeaf
and GraphNode = {
    Id: int
    Name: string
    Graph: Graph
}

type Model =
    { Bookmarks: Deferred<Result<Graph, string>> }

type Msg =
    | Append of Graph
    | Load of AsyncOperationStatus<Result<Graph, string>>

let fakeGraph =
    Nodes [
        ({
           Id = 1
           Name = "MyFolder"
           Graph =
            Leaf(
                Link
                    {| Id = 1
                       Name = "my_url"
                       Url = "http://google.com" |}
            )})
    ]

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

let drawLeaf = function
| Link n -> Html.a [prop.text n.Name; prop.href n.Url; prop.target "_blank"]
| Folder n -> Html.div [prop.text n.Name]

let rec drawNode (node: GraphNode) =
    Html.div [
        prop.id node.Id
        prop.classes ["flex flex-col"]
        prop.children [
            Html.div [
                prop.children [
                    Html.span [prop.text node.Name]
                ]
            ]
            Html.div [
                prop.classes ["ml-2"]
                prop.children [drawGraph node.Graph]
            ]
        ]
    ]
and drawGraph = function
| Nodes ns ->
    Html.div [
        prop.children (List.map drawNode ns)
    ]
| Leaf lf -> drawLeaf lf


let view (model: Model) (dispatch: Msg -> unit) =
    let bookmarks =
        match model.Bookmarks with
        | Resolved (Ok result) -> drawGraph result
        | _ -> Html.text "Failed to load"

    Html.div [
        prop.classes ["mt-1"]
        prop.children [
            bookmarks
        ]
    ]
