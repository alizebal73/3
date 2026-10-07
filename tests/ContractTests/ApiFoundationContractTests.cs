using GameNet.Shared.Contracts.V1.Api;

namespace GameNet.ContractTests;

public sealed class ApiFoundationContractTests
{
    [Fact]
    public void V1_error_codes_and_headers_are_stable()
    {
        Assert.Equal("X-Correlation-Id", ApiHeaders.CorrelationId);
        Assert.Equal("X-Operation-Id", ApiHeaders.OperationId);
        Assert.Equal("X-Idempotency-Key", ApiHeaders.IdempotencyKey);
        Assert.Equal("X-Contract-Version", ApiHeaders.ContractVersion);
        Assert.Equal("concurrency_conflict", "concurrency_conflict");
        Assert.Equal("idempotency_replay", "idempotency_replay");
    }

    [Fact]
    public void V1_envelope_preserves_operation_identity()
    {
        var envelope = new ApiEnvelope<string>(
            "v1",
            "corr-1",
            "op-1",
            DateTimeOffset.UnixEpoch,
            "ok");

        Assert.Equal("v1", envelope.ContractVersion);
        Assert.Equal("corr-1", envelope.CorrelationId);
        Assert.Equal("op-1", envelope.OperationId);
    }
}
