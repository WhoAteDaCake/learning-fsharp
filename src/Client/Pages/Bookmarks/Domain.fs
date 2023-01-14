module Client.Pages.Bookmarks.Domain

open Client
open Client.Deferred
open Elmish
open Feliz.Router

module SearchParams =
    [<Literal>]
    let Selected = "selected"

type IndexQuery = {
    Selected: string option
}

[<RequireQualifiedAccess>]
type Url =
| Index of IndexQuery

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

type UrlMsg =
| UrlSelect of string option

type Msg =
| Load of AsyncOperationStatus<Result<Tree, string>>
| Select of string
| Navigate of string
| UrlMsg of UrlMsg

[<RequireQualifiedAccess>]
type Intent =
| NoAction
| Update of Msg