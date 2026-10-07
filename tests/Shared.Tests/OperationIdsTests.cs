using GameNet.Shared.Primitives;

namespace GameNet.Shared.Tests;

public sealed class OperationIdsTests
{
    [Fact]
    public void IdempotencyKey_accepts_non_empty_value()
    {
        var key = new IdempotencyKey("checkout-123");

        Assert.Equal("checkout-123", key.Value);
        Assert.Equal("checkout-123", key.ToString());
    }

    [Fact]
    public void IdempotencyKey_rejects_blank_value()
    {
        Assert.Throws<ArgumentException>(() => new IdempotencyKey("   "));
    }

    [Fact]
    public void IdempotencyKey_rejects_values_longer_than_200_characters()
    {
        var value = new string('x', 201);

        Assert.Throws<ArgumentOutOfRangeException>(() => new IdempotencyKey(value));
    }
}
