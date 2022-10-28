module Server.Remote.Stats

open Shared.Remote.Stats
open FsToolkit.ErrorHandling


let getStats () =
    asyncResult { return { Bookmarks = { Current = 10; Limit = 100 } } }

let api: IStatsApi = { getStats = getStats }
