module Client.Pages.Import.View

open Client.Pages.Import.Domain
open Feliz

let view (model: Model) (dispatch: Msg -> unit) =
    Html.div [ prop.text "Hello" ]