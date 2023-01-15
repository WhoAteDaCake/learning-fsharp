module Server.Tests.Api.BookmarkTests

open System
open System.IO
open System.Runtime.CompilerServices
open Xunit
open FSharp.Data
open FsToolkit.ErrorHandling



type LinkMeta = {
    Name: string
    AddDate: DateTime
    Link: string
}
and FolderMeta = {
    Name: string
    AddDate: DateTime
    LastModified: DateTime
    Children: Bookmark list
}
and Bookmark =
| Folder of FolderMeta
| Link of LinkMeta

let tryParseInt s =
    try
        s |> int |> Some
    with :? FormatException ->
        None

[<Extension>]
/// Extension methods with operations on HTMLNode
type HtmlAttributeExtensions =

    /// Gets the name of the current attribute
    [<Extension>]
    static member FirstChild(n: HtmlNode) =
        match n.Elements () with
        | x::xs -> Ok x
        | _ -> Error $"Node {n} contains no children"


let dateField (field: string) (node: HtmlNode) =
    let attr = node.TryGetAttribute field
    match attr with
    | Some value ->
        match tryParseInt (value.Value ()) with
        | Some intValue ->
            Ok (DateTime.MinValue.AddSeconds intValue)
        | None -> Error $"Could not parse: {value} to int"
    | None -> Error $"Node ({node}) missing attribute: {field}"


let rec parseChildren (acc: Bookmark list) = function
| x::xs ->
    match parse x with
    | Ok n -> parseChildren (n :: acc) xs
    | Error e -> Error e
| [] -> Ok (List.rev acc)
and parseFolder (nameNode: HtmlNode) (childNodes: HtmlNode list) = result {
    let! added = dateField "ADD_DATE" nameNode
    let! lastModified = dateField "LAST_MODIFIED" nameNode
    let! children =
        match childNodes with
        | x::_ -> parseChildren [] (x.Elements ())
        | _ -> Ok []
    return Folder {
        Name = nameNode.InnerText ()
        AddDate = added
        LastModified = lastModified
        Children = children
    }
}
and parseLink (node: HtmlNode) = result {
    let! added = dateField "ADD_DATE" node
    let! link =
        match node.TryGetAttribute "href" with
        | Some href -> Ok (href.Value ())
        | None -> Error $"Missing href for: {node}"
    return Link {
        Name = node.InnerText ()
        AddDate = added
        Link = link
    }
}
and parse (node: HtmlNode) =
    match node.Name () with
    | "p" ->  Result.bind parse (node.FirstChild())
    | "dt" ->
        match node.Elements () with
        | x::xs ->
            match x.Name () with
            | "h3" -> parseFolder x xs
            | "a" -> parseLink x
            | _ -> Error $"Unexpected child note {x} of {node}"
        | [] -> Error $"Invalid structure, expected children for node: {node}"
    | el -> Error $"Unexpected node found: {el}"
let parseRoot (doc: HtmlDocument) = result {
    let! root =
        match (doc.Elements "dl") with
        | x::_ -> Ok x
        | _ -> Error "Failed to find first <dl> tag of page"
    let! children = parseChildren [] (root.Elements ())
    return Folder {
        Name = "ROOT"
        AddDate = DateTime.Now
        LastModified = DateTime.Now
        Children = children
    }
}

let cleanedDoc (content: string) =
    content.Replace("<p>", "").Replace("</p>", "")


[<Fact>]
let ``My test`` () =
    let content = File.ReadAllText("/home/toshiba/Documents/bookmarks_14_01_2023.html")
    let doc = HtmlDocument.Parse(cleanedDoc content)
    let structure = parseRoot doc
    ()