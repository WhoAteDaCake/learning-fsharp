module Client.Routing

[<RequireQualifiedAccess>]
type Url =
    | Home
    | NotFound

let parseUrl =
    function
    | [] -> Url.Home
    | _ -> Url.NotFound
