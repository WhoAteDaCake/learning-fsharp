module Client.Pages.Bookmarks.State

open Client.Pages.Bookmarks.Domain
open Domain
open Client
open Client.Deferred
open Elmish
open Fable.Core
open Feliz.Router


let parseUrl = function
| [Route.Query [SearchParams.Selected, selected ]] ->
    Url.Index ({ Selected = Some selected })
| _ -> Url.Index ({ Selected = None })


let onUrlChange = function
| Url.Index newUrl, Url.Index oldUrl ->
    // let _ = Browser.Dom.console.log "START: onUrlChange"
    // let _ = Browser.Dom.console.log [newUrl, oldUrl]
    // let _ = Browser.Dom.console.log "END: onUrlChange"
    if newUrl.Selected = oldUrl.Selected then
        Intent.NoAction
    else
        Intent.Update (UrlMsg (UrlSelect newUrl.Selected))

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
                    Title = "Searches google search"
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
                 TBranch {
                    Id = "6"
                    Title = "Houses"
                    ParentId = Some "4"
                    Children = [
                        TLeaf {
                            Id = "7"
                            ParentId = Some "6"
                            Title = "House number 1"
                            Icon = "test"
                            Url = "https://www.google.com/search?q=js+get+favicon&oq=js+get+favicon&aqs=chrome..69i57.4214j0j1&sourceid=chrome&ie=UTF-8"
                        }
                    ]
                };
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
    let selected =
        match url with
        | Url.Index x -> x.Selected

    let model = { Bookmarks = HasNotStartedYet; Selected = selected }

    let cmd =
        Cmd.batch [
            Cmd.OfAsync.perform fakeAsyncLoad () Load
            Cmd.ofMsg (Load Started)
        ]

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    // let _ = Browser.Dom.console.log "START: update"
    // let _ = Browser.Dom.console.log [msg]
    // let _ = Browser.Dom.console.log "END: update"
    match msg with
    | Select id -> model, Cmd.navigatePath(Routes.Bookmarks, ["selected", id])
    | Load Started -> { model with Bookmarks = InProgress }, Cmd.none
    | Load (Finished result) -> { model with Bookmarks = Resolved result }, Cmd.none
    | UrlMsg msg ->
        match msg with
        | UrlSelect selected ->
            { model with Selected = selected }, Cmd.none
    | Navigate url ->
        let view = Browser.Dom.window.``open``(url, "_blank")
        let _ = view.focus ()
        model, Cmd.none
    | _ ->
        model, Cmd.none