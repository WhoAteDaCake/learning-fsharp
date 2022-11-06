module Myriad.Typegen.Tests

open Myriad.Typegen.Tests.Base
open Myriad.Plugins

type button<'T> =
    inherit withChildren<'T>
