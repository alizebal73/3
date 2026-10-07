using GameNet.Shared.Contracts.V1.Api;

namespace GameNet.ContractTests;

public sealed class ApiFoundationContractTests
{
    [Fact]
    public void V1_headers_are_stable()
    {
        Assert.Equal("X-Correlation-Id", ApiHeaders.CorrelationId);
        Assert.Equal("X-Operation-Id", ApiHeaders.OperationId);
        Assert.Equal("X-Idempotency-Key", ApiHeaders.IdempotencyKey);
        Assert.Equal("X-Contract-Version", ApiHeaders.ContractVersion);
        Assert.Equal("ETag", ApiHeaders.ETag);
    }

    [Fact]
    public void V1_error_codes_are_stable()
    {
        Assert.Equal("concurrency_conflict", ApiErrorCodes.ConcurrencyConflict);
        Assert.Equal("idempotency_replay", ApiErrorCodes.IdempotencyReplay);
        Assert.Equal("internal_error", ApiErrorCodes.Internal);
    }

    [Fact]
    public void V1_envelope_preserves_operation_identity()
    {
        var envelope = new ApiEnvelope<string>(
            ContractVersions.V1,
            "corr-1",
            "op-1",
            DateTimeOffset.UnixEpoch,
            "ok");

        Assert.Equal(ContractVersions.V1, envelope.ContractVersion);
        Assert.Equal("corr-1", envelope.CorrelationId);
        Assert.Equal("op-1", envelope.OperationId);
    }
}
