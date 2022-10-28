module Client.Routing

module Routes =
    let bookmarks = "bookmarks"
    let home = "home"

[<RequireQualifiedAccess>]
type Url =
    | Home
    | NotFound

let parseUrl =
    function
    | [] -> Url.Home
    | _ -> Url.NotFound

let topLevelRoutes = [
    (Url.Home, "/home", "Home");
    (Url.Home, "/home", "Home");
]