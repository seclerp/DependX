### Documentation

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
        named "Some name"
        param "abc" (fromInstance 123)
        param "abc2" (fromFactory <| fun () -> 123)
    }
    contract<IService, Service>
]
```
