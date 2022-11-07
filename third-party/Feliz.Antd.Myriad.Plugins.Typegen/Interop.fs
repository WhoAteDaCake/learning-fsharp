namespace Myriad.Plugins

open System

module Interop =
    let attr (key: string) (value: obj) = unbox (key, value)

    type inlined = obj

    type Extends([<ParamArray>] classes: Type array) =
        member this.classes = classes
