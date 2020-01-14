module DependX.Tests.ExampleServices

open DependX.Tests.Common
open Microsoft.Extensions.Logging

type IHelloService = abstract SayHello : unit -> unit
type HelloService() = interface IHelloService with member _.SayHello() = printfn "Hello"

type HelloServiceWithParam(logger: ICustomLogger) =
    interface IHelloService with member _.SayHello() = logger.LogInfo "Hello"