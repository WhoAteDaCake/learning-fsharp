module Client.Pages.Import.Domain

[<RequireQualifiedAccess>]
type Url =
| Index


type Model = {
    Error: string option
}

type Msg =
| Upload