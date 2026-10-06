namespace GameNet.Server.Infrastructure.Transactions;

public interface ITransactionCoordinator
{
    Task<T> ExecuteSerializableAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default);
}
