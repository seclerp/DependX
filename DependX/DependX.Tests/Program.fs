open Expecto.Tests
open Expecto.TestResults

[<EntryPoint>]
let main argv =
    let writeResults = writeNUnitSummary ("TestResults.xml", "DependX.Tests")
    let config = defaultConfig.appendSummaryHandler writeResults
    runTestsInAssembly config argv