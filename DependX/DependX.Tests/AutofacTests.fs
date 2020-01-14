module DependX.Tests.AutofacTests

open Autofac
open DependX
open Expecto
open DependX.Autofac
open DependX.Tests.Common
open DependX.Tests.ExampleServices

exception ServiceNotRegistered of string

let ``Simple injection must be resolved correctly`` =
    test "Simple injection must be resolved correctly" {
        let deps () = dependencies [
            contract<IHelloService, HelloService>
    //        configure contract<IHelloService, HelloService> {
    //            named "ConfiguredHello"
    //        }
        ]

        let result =
            try
                let container = makeContainer deps ()
                match container.IsRegistered<IHelloService>() with
                | true -> Ok ()
                | false -> Error <| ServiceNotRegistered "Service isn't registered"
            with e ->
                Error e
        
        Expect.isOk result "There is an error"
    }

let ``Named injection must be resolved correctly`` =
    test "Named injection must be resolved correctly" {
        let name = "ConfiguredHello"
        let deps () = dependencies [
            configure contract<IHelloService, HelloService> {
                named name
            }
        ]

        let result =
            try
                let container = makeContainer deps ()
                match container.IsRegisteredWithName<IHelloService> name with
                | true -> Ok ()
                | false -> Error <| ServiceNotRegistered "Service isn't registered"
            with e ->
                Error e
        
        Expect.isOk result "There is an error"
    }

let ``Injection with parameter must be resolved correctly`` =
    test "Injection with parameter must be resolved correctly" {
        let logger: ICustomLogger = CustomLogger() :> _
        let deps () = dependencies [
            configure contract<IHelloService, HelloServiceWithParam> {
                param "logger" (fromInstance logger)
            }
        ]

        let result =
            try
                let container = makeContainer deps ()
                match container.IsRegistered<IHelloService>() with
                | true ->
                    let service = container.Resolve<IHelloService>()
                    service.SayHello()
                    Ok()
                | false -> Error <| ServiceNotRegistered "Service isn't registered"
            with e ->
                Error e
        
        Expect.isOk result "There is an error"
    }

let ``Injection with dependency on other dependency must be resolved correctly`` =
    test "Injection with dependency on other dependency must be resolved correctly" {
        let deps () = dependencies [
            contract<ICustomLogger, CustomLogger>
            contract<IHelloService, HelloServiceWithParam>
        ]

        let result =
            try
                let container = makeContainer deps ()
                match container.IsRegistered<IHelloService>() with
                | true ->
                    let service = container.Resolve<IHelloService>()
                    service.SayHello()
                    Ok()
                | false -> Error <| ServiceNotRegistered "Service isn't registered"
            with e ->
                Error e
        
        Expect.isOk result "There is an error"
    }

[<Tests>]
let tests = testList "DependX Autofac" [
    ``Simple injection must be resolved correctly``
    ``Named injection must be resolved correctly``
    ``Injection with parameter must be resolved correctly``
    ``Injection with dependency on other dependency must be resolved correctly``
]