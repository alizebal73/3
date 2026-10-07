using GameNet.Shared.Api;
using GameNet.Shared.Contracts.Errors;
using GameNet.Shared.Contracts.V1.Protocol;
using GameNet.Shared.Contracts.V1.System;
using GameNet.Shared.Primitives;

namespace GameNet.ContractTests;

public sealed class ContractSmokeTests
{
    [Fact]
    public void Shared_failure_contract_is_constructible()
    {
        var failure = new ApiFailure(
            new ApiError("foundation.error", "خطای زیرساختی"),
            "trace-1");

        Assert.Equal("foundation.error", failure.Error.Code);
        Assert.Equal("trace-1", failure.TraceId);
    }

    [Fact]
    public void Health_contract_is_versioned()
    {
        var health = new HealthResponse(
            "ok",
            "GameNet.Server",
            "0.2.0-foundation",
            "ready",
            CorrelationId.New().Value);

        Assert.Equal("GameNet.Server", health.Service);
        Assert.Equal("ready", health.Readiness);
    }

    [Fact]
    public void Agent_command_has_stable_command_identity()
    {
        var id = CommandId.New();
        var command = new AgentCommand<string>(
            id,
            "device-1",
            "lease-1",
            DateTimeOffset.UnixEpoch,
            DateTimeOffset.UnixEpoch.AddMinutes(1),
            "server.test",
            "payload");

        Assert.Equal(id, command.CommandId);
        Assert.Equal("device-1", command.DeviceId);
        Assert.Equal("lease-1", command.LeaseToken);
        Assert.Equal(DateTimeOffset.UnixEpoch.AddMinutes(1), command.ExpiresAtUtc);
    }

    [Fact]
    public void Agent_command_acknowledgement_preserves_duplicate_state()
    {
        var ack = new CommandAcknowledgement(
            CommandId.New(),
            "device-1",
            "lease-1",
            DateTimeOffset.UnixEpoch,
            Success: true,
            Duplicate: true,
            ErrorCode: null);

        Assert.True(ack.Success);
        Assert.True(ack.Duplicate);
    }
}
