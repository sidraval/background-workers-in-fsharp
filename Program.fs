open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open BackgroundTasksSample.TimedHostedService

[<EntryPoint>]
let main argv =
    let hostBuilder = Host.CreateDefaultBuilder()
    hostBuilder.ConfigureServices(fun hostContext services -> services.AddHostedService<TimedHostedService>() |> ignore)
    |> ignore
    hostBuilder.Build().Run()
    0
