using System.Data;
using GameNet.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GameNet.Server.Infrastructure.Transactions;

public sealed class EfTransactionCoordinator(GameNetDbContext dbContext) : ITransactionCoordinator
{
    private const int MaxAttempts = 3;

    public async Task<T> ExecuteSerializableAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

                var result = await action(cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch (Exception exception) when (
                attempt < MaxAttempts &&
                IsTransientSerializationFailure(exception))
            {
                dbContext.ChangeTracker.Clear();

                var delay = TimeSpan.FromMilliseconds(50 * attempt);
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private static bool IsTransientSerializationFailure(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgres &&
                postgres.SqlState is PostgresErrorCodes.SerializationFailure or
                    PostgresErrorCodes.DeadlockDetected)
            {
                return true;
            }
        }

        return false;
    }
}
