namespace Myriad.Plugins

open System

[<RequireQualifiedAccess>]
module Generator =
    type ComponentAttribute() =
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


    // Specifies the namespace where the components will be located
    type ModuleRootAttribute() =
        inherit Attribute()


    // Specifies the namespace where the components will be located
    type LibraryRootAttribute() =
        inherit Attribute()
