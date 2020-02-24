module BackgroundTasksSample.ScopedProcessingService

open Microsoft.Extensions.Logging
open System.Threading

type IScopedProcessingService =
    abstract DoWork: CancellationToken -> Async<unit>

type ScopedProcessingService(_logger: ILogger<ScopedProcessingService>) =
    member val executionCount = 0 with get, set

    interface IScopedProcessingService with
        member this.DoWork(stoppingToken: CancellationToken) =
            async {
                while (not stoppingToken.IsCancellationRequested) do
                    this.executionCount <- this.executionCount + 1
                    ("Scoped Processing Service is working. Count: {Count}", this.executionCount)
                    |> _logger.LogInformation

                    return! Async.Sleep(10000)
            }
