using GameNet.Shared.Contracts.V1.Api;

namespace GameNet.Shared.Tests;

public sealed class ApiContractTests
{
    [Fact]
    public void ApiError_keeps_machine_code_and_traceability()
    {
        var error = new ApiError(
            "customer.not_found",
            "customer.not_found",
            "trace-1",
            "op-1",
            Retryable: false);

        Assert.Equal("customer.not_found", error.Code);
        Assert.Equal("trace-1", error.CorrelationId);
        Assert.Equal("op-1", error.OperationId);
        Assert.False(error.Retryable);
    }
}
