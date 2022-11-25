module Client.Pages.Bookmarks

open Client
open Client.Deferred
open Client.Pages.Home
open Elmish
open Fable.React
open Feliz
open Feliz.AntdReact
open Fable.Core

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
    { Bookmarks: Deferred<Result<Tree, string>> }

type Msg =
    | Append of Tree
    | Load of AsyncOperationStatus<Result<Tree, string>>

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
    ]
}

let fakeAsyncLoad () =
    async { return AsyncOperationStatus.Finished(Result.Ok fakeData) }

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

let rec renderTree = function
| TBranch branch ->
    let output: TreeData = {
        title = U2.Case1 (Html.text branch.Title)
        key = U2.Case1 branch.Id
        icon = None
        disabled = false
        selectable = true
        children =
            branch.Children
            |> (List.map renderTree)
            |> Array.ofSeq
    }
    output
| TLeaf leaf ->
    let output: TreeData = {
        title = U2.Case1 (Html.text leaf.Title)
        key = U2.Case1 leaf.Id
        icon = None
        disabled = false
        selectable = true
        children = [||]
    }
    output

let view (model: Model) (dispatch: Msg -> unit) =
    let bookmarks =
        match model.Bookmarks with
        | Resolved (Ok result) ->
            Antd.tree [
                tree.treeData [renderTree result]
                tree.defaultExpandAll true
            ]
        | _ -> Html.text "Failed to load"

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
