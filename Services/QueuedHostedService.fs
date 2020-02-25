module BackgroundTasksSample.Services.QueuedHostedService

open System
open System.Threading
open System.Threading.Tasks
open BackgroundTasksSample.Services.BackgroundTaskQueue
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting

type QueuedHostedService(logger: ILogger<QueuedHostedService>, taskQueue: IBackgroundTaskQueue) =
    inherit BackgroundService()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        async { return! this.BackgroundProcessing(stoppingToken) } |> Async.StartAsTask :> Task

    override this.StopAsync(stoppingToken: CancellationToken) =
        "Queued Hosted Service is stopping." |> logger.LogInformation
        base.StopAsync(stoppingToken)

    member this.BackgroundProcessing(stoppingToken: CancellationToken) =
        async {
            while (not stoppingToken.IsCancellationRequested) do
                let! workItem = taskQueue.DequeueAsync(stoppingToken)
                try
                    return! workItem <| stoppingToken
                with :? Exception as ex -> logger.LogError(ex, "Error occurred executing")
        }
