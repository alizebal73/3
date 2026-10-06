namespace GameNet.Server.Infrastructure.Jobs;

public sealed record BackgroundJob(
    string Name,
    Func<CancellationToken, Task> ExecuteAsync);
