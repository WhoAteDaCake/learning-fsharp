module Feliz.Antd.Myriad.Plugins.Typegen.Standard

// TOOD: look at
// https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-list.html
// https://fsprojects.github.io/FSharpx.Collections/reference/fsharpx-collections-list.html

module List =
    let splitWhen<'a> (fn: 'a -> bool) ls =
        let rec loop acc = function
        | x :: xs ->
            if fn x then
                Some x, (List.rev acc) @ xs
            else
                loop (x :: acc) xs
        | [] -> None, List.rev acc
        loop [] ls