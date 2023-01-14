module Client.Pages.Bookmarks.View

open Domain
open Client.Deferred
open Fable.Core
open Feliz
open Feliz.AntdIconsReact
open Feliz.AntdReact

let firstId = function
| TBranch x -> x.Id
| TLeaf x -> x.Id

let rec trimTree (rootId: string) = function
| TBranch branch ->
    if branch.Id = rootId then
        Some (TBranch branch)
    else
        List.tryPick (trimTree rootId) branch.Children
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
        icon = Some (U2.Case1 (
            Html.div [
                prop.className "flex w-full h-full justify-center items-center"
                prop.children [
                    AntdIcons.folderOutlined [
                        folderOutlined.style [
                            style.fontSize 18
                        ]
                    ]
                ]
            ]))
        disabled = false
        selectable = true
        children = childrenRender leafRender branch.Children
    }
    [output]
| TLeaf leaf ->
    leafRender leaf

let favicon (url: string) =
    let cleanUrl = url.Split("?")[0]
    let src =
        $"https://s2.googleusercontent.com/s2/favicons?domain_url={JS.encodeURIComponent cleanUrl}"
    Html.div [
        prop.className "flex w-full h-full justify-center items-center"
        prop.children [
            Html.img [ prop.src src ]
        ]
    ]

let leafRender (leaf: Leaf) =
    let output: TreeData = {
        title = U2.Case1 (Html.text leaf.Title)
        key = U2.Case1 leaf.Id
        // chrome://favicon2/?size=16&scaleFactor=1x&pageUrl=https%3A%2F%2Fhn.algolia.com%2F&allowGoogleServerFallback=0
        icon = Some (U2.Case1 (favicon leaf.Url))
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

let rec findTreePart (partId: string) = function
| TLeaf leaf ->
    if leaf.Id = partId then
        Some (TLeaf leaf)
    else
        None
| TBranch branch ->
    if branch.Id = partId then
        Some (TBranch branch)
    else
        List.tryPick (findTreePart partId) branch.Children

let view (model: Model) (dispatch: Msg -> unit) =
    let content =
        match model.Bookmarks with
        | Resolved (Ok result) ->
            let selectedId = Option.defaultValue (firstId result) model.Selected
            let bookmarks =
                Antd.tree [
                    tree.treeData (previewRender result)
                    tree.defaultExpandAll true
                    tree.showIcon true
                    tree.selectedKeys [selectedId]
                    tree.onSelect (fun keys event ->
                        Browser.Dom.console.log keys
                        dispatch (Select (keys[0])))
                ]
            let body =
                match trimTree selectedId result |> Option.map bodyRender with
                | Some data ->
                    Antd.tree [
                        tree.showIcon true
                        tree.treeData data
                        tree.defaultExpandAll true
                        tree.onSelect (fun keys event ->
                            match findTreePart keys[0] result with
                            | Some (TBranch branch) -> dispatch (Select branch.Id)
                            | Some (TLeaf leaf) -> dispatch (Navigate leaf.Url)
                            | _ -> ()
                            // dispatch (Select (keys |> List.ofArray))
                        )
                    ]
                | None -> Html.none
            [
                Antd.col [
                    col.span 8
                    col.children [
                        Html.div [
                            prop.className "p-2"
                            prop.children bookmarks
                        ]
                    ]
                ]
                Antd.col [
                    col.span 16
                    col.children [
                        Html.div [
                            prop.className "p-2"
                            prop.children body
                        ]
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
