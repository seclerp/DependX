## DependX
Library that provides F# DSL for dependency injection registration for your favorite IoC provider

:warning: **Currently project in active development and only supports Autofac container for now.**

DependX allows you to chang this:

```fsharp
let deps =
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
```

Into this:
```fsharp
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
```

Also there are some helper functions to nicely get started into your .NET Core or ASP.NET Core environment

Library built on top of **.NET Startard 2.0**

### Documentation

#### Getting started

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

#### Dependency configuration

Contracts can be customized using `configure` function.
It produces dependency builder that has custom operations that you could use to set name, parameters resolcing, lifetime, etc.

**`configure [contract provider] { ... }`** - defines configuration for given contract, There are two built-in contract providers:

- `contract<Abstraction, Implementation>`
- `selfContract<Implementation>`

**`lifetime [Lifetime]`** - sets lifetime for current dependency. Built-in helpers:

- `singleton` - instance per runtime
- `transient` - instance per call (DEFAULT)
- `scoped` - instance per scope

**`param [name] [resolve strategy]`** - sets how concrete parameter need to be resolved for constructor. Currently only named way to setup parameter is supported. Resolve strategies:

- `fromInstance [object]` - resolves parameter using provided object
- `fromFactory [factory]` - resolves parameter by calling the factory. Signature of factory is `unit -> 'value`
- `fromDep` - resolves parameter using container (DEFAULT)
- `fromDepNamed [name]` - same as `fromDep` but with name passed to container resolver

**`resolve [resolve strategy]`** - sets the way how dependency must be resolved. Options here:

- `usingInstance [object]` - resolves dependency using provided object
- `usingFactory [factory]` - resolves dependency by calling the factory. Signature of factory is `unit -> 'service`
- `auto` - resolves dependency using container (DEFAULT)

So default configuration for service looks like:
```fsharp
configure selfContract<Service> {
    lifetime transient
    resolve auto
}
```
And complete example:
```fsharp
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
```



#### :warning: TODO: Move whole documentation to another file

### Contributing

Feel free to submit issues and PR's. If you have any questions contact me via **[email](mailto:andrewrublyov99@gmail.com)** or **[telegram](https://t.me/FreeParticle)**.
