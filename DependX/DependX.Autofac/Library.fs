module DependX.Autofac

open DependX
open Autofac
open Autofac.Builder

type AutofacBuilder = ContainerBuilder


let interpret (autofacBuilder: AutofacBuilder) (state: Dependency) =
    let interpretParameter (depBuilder: IRegistrationBuilder<'a, ReflectionActivatorData, 'c>) =
        let interpretParameter' (parameter: Parameter) =
            match parameter.strategy with
            | ParameterResolveStrategy.Value value -> depBuilder.WithParameter(parameter.name, value) |> ignore
            | ParameterResolveStrategy.Reflection (Some name) -> depBuilder.WithParameter(parameter.name, fun  _ (ctx: IComponentContext) -> ctx.ResolveNamed(name)) |> ignore 
            | ParameterResolveStrategy.Reflection (None) -> () // Do nothing, it will be resolved automatically
            | ParameterResolveStrategy.Factory factory -> depBuilder.WithParameter(parameter.name, fun _ -> factory()) |> ignore
    
        state.parameters |> List.iter interpretParameter'
    
    let interpretDepBuilder (depBuilder: IRegistrationBuilder<'a, 'b, 'c>) =
        match state.name with
        | Some name -> depBuilder.Named(name, state.contractType) |> ignore
        | None -> depBuilder.As(state.contractType) |> ignore
        
        match state.lifetime with
        | Singleton -> depBuilder.SingleInstance() |> ignore
        | Transient -> depBuilder.InstancePerDependency() |> ignore
        | Scoped -> depBuilder.InstancePerLifetimeScope() |> ignore
        
        match depBuilder with
        | :? IRegistrationBuilder<'a, ReflectionActivatorData, 'c> as v -> interpretParameter v
        | _ -> () //TODO: Throw error

    match state.strategy with
    | DependencyResolveStrategy.Value value -> autofacBuilder.RegisterInstance(value) |> interpretDepBuilder
    | DependencyResolveStrategy.Reflection -> autofacBuilder.RegisterType(state.serviceType) |> interpretDepBuilder
    | DependencyResolveStrategy.Factory factory -> autofacBuilder.Register(fun _ -> factory()) |> interpretDepBuilder

let dependencies deps (builder: AutofacBuilder) = interpretBuilder interpret builder deps

type DependenciesModule<'parameters>(register : 'parameters -> ContainerBuilder -> ContainerBuilder, parameters: 'parameters) =
    inherit Module()
    override __.Load(builder: ContainerBuilder) =
        register parameters builder |> ignore

let makeContainer (register : DependXBuilder<'parameters, AutofacBuilder>) (parameters: 'parameters) =
    (ContainerBuilder() |> register parameters).Build()

let makeModule (register : DependXBuilder<'parameters, AutofacBuilder>) (parameters: 'parameters) =
    DependenciesModule<'parameters>(register, parameters)

let addModule (builder: ContainerBuilder) (module': DependenciesModule<'parameters>) =
    builder.RegisterModule(module') |> ignore