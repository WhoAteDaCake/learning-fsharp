module Client.Pages.Bookmarks

open Client
open Client.Deferred
open Client.Pages.Home
open Elmish
open Fable.React
open Feliz
open Feliz.AntdReact
open Fable.Core
open Feliz.Router

type IndexQuery = {
    Selected: string option
}

[<RequireQualifiedAccess>]
type Url =
| Index of IndexQuery

let parseUrl = function
| [Route.Query ["selected", selected ]] ->
    Url.Index ({ Selected = Some selected })
| _ -> Url.Index ({ Selected = None })


type Leaf = {
    Id: string
    Title: string
    ParentId: string option
    Icon: string
    Url: string;
}
type Branch = {
    Id: string
    ParentId: string option
    Title: string
    Children: Tree list
}
and Tree =
| TBranch of Branch
| TLeaf of Leaf

type Model =
    {
        Bookmarks: Deferred<Result<Tree, string>>
        Selected: string option
    }

type Msg =
    | Load of AsyncOperationStatus<Result<Tree, string>>
    | Select of string

let fakeData: Tree = TBranch {
    Id = "0";
    Title = "Bookmarks bar"
    ParentId = None
    Children = [
        TBranch {
            Id = "1"
            Title = "Searches"
            ParentId = Some "0"
            Children = [
                TLeaf {
                    Id = "2"
                    ParentId = Some "1"
                    Title = "Previous google search"
                    Icon = "test"
                    Url = "http://google.com"
                }
            ]
        }
        TBranch {
            Id = "4"
            Title = "Personal"
            ParentId = Some "0"
            Children = [
                TLeaf {
                    Id = "5"
                    ParentId = Some "4"
                    Title = "Previous google search"
                    Icon = "test"
                    Url = "http://google.com"
                }
            ]
        }
    ]
}

let fakeAsyncLoad () =
    async { return AsyncOperationStatus.Finished(Result.Ok fakeData) }

let init (url: Url) : Model * Cmd<Msg> =
    let model = { Bookmarks = HasNotStartedYet; Selected = None }

    let cmd =
        Cmd.batch [
            Cmd.OfAsync.perform fakeAsyncLoad () Load
            Cmd.ofMsg (Load Started)
        ]

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Select id -> { model with Selected = Some id }, Cmd.none
    | Load Started -> { model with Bookmarks = InProgress }, Cmd.none
    | Load (Finished result) -> { model with Bookmarks = Resolved result }, Cmd.none

let rec renderBranches = function
| TBranch branch ->
    let output: TreeData = {
        title = U2.Case1 (Html.text branch.Title)
        key = U2.Case1 branch.Id
        icon = None
        disabled = false
        selectable = true
        children =
            branch.Children
            |> (List.map renderBranches)
            |> (List.fold (fun acc el ->
                match el with
                | Some(n) -> n :: acc
                | None -> acc) [])
            |> List.rev
            |> Array.ofSeq
    }
    Some output
| TLeaf _ ->
    None

let view (model: Model) (dispatch: Msg -> unit) =
    let bookmarks =
        match model.Bookmarks with
        | Resolved (Ok result) ->
            match renderBranches result with
            | Some data ->
                Antd.tree [
                    tree.treeData [data]
                    tree.defaultExpandAll true
                ]
            | None -> Html.text "Could not render the tree due to invalid configuration"

        | Resolved (Error error) ->
            Html.text $"Failed: {error}"
        | HasNotStartedYet
        | InProgress -> Html.text "Loading"

    Antd.row [
        row.className "mt-1"
        row.children [
            Antd.col [
                col.span 8
                col.children [
                    bookmarks
                ]
            ]
            Antd.col [
                col.span 16
                col.children [
                    Html.text "Body here"
                ]
            ]
        ]
    ]
