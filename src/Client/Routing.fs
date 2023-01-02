module Client.Routing

open Client
open Client.Remote
open Client.Pages

[<RequireQualifiedAccess>]
type Url =
    | Home
    | Bookmarks of Bookmarks.Domain.Url
    | NotFound

let parseUrl =
    function
    | [] -> Url.Home
    | Routes.Bookmarks::parts -> Url.Bookmarks (Bookmarks.State.parseUrl parts)
    | _ -> Url.NotFound

let urlToString =
    function
    | Url.Home -> Routes.Home
    | Url.Bookmarks _ -> Routes.Bookmarks
    | Url.NotFound -> Routes.NotFound

let topLevelRoutes =
    [ (Routes.Home, "Home")
      (Routes.Bookmarks, "Bookmarks") ]