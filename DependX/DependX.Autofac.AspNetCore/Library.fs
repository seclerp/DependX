module DependX.Autofac.AspNetCore

open Microsoft.Extensions.DependencyInjection
open Autofac.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting

let configureAutofacDefaults (hostBuilder: IWebHostBuilder) =
    hostBuilder.ConfigureServices(fun (collection: IServiceCollection) -> collection.AddAutofac() |> ignore) |> ignore