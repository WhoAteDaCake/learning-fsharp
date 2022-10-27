// For more information see https://aka.ms/fsharp-console-apps
open FSharp.Data

[<Literal>]
let ResolutionFolder = __SOURCE_DIRECTORY__

[<Literal>]
let exampleFile = "/../tmp/incident_gas_distribution_jan2010_present.txt"

type RawData = CsvProvider<exampleFile, "\t", ResolutionFolder=ResolutionFolder>

[<EntryPoint>]
let main args =
    let msft =
        RawData
            .Load(ResolutionFolder + exampleFile)
            .Cache()
    printfn "Hello from F#"
    0