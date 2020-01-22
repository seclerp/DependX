module DependX.AspNetCore

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

/// Creates dependency as IHostedService (with singleton lifetime)
/// Equivalent of IServiceCollection.AddHostedService
let hostedService<'service when 'service :> IHostedService> =
    { initial<IHostedService, 'service> with lifetime = Singleton }

/// Creates dependency as ILogger<> (with singleton lifetime)
let typedLogger<'logger> =
    { genericContract typedefof<ILogger<_>> typedefof<'logger> with lifetime = Singleton }