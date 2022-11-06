module Myriad.Typegen.Tests.Base

let inline interopAttr<'T> (key: string) (value: obj) : 'T = unbox (key, value)

type prop =
    static member inline children(elements: string list) =
        "test"

type withChildren<'T>() =
    member this.children(elements: string list) =
        unbox<'T> (prop.children elements)

type withClass<'T> =
    member this.className(value: string) = interopAttr "className" value

    member this.className(names: seq<string>) =
        interopAttr "className" (String.concat " " names)

type CustomType =
| Var1
| Var2

type withCustom<'T> =
    member this.customAction(myType: CustomType) =
        interopAttr "customProp" myType