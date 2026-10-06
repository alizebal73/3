using GameNet.Shared.Api;
using GameNet.Shared.Contracts.Errors;

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
}
