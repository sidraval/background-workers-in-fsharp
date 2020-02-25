open BackgroundTasksSample.ConsumeScopedServiceHostedService
open BackgroundTasksSample.ScopedProcessingService
open BackgroundTasksSample.Services
open BackgroundTasksSample.Services.BackgroundTaskQueue
open BackgroundTasksSample.Services.MonitorLoop
open BackgroundTasksSample.Services.QueuedHostedService
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open BackgroundTasksSample.TimedHostedService

[<EntryPoint>]
let main argv =
    async {
        let hostBuilder = Host.CreateDefaultBuilder()
        hostBuilder.ConfigureServices(fun hostContext services ->
            services.AddSingleton<MonitorLoop>() |> ignore
            services.AddHostedService<QueuedHostedService>() |> ignore
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>() |> ignore

            services.AddHostedService<TimedHostedService>() |> ignore

            services.AddHostedService<ConsumeScopedServiceHostedService>() |> ignore
            services.AddScoped<IScopedProcessingService, ScopedProcessingService>() |> ignore)
        |> ignore

        let host = hostBuilder.Build()
        let! _ = host.StartAsync() |> Async.AwaitTask
        let monitorLoop = host.Services.GetRequiredService<MonitorLoop>()
        monitorLoop.StartMonitorLoop() |> ignore

        return! host.WaitForShutdownAsync() |> Async.AwaitTask
    } |> Async.RunSynchronously
    0
