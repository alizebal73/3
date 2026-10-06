namespace GameNet.Agent;

public sealed class AgentWorker(
    ILogger<AgentWorker> logger,
    TimeProvider timeProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("GameNet Agent foundation started at {Time}", timeProvider.GetUtcNow());

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            logger.LogDebug("Agent heartbeat placeholder at {Time}", timeProvider.GetUtcNow());
        }
    }
}
