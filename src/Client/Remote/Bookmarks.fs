module Client.Remote.Bookmarks

open Fable.Remoting.Client
open Shared
open Shared.Remote

let api =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<Bookmark.IBookmarksApi>
