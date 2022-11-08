namespace Myriad.Plugins

open System

[<RequireQualifiedAccess>]
module Generator =
    type ComponentAttribute(name: string) =
        inherit Attribute()

    type ExtendsMethodsAttribute([<ParamArray>] toInherit: Type array) =
        inherit Attribute()

    /// Mark a class as method only, this way we can re-use the code
    /// These classes will be removed in the final produced file
    type MethodsAttribute() =
        inherit Attribute()

    /// A marker to include an additional type
    type IncludedAttribute() =
        inherit Attribute()
