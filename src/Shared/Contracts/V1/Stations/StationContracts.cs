namespace GameNet.Shared.Contracts.V1.Stations;

public enum StationType
{
    Pc = 1,
    Ps5 = 2,
    Foosball = 3
}

public enum StationOperationalState
{
    Disabled = 0,
    Available = 1,
    InUse = 2,
    Offline = 3
}

public sealed record StationDto(
    Guid Id,
    string Code,
    string Name,
    StationType Type,
    StationOperationalState State,
    bool AgentRequired,
    string? AgentDeviceId,
    DateTimeOffset? LastSeenAtUtc);

public sealed record CreateStationRequest(
    string Code,
    string Name,
    StationType Type);

public sealed record RenameStationRequest(string Name);

public sealed record SetStationStateRequest(StationOperationalState State);