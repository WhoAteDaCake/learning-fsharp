module Client.Pages.Import.State
open Client.Pages.Import.Domain
open Elmish

let parseUrl = function
| _ -> Url.Index

let init (url: Url) : Model * Cmd<Msg> =
    { Error = None }, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    model, Cmd.none