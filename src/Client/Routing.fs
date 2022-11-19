module Client.Routing

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
    | Bookmarks
    | NotFound

let parseUrl =
    function
    | [] -> Url.Home
    | [ Routes.Bookmarks ] -> Url.Bookmarks
    | _ -> Url.NotFound

let urlToString =
    function
    | Url.Home -> Routes.Home
    | Url.Bookmarks -> Routes.Bookmarks
    | Url.NotFound -> Routes.NotFound

let topLevelRoutes =
    [ (Url.Home, "Home")
      (Url.Bookmarks, "Bookmarks") ]
    |> List.map (fun (u, label) -> (u, urlToString u, label))
