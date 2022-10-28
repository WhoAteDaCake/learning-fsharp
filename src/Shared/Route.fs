module Shared.Route

let builder typeName methodName =
    sprintf "/api/%s/%s" typeName methodName
