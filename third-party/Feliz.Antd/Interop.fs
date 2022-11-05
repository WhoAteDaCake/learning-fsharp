namespace Feliz.AntdReact

open Fable.Core

[<Erase; RequireQualifiedAccess>]
module Interop =
    let inline mkButtonAttr (key: string) (value: obj) : IButtonProperty = unbox (key, value)
    let inline mkLayoutAttr (key: string) (value: obj) : ILayoutProperty = unbox (key, value)
    let inline mkLayoutHeaderAttr (key: string) (value: obj) : ILayoutHeaderProperty = unbox (key, value)
    let inline mkLayoutContentAttr (key: string) (value: obj) : IRowProperty = unbox (key, value)
    let inline mkRowAttr (key: string) (value: obj) : IRowProperty = unbox (key, value)
