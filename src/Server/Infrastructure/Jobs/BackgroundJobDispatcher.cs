namespace GameNet.Server.Infrastructure.Jobs;

public sealed class BackgroundJobDispatcher(
    IBackgroundJobQueue queue,
    ILogger<BackgroundJobDispatcher> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await job.ExecuteAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Background job {JobName} failed.",
                    job.Name);
            }
        }
    }
}
