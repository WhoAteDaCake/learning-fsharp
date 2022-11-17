module Client.Routing

module Routes =
    [<Literal>]
    let Bookmarks = "bookmarks"

    [<Literal>]
    let Home = "home"

    [<Literal>]
    let NotFound = "404"

[<RequireQualifiedAccess>]
type Url =
    | Home
    | Bookmarks
    | NotFound

let parseUrl =
    function
    | [] -> Url.Home
    | [ Routes.Bookmarks ] -> Url.Home
    | _ -> Url.NotFound

let urlToPath =
    function
    | Url.Home -> "/" + Routes.Home
    | Url.Bookmarks -> "/" + Routes.Bookmarks
    | Url.NotFound -> "/" + Routes.NotFound

let topLevelRoutes =
    [ (Url.Home, "Home")
      (Url.Bookmarks, "Bookmarks")
      (Url.NotFound, "NotFound") ]
