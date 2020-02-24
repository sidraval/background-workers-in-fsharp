module BackgroundTasksSample.TimedHostedService

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open System
open System.Threading
open System.Threading.Tasks

type TimedHostedService(_logger: ILogger<TimedHostedService>) =
    member val _timer: Timer option = None with get, set
    member val executionCount = 0 with get, set

    interface IHostedService with
        member this.StartAsync(stoppingToken: CancellationToken) =
            "Timed Hosted Service running" |> _logger.LogInformation
            this._timer <- Some(new Timer(this.DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5.)))
            Task.CompletedTask

        member this.StopAsync(stoppingToken: CancellationToken) =
            "Timed Hosted Service is stopping" |> _logger.LogInformation
            match this._timer with
            | Some(t) -> t.Change(Timeout.Infinite, 0) |> ignore
            | None -> ignore()
            Task.CompletedTask

    interface IDisposable with
        member this.Dispose() =
            match this._timer with
            | Some(t) -> t.Dispose()
            | None -> ignore()

    member this.DoWork(state: Object) =
        this.executionCount <- this.executionCount + 1
        ("Timed Hosted Service is working. Count: {Count}", this.executionCount) |> _logger.LogInformation
