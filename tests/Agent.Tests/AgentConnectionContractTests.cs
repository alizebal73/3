using GameNet.Shared.Contracts.V1.Protocol;

namespace GameNet.Agent.Tests;

public sealed class AgentConnectionContractTests
{
    [Fact]
    public void Connection_contract_keeps_device_and_connection_identity_distinct()
    {
        var request = new AgentConnectionLeaseRequest(
            "device-1",
            "connection-9",
            DateTimeOffset.UnixEpoch);

        Assert.Equal("device-1", request.DeviceId);
        Assert.Equal("connection-9", request.ConnectionId);
        Assert.NotEqual(request.DeviceId, request.ConnectionId);
    }

    [Fact]
    public void Lease_state_requires_an_explicit_token()
    {
        var state = new AgentConnectionLeaseState(
            "device-1",
            "connection-9",
            "lease-1",
            DateTimeOffset.UnixEpoch.AddMinutes(1),
            true);

        Assert.False(string.IsNullOrWhiteSpace(state.LeaseToken));
        Assert.True(state.IsAuthoritative);
    }
}
