module Server.App

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared
open Server

let webApp =
    let bookmarksApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue Remote.Bookmarks.api
        |> Remoting.buildHttpHandler

    router { forward "" bookmarksApi }

let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

[<EntryPoint>]
let main _ =
    run app
    0
