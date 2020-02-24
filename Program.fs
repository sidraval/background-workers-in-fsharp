open BackgroundTasksSample.ConsumeScopedServiceHostedService
open BackgroundTasksSample.ScopedProcessingService
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open BackgroundTasksSample.TimedHostedService

[<EntryPoint>]
let main argv =
    let hostBuilder = Host.CreateDefaultBuilder()
    hostBuilder.ConfigureServices(fun hostContext services ->
        services.AddHostedService<TimedHostedService>() |> ignore
        services.AddHostedService<ConsumeScopedServiceHostedService>() |> ignore
        services.AddScoped<IScopedProcessingService, ScopedProcessingService>() |> ignore)
    |> ignore
    hostBuilder.Build().Run()
    0
