module Client.Pages.Bookmarks.View

open Domain
open Client.Deferred
open Feliz
open Feliz.AntdReact
open Fable.Core

let firstId = function
| TBranch x -> x.Id
| TLeaf x -> x.Id

let rec trimTree (rootId: string) = function
| TBranch branch ->
    if branch.Id = rootId then
        Some (TBranch branch)
    else
        List.tryFind (fun tree -> Option.isSome (trimTree rootId tree)) branch.Children
| TLeaf leaf ->
    if rootId = leaf.Id then
        Some (TLeaf leaf)
    else
        None

let rec treeRenderer (leafRender: Leaf -> TreeData list) childrenRender =  function
| TBranch branch ->
    let output: TreeData = {
        title = U2.Case1 (Html.text branch.Title)
        key = U2.Case1 branch.Id
        icon = None
        disabled = false
        selectable = true
        children = childrenRender leafRender branch.Children
    }
    [output]
| TLeaf leaf ->
    leafRender leaf

let leafRender (leaf: Leaf) =
    let output: TreeData = {
        title = U2.Case1 (Html.text leaf.Title)
        key = U2.Case1 leaf.Id
        icon = None
        disabled = false
        selectable = true
        children = Array.empty
    }
    [output]

let rec nestedRender leafRender children =
    children
    |> (List.map (treeRenderer leafRender nestedRender))
    |> List.fold List.append []
    |> Array.ofSeq

let previewRender = treeRenderer (fun _ -> []) nestedRender

// Skips rendering of the parent branch
let bodyRender = function
| TBranch branch ->
    branch.Children
    |> List.map (treeRenderer leafRender (fun _ _ -> Array.empty))
    |> List.fold List.append []
| TLeaf leaf ->
    leafRender leaf

let view (model: Model) (dispatch: Msg -> unit) =
    let content =
        match model.Bookmarks with
        | Resolved (Ok result) ->
            let selectedId = Option.defaultValue (firstId result) model.Selected
            let bookmarks =
                Antd.tree [
                    tree.treeData (previewRender result)
                    tree.defaultExpandAll true
                    tree.selectedKeys [selectedId]
                    tree.onSelect (fun keys event -> dispatch (Select (keys |> List.ofArray)))
                ]
            let body =
                let value = trimTree selectedId result |> Option.map bodyRender
                match value with
                | Some data ->
                    Antd.tree [
                        tree.treeData data
                        tree.defaultExpandAll true
                        tree.onSelect (fun keys event -> dispatch (Select (keys |> List.ofArray)))
                    ]
                | None -> Html.none
            [
                Antd.col [
                    col.span 8
                    col.children [
                        bookmarks
                    ]
                ]
                Antd.col [
                    col.span 16
                    col.children [
                        body
                    ]
                ]
            ]

        | Resolved (Error error) ->
            [Html.text $"Failed: {error}"]
        | HasNotStartedYet
        | InProgress -> [Html.text "Loading"]

    Antd.row [
        row.className "mt-1"
        row.children content
    ]
