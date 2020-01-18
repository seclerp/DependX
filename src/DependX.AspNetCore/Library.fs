module DependX.AspNetCore

open Microsoft.Extensions.Hosting

/// Creates dependency as IHostedService (with singleton lifetime)
/// Equivalent of IServiceCollection.AddHostedService
let hostedService<'service when 'service :> IHostedService> =
    { initial<IHostedService, 'service> with lifetime = Singleton }