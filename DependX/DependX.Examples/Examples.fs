module DependX.Autofac.Examples

open Autofac
open DependX
open DependX.Autofac

type IService = abstract SayHello : unit -> unit
type Service() = interface IService with member _.SayHello() = printfn "Hello"

let depsOld =
    let containerBuilder = ContainerBuilder()
    
    containerBuilder.RegisterType<Service>()
        .AsSelf()
        .SingleInstance()
        .Named("Kek")
        .WithParameter("abc", 123)
        .WithParameter("abc2", fun _ -> fun () -> 123) |> ignore
    
    containerBuilder.RegisterType<Service>()
        .As<IService>() |> ignore

    containerBuilder

let deps () = dependencies [
    configure selfContract<Service> {
        lifetime singleton
        named "Kek"
        param "abc" (fromInstance 123)
        param "abc2" (fromFactory <| fun () -> 123)
        param "abc3" fromDep
    }
    contract<IService, Service>
]

let module' = makeModule deps ()