module DependX

open System

type Lifetime =
    | Singleton
    | Transient
    | Scoped

type DependencyResolveStrategy =
    | Value of obj
    | Reflection
    | Factory of (unit -> obj)

type ParameterResolveStrategy =
    | Value of obj
    | Reflection of string option // Named dependency name
    | Factory of (unit -> obj)
    
type Parameter = {
    name: string
    strategy: ParameterResolveStrategy
}

type Dependency = {
    contractType: Type
    serviceType: Type
    lifetime: Lifetime
    name: string option
    strategy: DependencyResolveStrategy
    parameters: Parameter list
}

let initial<'contract, 'service> : Dependency = {
    contractType = typeof<'contract>
    serviceType = typeof<'service>
    lifetime = Lifetime.Transient
    name = None
    strategy = DependencyResolveStrategy.Reflection
    parameters = []
}

type DependencyBuilder(initial: Dependency) =
    member _.Yield(_) = initial
    member _.Yield() = initial
    
    [<CustomOperation("named")>]
    member _.Named(dependency: Dependency, name) = { dependency with name = Some name }
        
    [<CustomOperation("lifetime")>]
    member _.Lifetime(dependency, lifetime) = { dependency with lifetime = lifetime }

    [<CustomOperation("param")>]
    member _.Param(dependency, name, strategy) =
        { dependency with parameters = dependency.parameters @ [{ name = name; strategy = strategy }]}
    
    [<CustomOperation("resolve")>]
    member _.Resolve(dependency: Dependency, strategy) = { dependency with strategy = strategy}

let configure (initial: Dependency) = DependencyBuilder(initial)
let simpleDependency (dep: Dependency) = DependencyBuilder(dep).Yield()

let interpretBuilder (interpret: 'builder -> Dependency -> unit) (builder: 'builder) (deps: Dependency list) =
    deps |> List.iter (interpret builder) |> ignore
    builder

[<AutoOpen>]
module Helpers =
    let contract<'contract, 'service> = initial<'contract, 'service>
    let selfContract<'service> = initial<'service, 'service>
    let options<'options> (strategy: DependencyResolveStrategy) =
        { selfContract<'options> with
            lifetime = Singleton
            strategy = strategy }

    let singleton = Singleton
    let transient = Transient
    let scoped = Scoped
    
    let usingInstance<'value> (value: 'value) = DependencyResolveStrategy.Value value
    let usingFactory<'value> (factory: unit -> 'value) = DependencyResolveStrategy.Factory (fun () -> factory () |> box)
    let auto = DependencyResolveStrategy.Reflection
    
    let fromInstance<'value> (value: 'value) = ParameterResolveStrategy.Value value
    let fromFactory<'value> (factory: unit -> 'value) = ParameterResolveStrategy.Factory (fun () -> factory () |> box)
    let fromDep = ParameterResolveStrategy.Reflection None
    let fromDepNamed name = ParameterResolveStrategy.Reflection (Some name)

type DependXBuilder<'input, 'builder> = 'input -> 'builder -> 'builder