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

### Getting started

Most of the magic located in those modules:
```fsharp
open DependX // For core types and logic
open DependX.Autofac // For autofac-specific interpreter
```
Next, lets define the dependency composition:
```fsharp
let deps () = dependencies [...]
```
`()` means the input that will be passed here from startup or from any other place where this composition will be used. For example, it may be `IConfiguration`:
```fsharp
let deps (config: IConfiguration) = dependencies [...]
```
So lets define the simple contract:

```fsharp
let deps () = dependencies [
   contract<IService, Service>
]
```
This example defines `Service` implementation with `IService` abstraction, that link called contract on DependX.
You could also use same type for abstraction:
```fsharp
let deps () = dependencies [
   selfContract<Service>
]
```
And it may be resolved using `Service` type.

**TODO: Examples of setup in ASP.NET Core**

### Projects structure

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
