using GameNet.Desktop.Api;
using GameNet.Desktop.Infrastructure.Hosting;
using GameNet.Desktop.UI.State;
using Microsoft.Extensions.Options;
using System.Windows;

namespace GameNet.Desktop.UI.Services;

public sealed class ServerConnectivityMonitor(
    IGameNetServerClient serverClient,
    IOptions<DesktopOptions> options,
    TimeProvider timeProvider,
    ILogger<ServerConnectivityMonitor> logger) : IAsyncDisposable
{
    private CancellationTokenSource? _lifetime;
    private Task? _monitorTask;

    public UiState State { get; } = new();

    public event EventHandler? StateChanged;

    public Task StartAsync(CancellationToken applicationStopping)
    {
        if (_monitorTask is not null)
            return Task.CompletedTask;

        _lifetime = CancellationTokenSource.CreateLinkedTokenSource(
            applicationStopping);

        _monitorTask = RunAsync(_lifetime.Token);
        return Task.CompletedTask;
    }

    public async Task RefreshAsync(
        CancellationToken cancellationToken = default)
    {
        State.Connection = UiConnectionState.Connecting;
        State.ErrorCode = null;
        State.ErrorMessage = null;
        Publish();

        try
        {
            State.Busy = UiBusyState.Busy;

            var health = await serverClient.GetHealthAsync(cancellationToken);

            State.Connection =
                health.Status.Equals(
                    "Healthy",
                    StringComparison.OrdinalIgnoreCase) &&
                health.Readiness.Equals(
                    "Ready",
                    StringComparison.OrdinalIgnoreCase)
                    ? UiConnectionState.Online
                    : UiConnectionState.Offline;

            State.CorrelationId = health.CorrelationId;

            if (State.Connection == UiConnectionState.Offline)
            {
                State.ErrorCode = "SERVER_NOT_READY";
                State.ErrorMessage =
                    "The GameNet Server is reachable but is not ready.";
            }

            logger.LogDebug(
                "Server health refreshed at {Time}. Status={Status}; Readiness={Readiness}; CorrelationId={CorrelationId}",
                timeProvider.GetUtcNow(),
                health.Status,
                health.Readiness,
                health.CorrelationId);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            State.Connection = UiConnectionState.Offline;
            State.ErrorCode = "SERVER_UNAVAILABLE";
            State.ErrorMessage =
                "The GameNet Server is unavailable.";

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

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            await RefreshAsync(cancellationToken);

            var seconds = Math.Clamp(
                options.Value.HealthRefreshSeconds,
                5,
                300);

            using var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(seconds));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await RefreshAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private void Publish()
    {
        if (Application.Current?.Dispatcher is { } dispatcher &&
            !dispatcher.CheckAccess())
        {
            _ = dispatcher.InvokeAsync(
                () => StateChanged?.Invoke(this, EventArgs.Empty));
            return;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        if (_lifetime is not null)
        {
            _lifetime.Cancel();

            if (_monitorTask is not null)
            {
                try
                {
                    await _monitorTask;
                }
                catch (OperationCanceledException)
                {
                }
            }

            _lifetime.Dispose();
            _lifetime = null;
        }

        _monitorTask = null;
    }
}
