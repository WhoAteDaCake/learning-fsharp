module Client.Routing

open Client.Remote
open Client.Pages

[<RequireQualifiedAccess>]
module Routes =
    [<Literal>]
    let Bookmarks = "bookmarks"

    [<Literal>]
    let Home = ""

    [<Literal>]
    let NotFound = "404"

[<RequireQualifiedAccess>]
type Url =
    | Home
    | Bookmarks of Bookmarks.Url
    | NotFound

let parseUrl =
    function
    | [] -> Url.Home
    | Routes.Bookmarks::parts -> Url.Bookmarks (Bookmarks.parseUrl parts)
    | _ -> Url.NotFound

let urlToString =
    function
    | Url.Home -> Routes.Home
    | Url.Bookmarks _ -> Routes.Bookmarks
    | Url.NotFound -> Routes.NotFound

let topLevelRoutes =
    [ (Routes.Home, "Home")
      (Routes.Bookmarks, "Bookmarks") ]