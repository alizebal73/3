using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNet.Server.Infrastructure.Outbox;

public sealed class OutboxDeliveryWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxDeliveryOptions> options,
    ILogger<OutboxDeliveryWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        if (!settings.Enabled)
        {
            logger.LogInformation(
                "Outbox delivery is disabled. No external durable side effect publisher is active.");
            return;
        }

        if (settings.BatchSize is < 1 or > 500)
            throw new InvalidOperationException("OutboxDelivery:BatchSize must be between 1 and 500.");
        if (settings.LeaseDurationSeconds <= 0)
            throw new InvalidOperationException("OutboxDelivery:LeaseDurationSeconds must be positive.");
        if (settings.PollIntervalMilliseconds <= 0)
            throw new InvalidOperationException("OutboxDelivery:PollIntervalMilliseconds must be positive.");

        var leaseDuration = TimeSpan.FromSeconds(settings.LeaseDurationSeconds);
        var pollInterval = TimeSpan.FromMilliseconds(settings.PollIntervalMilliseconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var processor = scope.ServiceProvider.GetRequiredService<OutboxDeliveryProcessor>();
                await processor.ProcessBatchAsync(settings.BatchSize, leaseDuration, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "Outbox delivery cycle failed. No message is marked published by the failed cycle.");
            }

            await Task.Delay(pollInterval, stoppingToken);
        }
    }
}