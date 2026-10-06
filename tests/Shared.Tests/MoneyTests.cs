using GameNet.Shared.Primitives;

namespace GameNet.Shared.Tests;

public sealed class MoneyTests
{
    [Fact]
    public void Add_requires_matching_currency()
    {
        var toman = Money.From(100, "TOM");
        var other = Money.From(50, "USD");

        Assert.Throws<InvalidOperationException>(() => toman.Add(other));
    }

    [Fact]
    public void Addition_preserves_currency_and_amount()
    {
        var value = Money.From(100, "TOM").Add(Money.From(50, "tom"));

        Assert.Equal(150, value.Amount);
        Assert.Equal("TOM", value.Currency);
    }

    [Fact]
    public void Empty_currency_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => Money.From(100, ""));
    }
}
