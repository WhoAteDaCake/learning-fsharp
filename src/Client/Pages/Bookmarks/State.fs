module Client.Pages.Bookmarks.State

open Domain
open Client
open Client.Deferred
open Elmish
open Feliz.Router


let parseUrl = function
| [Route.Query [SearchParams.Selected, selected ]] ->
    Url.Index ({ Selected = Some selected })
| _ -> Url.Index ({ Selected = None })


let onUrlChange = function
| Url.Index newUrl, Url.Index oldUrl ->
    if newUrl.Selected = oldUrl.Selected then
        Intent.NoAction
    else
        Intent.Update (UrlMsg (UrlSelect newUrl.Selected))
| _ -> Intent.NoAction

let fakeData: Tree = TBranch {
    Id = "0";
    Title = "Bookmarks bar"
    ParentId = None
    Children = [
        TBranch {
            Id = "1"
            Title = "Searches"
            ParentId = Some "0"
            Children = [
                TLeaf {
                    Id = "2"
                    ParentId = Some "1"
                    Title = "Previous google search"
                    Icon = "test"
                    Url = "http://google.com"
                }
            ]
        }
        TBranch {
            Id = "4"
            Title = "Personal"
            ParentId = Some "0"
            Children = [
                TLeaf {
                    Id = "5"
                    ParentId = Some "4"
                    Title = "Previous google search"
                    Icon = "test"
                    Url = "http://google.com"
                }
            ]
        }
    ]
}

let fakeAsyncLoad () =
    async { return AsyncOperationStatus.Finished(Result.Ok fakeData) }

let init (url: Url) : Model * Cmd<Msg> =
    // TODO: load selected from url
    let model = { Bookmarks = HasNotStartedYet; Selected = None }

    let cmd =
        Cmd.batch [
            Cmd.OfAsync.perform fakeAsyncLoad () Load
            Cmd.ofMsg (Load Started)
        ]

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Select id -> model, Cmd.navigate(Routes.Bookmarks, ["selected", id])
    | Load Started -> { model with Bookmarks = InProgress }, Cmd.none
    | Load (Finished result) -> { model with Bookmarks = Resolved result }, Cmd.none
    | UrlMsg msg ->
        match msg with
        | UrlSelect selected ->
            { model with Selected = selected }, Cmd.none
        | _ -> model, Cmd.none
    | _ -> model, Cmd.none
