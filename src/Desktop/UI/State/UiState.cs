namespace GameNet.Desktop.UI.State;

public enum UiConnectionState
{
    Unknown,
    Connecting,
    Online,
    Offline
}

public enum UiBusyState
{
    Idle,
    Busy
}

public sealed class UiState
{
    public UiConnectionState Connection { get; set; } = UiConnectionState.Unknown;
    public UiBusyState Busy { get; set; } = UiBusyState.Idle;
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CorrelationId { get; set; }

    public bool IsOnline => Connection == UiConnectionState.Online;
    public bool IsOffline => Connection == UiConnectionState.Offline;
    public bool IsBusy => Busy == UiBusyState.Busy;
}
