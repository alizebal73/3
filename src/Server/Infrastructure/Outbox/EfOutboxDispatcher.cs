using GameNet.Server.Infrastructure.Time;
using GameNet.Server.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Server.Infrastructure.Outbox;

public sealed class EfOutboxDispatcher(GameNetDbContext dbContext, IGameClock clock) : IOutboxDispatcher
{
    public async Task<IReadOnlyList<OutboxMessage>> ClaimBatchAsync(
        int batchSize,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        if (batchSize is < 1 or > 500) throw new ArgumentOutOfRangeException(nameof(batchSize));
        if (leaseDuration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(leaseDuration));

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var now = clock.UtcNow;
        var until = now.Add(leaseDuration);

        var ids = await dbContext.Database.SqlQueryRaw<long>($@"
            SELECT id
            FROM outbox_messages
            WHERE published_at_utc IS NULL
              AND (lease_expires_at_utc IS NULL OR lease_expires_at_utc <= {{0}})
            ORDER BY id
            FOR UPDATE SKIP LOCKED
            LIMIT {{1}}", now, batchSize).ToListAsync(cancellationToken);

        if (ids.Count == 0)
        {
            await transaction.CommitAsync(cancellationToken);
            return Array.Empty<OutboxMessage>();
        }

        var messages = await dbContext.OutboxMessages
            .Where(x => ids.Contains(x.Id))
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
            message.Claim(Guid.NewGuid().ToString("N"), until);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return messages;
    }

    public async Task MarkPublishedAsync(
        long messageId,
        string leaseToken,
        DateTimeOffset publishedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var message = await dbContext.OutboxMessages.SingleAsync(x => x.Id == messageId, cancellationToken);

        if (!string.Equals(message.LeaseToken, leaseToken, StringComparison.Ordinal))
            throw new InvalidOperationException("Outbox lease is no longer owned.");

        message.MarkPublished(publishedAtUtc);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
