module Client.Routing

open Client
open Client.Remote
open Client.Pages

[<RequireQualifiedAccess>]
type Url =
    | Home
    | Bookmarks of Bookmarks.Domain.Url
    | Import of Import.Domain.Url
    | NotFound

let parseUrl =
    function
    | [] -> Url.Home
    | Routes.Bookmarks::parts -> Url.Bookmarks (Bookmarks.State.parseUrl parts)
    | Routes.Import::parts -> Url.Import (Import.State.parseUrl parts)
    | _ -> Url.NotFound

let urlToString =
    function
    | Url.Home -> Routes.Home
    | Url.Bookmarks _ -> Routes.Bookmarks
    | Url.Import _ -> Routes.Import
    | Url.NotFound -> Routes.NotFound

let topLevelRoutes =
    [ (Routes.Home, "Home")
      (Routes.Bookmarks, "Bookmarks")
      (Routes.Import, "Import") ]