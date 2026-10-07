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
    Loading,
    Saving
}

public enum UiDataState
{
    Unknown,
    Ready,
    Empty,
    Stale,
    Error,
    PermissionDenied,
    Conflict,
    RecoveryRequired
}

public sealed class UiState
{
    public UiConnectionState Connection { get; set; } = UiConnectionState.Unknown;
    public UiBusyState Busy { get; set; } = UiBusyState.Idle;
    public UiDataState Data { get; set; } = UiDataState.Unknown;
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CorrelationId { get; set; }
    public DateTimeOffset? LastKnownAtUtc { get; set; }

    public bool IsOnline => Connection == UiConnectionState.Online;
    public bool IsOffline => Connection == UiConnectionState.Offline;
    public bool IsBusy => Busy != UiBusyState.Idle;
    public bool IsSaving => Busy == UiBusyState.Saving;
    public bool IsStale => Data == UiDataState.Stale;
    public bool IsPermissionDenied => Data == UiDataState.PermissionDenied;
    public bool IsConflict => Data == UiDataState.Conflict;
    public bool IsRecoveryRequired => Data == UiDataState.RecoveryRequired;
}
