using GameNet.Desktop.Api;
using GameNet.Desktop.UI.State;

namespace GameNet.Desktop.UI.Services;

public sealed class ServerConnectivityMonitor(
    IGameNetServerClient serverClient,
    TimeProvider timeProvider,
    ILogger<ServerConnectivityMonitor> logger)
{
    public UiState State { get; } = new();

    public event EventHandler? StateChanged;

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        State.Connection = UiConnectionState.Connecting;
        State.ErrorCode = null;
        State.ErrorMessage = null;
        Publish();

        try
        {
            State.Busy = UiBusyState.Busy;
            var health = await serverClient.GetHealthAsync(cancellationToken);

            State.Connection = health.Status.Equals(
                "Healthy",
                StringComparison.OrdinalIgnoreCase)
                ? UiConnectionState.Online
                : UiConnectionState.Offline;

            logger.LogDebug(
                "Server health refreshed at {Time}. Status={Status}",
                timeProvider.GetUtcNow(),
                health.Status);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            State.Connection = UiConnectionState.Offline;
            State.ErrorCode = "SERVER_UNAVAILABLE";
            State.ErrorMessage = "The GameNet Server is unavailable.";
            logger.LogWarning(
                exception,
                "Server health refresh failed.");
        }
        finally
        {
            State.Busy = UiBusyState.Idle;
            Publish();
        }
    }

    private void Publish() =>
        StateChanged?.Invoke(this, EventArgs.Empty);
}
