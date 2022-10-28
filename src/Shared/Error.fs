module Shared.Error

type T =
    { Title: string
      Detail: string
      Origin: Option<string> }

let make title detail =
    { Title = title
      Detail = detail
      Origin = None }
