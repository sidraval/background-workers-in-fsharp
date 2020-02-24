module BackgroundTasksSample.ConsumeScopedServiceHostedService

open BackgroundTasksSample.ScopedProcessingService
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open System
open System.Threading
open System.Threading.Tasks

type ConsumeScopedServiceHostedService(_logger: ILogger<ConsumeScopedServiceHostedService>, _services: IServiceProvider) =
    inherit BackgroundService()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        "Consume Scoped Service Hosted Service running." |> _logger.LogInformation
        this.DoWork(stoppingToken) |> Async.StartAsTask :> Task

    member this.DoWork(stoppingToken: CancellationToken) =
        "Consume Scoped Service Hosted Service is working." |> _logger.LogInformation
        use scope = _services.CreateScope()
        let service = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>()
        service.DoWork <| stoppingToken

    override this.StopAsync(stoppingToken: CancellationToken) =
        "Consume Scoped Service Hosted Service is stopping." |> _logger.LogInformation
        Task.CompletedTask
