module BackgroundTasksSample.Services.MonitorLoop

open System.Threading
open System
open System.Threading.Tasks
open BackgroundTasksSample.Services.BackgroundTaskQueue
open Microsoft.Extensions.Logging

type MonitorLoop(_logger: ILogger<MonitorLoop>, _taskQueue: IBackgroundTaskQueue, _cancellationToken: CancellationToken) =
    member this.StartMonitorLoop() = "Monitor Loop is starting." |> _logger.LogInformation

    member this.Monitor() =
        while (not _cancellationToken.IsCancellationRequested) do
            let keyStroke = Console.ReadKey()
            match keyStroke.Key with
            | ConsoleKey.W ->
                _taskQueue.QueueBackgroundWorkItem(fun token ->
                    async {
                        let mutable delayLoop = 0
                        let guid = Guid.NewGuid().ToString()
                        ("Queued Background Task {Guid} is starting.", guid) |> _logger.LogInformation

                        while ((not token.IsCancellationRequested) && (delayLoop < 3)) do
                            try
                                return! Task.Delay(TimeSpan.FromSeconds(5.), token) |> Async.AwaitTask
                            with :? OperationCanceledException -> ()

                            delayLoop <- delayLoop + 1

                            ("Queued Background Task {Guid} is running. {DelayLoop}/3", guid, delayLoop)
                            |> _logger.LogInformation

                        match delayLoop with
                        | 3 -> ("Queued Background Task {Guid} is complete.", guid) |> _logger.LogInformation
                        | _ -> ("Queued Background Task {Guid} was cancelled", guid) |> _logger.LogInformation
                    })
            | _ -> ()
