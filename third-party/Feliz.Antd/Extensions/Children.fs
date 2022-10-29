namespace Feliz.AntdReact

open Feliz
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type Children<'C> =
    static member inline children(elements: ReactElement list) = unbox<'C> (prop.children elements)
