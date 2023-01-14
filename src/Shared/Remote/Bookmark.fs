module Shared.Remote.Bookmark

open Shared

type BookmarkTree =
    | Nodes of
        {| Id: int
           Name: string
           Tree: BookmarkTree |} list
    | Leaf of {| Id: int; Name: string; Url: string |}

type IBookmarksApi =
    {
        getBookmarks: unit -> Async<Result<BookmarkTree, Error.T>>
        uploadBookmarks: byte[] -> Async<Result<string, Error.T>>
    }
