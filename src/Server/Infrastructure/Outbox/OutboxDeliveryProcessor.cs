using GameNet.Server.Infrastructure.Time;
using Microsoft.Extensions.Logging;

namespace GameNet.Server.Infrastructure.Outbox;

public sealed class OutboxDeliveryProcessor(
    IOutboxDispatcher dispatcher,
    IOutboxDeliveryRegistry registry,
    IGameClock clock,
    ILogger<OutboxDeliveryProcessor> logger)
{
    public async Task<int> ProcessBatchAsync(
        int batchSize,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        var messages = await dispatcher.ClaimBatchAsync(batchSize, leaseDuration, cancellationToken);
        var publishedCount = 0;

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!registry.TryGet(message.Type, out var handler) || handler is null)
            {
                logger.LogError(
                    "Outbox message {MessageId} of type {MessageType} has no delivery handler. Lease remains until expiry.",
                    message.Id,
                    message.Type);
                continue;
            }

            try
            {
                await handler.DeliverAsync(message, cancellationToken);
                await dispatcher.MarkPublishedAsync(
                    message.Id,
                    message.LeaseToken ?? throw new InvalidOperationException(
                        $"Claimed Outbox message {message.Id} has no lease token."),
                    clock.UtcNow,
                    cancellationToken);

                publishedCount++;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Outbox delivery failed for message {MessageId} of type {MessageType}. Message remains unpublished.",
                    message.Id,
                    message.Type);
            }
        }

        return publishedCount;
    }
}