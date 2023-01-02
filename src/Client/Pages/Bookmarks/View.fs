module Client.Pages.Bookmarks.View

open Domain
open Client.Deferred
open Feliz
open Feliz.AntdReact
open Fable.Core

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
