module Client.Remote.Bookmarks

open Fable.Remoting.Client
open Shared
open Shared.Remote

let bookmarksApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<Bookmark.IBookmarksApi>
