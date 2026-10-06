using GameNet.Shared.Primitives;

namespace GameNet.Server.UnitTests;

public sealed class FoundationTests
{
    [Fact]
    public void Idempotency_key_rejects_blank_values()
    {
        Assert.Throws<ArgumentException>(() => new IdempotencyKey(" "));
    }

    [Fact]
    public void Idempotency_key_rejects_values_over_the_protocol_limit()
    {
        var key = new string('x', 201);

        Assert.Throws<ArgumentOutOfRangeException>(() => new IdempotencyKey(key));
    }
}
