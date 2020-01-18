module DependX.Tests.Common

type ICustomLogger =
    abstract LogInfo : string -> unit

type CustomLogger() =
    interface ICustomLogger with
        member _.LogInfo(str) = printfn "%s" str