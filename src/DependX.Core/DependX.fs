module DependX

open System

/// Describes how long dependency should live and how often need to be created
type Lifetime =
    /// New instance should be created once per application lifetime
    | Singleton
    /// New instance should be created once per resolve call
    | Transient
    /// New instance should be created once per lifetime scope
    | Scoped

/// Describes how dependency should be resolved
type DependencyResolveStrategy =
    /// Resolve using given value
    | Value of obj
    /// Resolve using reflection (default)
    | Reflection
    /// Resolve using factory function
    | Factory of (unit -> obj)
    /// Resolve type that registrates as non parameterised generic type using reflection (e.g. Repository<>)
    | ReflectionGeneric

/// Describes how constructor parameter should be resolver
type ParameterResolveStrategy =
    /// Resolve using given value
    | Value of obj
    /// Resolve using given value (default)
    /// 'None' for dependency without name and 'Some name' for named dependency
    | Reflection of string option
    /// Resolve using factory function
    | Factory of (unit -> obj)

/// Represents parameter information
type Parameter = {
    /// Name of the parameter
    name: string
    /// Parameter resolve strategy
    strategy: ParameterResolveStrategy
}

/// Represents dependency information
type Dependency = {
    /// Type on which implementation should be resolved
    contractType: Type
    /// Type of implementation
    serviceType: Type
    /// Dependency lifetime
    lifetime: Lifetime
    /// Has value if dependency need to be resolved using name ("named dependency")
    name: string option
    /// Dependency resolve strategy
    strategy: DependencyResolveStrategy
    /// Info about how some of the constructor parameters should be resolved
    parameters: Parameter list
}

/// Initial value of Dependency
/// contract - abstraction type on which 'service' will be resolved
/// service - implementation type
let initialRaw contract service : Dependency = {
    contractType = contract
    serviceType = service
    lifetime = Lifetime.Transient
    name = None
    strategy = DependencyResolveStrategy.Reflection
    parameters = []
}

/// Initial value of Dependency
/// 'contract - abstraction type on which 'service will be resolved
/// 'service - implementation type
let initial<'contract, 'service> : Dependency =
    initialRaw typedefof<'contract> typedefof<'service>

/// Encapsulate logic for building and configuring Dependency
type DependencyBuilder(initial: Dependency) =
    member _.Yield(_) = initial
    member _.Yield() = initial
    
    /// Makes dependency use name for resolving
    [<CustomOperation("named")>]
    member _.Named(dependency: Dependency, name) =
        { dependency with name = Some name }
    
    /// Set lifetime of Dependency 
    [<CustomOperation("lifetime")>]
    member _.Lifetime(dependency, lifetime) =
        { dependency with lifetime = lifetime }

    /// Set custom rule of resolving for constructor parameter with given name
    [<CustomOperation("param")>]
    member _.Param(dependency, name, strategy) =
        { dependency with parameters = { name = name; strategy = strategy }::dependency.parameters}
    
    /// Set resolving strategy of Dependency
    [<CustomOperation("resolve")>]
    member _.Resolve(dependency: Dependency, strategy) =
        { dependency with strategy = strategy}

/// Configurates given contract using DependencyBuilder
let configure (initial: Dependency) = DependencyBuilder(initial)

/// Applies given 'interpret' function using 'builder' as argument for each given dependency
let interpretBuilder (interpret: 'builder -> Dependency -> unit) (builder: 'builder) (deps: Dependency list) =
    deps |> List.iter (interpret builder) |> ignore
    builder

/// Creates default Dependency using 'contract type as a abstraction and 'service as an implementation 
let contract<'contract, 'service> = initial<'contract, 'service>

/// Creates default Dependency using 'service type both for abstraction and implementation 
let selfContract<'service> = contract<'service, 'service>

/// Creates Dependency using 'contract type as a abstraction and 'service as an implementation with 'generic' resolve strategy
let genericContract contract service =
    { initialRaw contract service with strategy = DependencyResolveStrategy.ReflectionGeneric }

/// Creates Dependency using 'service type both for abstraction and implementation with 'generic' resolve strategy
let genericSelfContract service = genericContract service service

/// Registers type with given strategy as Singleton
/// Mainly used for registering simple configuration objects
let options<'options> (strategy: DependencyResolveStrategy) =
    { selfContract<'options> with lifetime = Singleton; strategy = strategy }

/// Shortcut for Lifetime.Singleton
/// New instance should be created once per application lifetime
let singleton = Singleton

/// Shortcut for Lifetime.Transient
/// New instance should be created once per resolve call (default)
let transient = Transient

/// Shortcut for Lifetime.Scoped
/// New instance should be created once per lifetime scope
let scoped = Scoped

/// Shortcut for DependencyResolveStrategy.Value
/// Resolve using given value
let usingInstance<'value> (value: 'value) = DependencyResolveStrategy.Value value

/// Shortcut for DependencyResolveStrategy.Factory
/// Resolve using factory function
let usingFactory<'value> (factory: unit -> 'value) = DependencyResolveStrategy.Factory (fun () -> factory () |> box)

/// Shortcut for DependencyResolveStrategy.Reflection
/// Resolve using reflection (default)
let auto = DependencyResolveStrategy.Reflection

/// Shortcut for ParameterResolveStrategy.Value
/// Resolve using given value
let fromInstance<'value> (value: 'value) = ParameterResolveStrategy.Value value

/// Shortcut for ParameterResolveStrategy.Factory
/// Resolve using factory function
let fromFactory<'value> (factory: unit -> 'value) = ParameterResolveStrategy.Factory (fun () -> factory () |> box)

/// Shortcut for ParameterResolveStrategy.Reflection None
/// Resolve parameter as dependency from IoC container
let fromDep = ParameterResolveStrategy.Reflection None

/// Shortcut for ParameterResolveStrategy.Reflection (Some name)
/// Resolve parameter as dependency with given name from IoC container  
let fromDepNamed name = ParameterResolveStrategy.Reflection (Some name)

/// Describes shape of 'register' function
type Register<'builder> = 'builder -> 'builder