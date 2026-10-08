namespace GameNet.Server.Modules.Stations.Domain;

using GameNet.Shared.Contracts.V1.Stations;

public sealed class Station
{
    private Station() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public StationType Type { get; private set; }
    public StationOperationalState State { get; private set; }
    public string? AgentDeviceId { get; private set; }
    public DateTimeOffset? LastSeenAtUtc { get; private set; }

    public bool AgentRequired => Type == StationType.Pc;

    public static Station Create(
        string code,
        string name,
        StationType type)
    {
        code = Normalize(code);
        name = Normalize(name);

        if (code.Length is < 1 or > 64)
            throw new ArgumentException("Station code must be 1-64 characters.", nameof(code));

        if (name.Length is < 1 or > 120)
            throw new ArgumentException("Station name must be 1-120 characters.", nameof(name));

        return new Station
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Type = type,
            State = StationOperationalState.Available
        };
    }

    public void Rename(string name)
    {
        name = Normalize(name);
        if (name.Length is < 1 or > 120)
            throw new ArgumentException("Station name must be 1-120 characters.", nameof(name));

        Name = name;
    }

    public void SetState(StationOperationalState state)
    {
        if (State == StationOperationalState.Disabled &&
            state == StationOperationalState.InUse)
            throw new InvalidOperationException("A disabled station cannot enter a session directly.");

        State = state;
    }

    public void AssignAgent(string deviceId, DateTimeOffset observedAtUtc)
    {
        if (!AgentRequired)
            throw new InvalidOperationException("Only PC stations can have an Agent.");

        deviceId = Normalize(deviceId);
        if (deviceId.Length is < 1 or > 128)
            throw new ArgumentException("Agent device id is invalid.", nameof(deviceId));

        AgentDeviceId = deviceId;
        LastSeenAtUtc = observedAtUtc;

        if (State == StationOperationalState.Offline)
            State = StationOperationalState.Available;
    }

    private static string Normalize(string value) =>
        value.Trim();
}