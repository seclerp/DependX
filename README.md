<h2 align="center">DependX</h2>

<p align="center"> <a href="https://ci.appveyor.com/project/AndrewRublyov/dependx"> <img src="https://ci.appveyor.com/api/projects/status/7ep04nheqfav80on?svg=true"></a></p>

<p align="center">Library that provides F# DSL for dependency injection registration for your favorite IoC provider</p>

---

:warning: **Project in active development now and currently only supports Autofac container.**

DependX allows you to change this:

```fsharp
let deps =
    let containerBuilder = ContainerBuilder()
    
    containerBuilder.RegisterType<Service>()
        .AsSelf()
        .SingleInstance()
        .Named("Some name")
        .WithParameter("abc", 123)
        .WithParameter("abc2", fun _ -> 123) |> ignore
    
    containerBuilder.RegisterType<Service>()
        .As<IService>() |> ignore

    containerBuilder
```

Into this:
```fsharp
let deps () = dependencies [
    configure selfContract<Service> {
        lifetime singleton
        named "Some name"
        param "abc" (fromInstance 123)
        param "abc2" (fromFactory <| fun () -> 123)
    }
    contract<IService, Service>
]
```

Also there are some helper functions to nicely get started into your .NET Core or ASP.NET Core environment

Core library built on top of **.NET Startard 2.0**

ASP.NET Core extensions made for **ASP.NET Core 2.2+**

Autofac libraries currently built on top of **Autofac 4.9.4** and **Autofac.Extensions.DependencyInjection 5.0.1**

### Getting started with ASP.NET Core and Autofac 

Before we start, let's assume that there are some dependencies:

```fsharp
// ExampleServices.fs
module SomeProject.ExampleServices

type ICustomLogger =
    abstract LogInfo : string -> unit

type IMessageService = abstract SayHello : unit -> unit

type CustomLogger() =
    interface ICustomLogger with
        member _.LogInfo(str) = printfn "%s" str

type MessageService(logger: ICustomLogger) =
    interface IHelloService with
        member _.Say(message) = logger.LogInfo message

type StartupService(messageService: IMessageService) =
    interface IHostedService with
        member _.StartAsync(token: CancellationToken) =
            messageService.Say("Hello")
            Task.CompletedTask

        member _.StopAsync(_: CancellationToken) =
            Task.CompletedTask
```

Here we defined dummy logger, `IMessageService` that uses that logger and `IHostedService` that on start uses our `IMessageService` for writing some stuff.

Then, lets define our dependencies module:
```fsharp
// Dependencies.fs
module SomeProject.Dependencies

open DependX
open DependX.Autofac
open DependX.AspNetCore
open ExampleServices

let register = dependencies [
    // ...
]
```

`open DependX` - for core types and logic

`open DependX.Autofac` - for autofac-specific interpreter

`open DependX.AspNetCore` - for ASP.NET Core related stuff, in our case - `hostedService` 

`let register = dependencies [...]` - this is our initial dependency composition. We will declare dependencies inside that block.

Lets add our newly created dependencies:

```fsharp
// Dependencies.fs
// ...
let register = dependencies [
    configure contract<ICustomLogger, CustomLogger> {
        lifetime singleton
    }
    contract<ISomeService, SomeService>
    hostedService<StartupService>
]
```

In that case we registered `CustomLogger` as singleton and `SomeService` as transient (by default). We also added StartupService as IHostedService just like regular `.AddHostedService` in ASP.NET Core DI.

That's it. We only need to provide information how dependency need to be registered, **there are no direct usage on Autofac related stuff like ContainerBuilder**.

After that we need to use our register function in `Startup.fs` class:
```fsharp
// Startup.fs
// ...
open Dependencies
// ...
type Startup(...) =
    member this.ConfigureContainer(builder: ContainerBuilder) =
        register builder |> ignore
```

And (if you don't added it yet) configure Autofac in `Program.fs`:

```fsharp
// ...
open DependX.AspNetCore.Autofac
// ...
    Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(AutofacServiceProviderFactory()) // ASP.NET Core 3+ related stuff
        .ConfigureWebHostDefaults(fun webBuilder ->
            webBuilder.UseStartup<Startup>() |> ignore
            webBuilder |> configureAutofacDefaults
        )
```

After project start you will see
```
Hello
```
in the console window. More information about what functions you could use for registratino can be found in **[documentation](DOCUMENTATION.md)**

### Solution structure

#### Core
- `DependX.Core` - core library with main logic for creating abstract dependencies

#### Autofac
- `DependX.Autofac` - provides interpret functions that produces Autofac IoC container from abstract dependencies

#### ASP.NET Core extensions
- `DependX.AspNetCore` - provides helper functions to support injection of ASP.NET Core related services (like IHostedService) 
- `DependX.AspNetCore.Autofac` - provides helper functions to configure and use Autofac in ASP.NET Core

#### Tests
- `DependX.Tests` - contains unit tests for core logic

### Documentation

You could found full documentation **[here](DOCUMENTATION.md)**

### Contributing

Feel free to submit issues and PR's. If you have any questions contact me via **[email](mailto:andrewrublyov99@gmail.com)** or **[telegram](https://t.me/FreeParticle)**.
