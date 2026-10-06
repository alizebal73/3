using GameNet.Shared.Api;

namespace GameNet.Shared.Tests;

public sealed class ApiContractTests
{
    [Fact]
    public void ApiError_keeps_machine_code()
        => Assert.Equal("customer.not_found", new ApiError("customer.not_found", "مشتری پیدا نشد.").Code);
}
