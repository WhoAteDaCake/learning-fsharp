module Server.Remote.Bookmarks

open Shared.Bookmark
open FsToolkit.ErrorHandling

let mutable store =
    Nodes [ {| Id = 1
               Name = "test"
               Tree =
                Leaf
                    {| Id = 1
                       Name = "my_url"
                       Url = "http://google.com" |} |} ]

let getBookmarks () = asyncResult { return store }

let api: IBookmarksApi =
    { getBookmarks = getBookmarks }
