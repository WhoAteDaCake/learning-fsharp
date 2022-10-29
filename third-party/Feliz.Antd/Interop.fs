namespace Feliz.Antd

open Fable.Core

[<Erase; RequireQualifiedAccess>]
module Interop =
    let inline mkButtonAttr (key: string) (value: obj) : IButtonProperty = unbox (key, value)
