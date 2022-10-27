module Client.Api

open Fable.Remoting.Client
open Shared

let todos =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>
