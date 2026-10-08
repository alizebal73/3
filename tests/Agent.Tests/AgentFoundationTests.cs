using GameNet.Shared.Contracts.V1.Protocol;
using GameNet.Shared.Primitives;

namespace GameNet.Agent.Tests;

public sealed class AgentFoundationTests
{
    [Fact]
    public void Agent_command_preserves_stable_device_and_command_identity()
    {
        var commandId = CommandId.New();
        var command = new AgentCommand<string>(
            commandId,
            "PC-04",
            "lease-1",
            DateTimeOffset.UnixEpoch,
            DateTimeOffset.UnixEpoch.AddMinutes(1),
            "station.reconcile",
            "payload");

        Assert.Equal(commandId, command.CommandId);
        Assert.Equal("PC-04", command.DeviceId);
        Assert.Equal("station.reconcile", command.Type);
    }
}
