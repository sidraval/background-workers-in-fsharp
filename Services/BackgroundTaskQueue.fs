module BackgroundTasksSample.Services.BackgroundTaskQueue

open System.Collections.Concurrent
open System.Threading

type IBackgroundTaskQueue =
    abstract QueueBackgroundWorkItem: (CancellationToken -> Async<unit>) -> unit
    abstract DequeueAsync: CancellationToken -> Async<CancellationToken -> Async<unit>>

type BackgroundTaskQueue() =
    member val _workItems: ConcurrentQueue<CancellationToken -> Async<unit>> = ConcurrentQueue<CancellationToken -> Async<unit>>
                                                                                   ()
    member val _signal: SemaphoreSlim = new SemaphoreSlim(0)

    interface IBackgroundTaskQueue with
        member this.QueueBackgroundWorkItem(workItem: CancellationToken -> Async<unit>) =
            this._workItems.Enqueue(workItem)
            this._signal.Release() |> ignore

        member this.DequeueAsync(cancellationToken: CancellationToken) =
            async {
                let! _ = this._signal.WaitAsync() |> Async.AwaitTask
                let mutable workItem: CancellationToken -> Async<unit> =
                    Operators.Unchecked.defaultof<CancellationToken -> Async<unit>>
                this._workItems.TryDequeue(&workItem) |> ignore
                return workItem
            }
