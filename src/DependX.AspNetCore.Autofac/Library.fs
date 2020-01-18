module DependX.Autofac.AspNetCore

open Microsoft.Extensions.DependencyInjection
open Autofac.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting

let configureAutofacDefaults (hostBuilder: IWebHostBuilder) =
    hostBuilder.ConfigureServices(fun (collection: IServiceCollection) -> collection.AddAutofac() |> ignore) |> ignore

// TODO: May be used in future support for IOptions<>
//let usesOptions (builder: ContainerBuilder) : ContainerBuilder =
//    builder.RegisterType(typedefof<OptionsManager<_>>).As(typeof<IOptions<_>>).SingleInstance() |> ignore
//    builder.RegisterType(typedefof<OptionsManager<_>>).As(typeof<IOptionsSnapshot<_>>).InstancePerLifetimeScope() |> ignore
//    builder.RegisterType(typedefof<OptionsMonitor<_>>).As(typeof<IOptionsMonitor<_>>).SingleInstance() |> ignore
//    builder.RegisterType(typedefof<OptionsFactory<_>>).As(typeof<IOptionsFactory<_>>) |> ignore
//    builder.RegisterType(typedefof<OptionsCache<_>>).As(typeof<IOptionsMonitorCache<_>>).SingleInstance() |> ignore
//    builder