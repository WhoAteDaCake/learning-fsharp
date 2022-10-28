module Shared.Remote.Stats

open Shared

type BookmarksUsage = { Current: int; Limit: int }

type Stats = { Bookmarks: BookmarksUsage }

type IStatsApi =
    { getStats: unit -> Async<Result<Stats, Error.T>> }
