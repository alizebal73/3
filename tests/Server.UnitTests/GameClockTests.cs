using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Time;
using Microsoft.Extensions.Options;

namespace GameNet.Server.UnitTests;

public sealed class GameClockTests
{
    [Fact]
    public void Business_date_comes_from_configured_timezone()
    {
        var utc = new FixedTimeProvider(
            new DateTimeOffset(2026, 10, 6, 21, 30, 0, TimeSpan.Zero));

        var options = Options.Create(new GameNetOptions
        {
            BusinessTimeZone = "Asia/Tehran"
        });

        var clock = new GameClock(utc, options);

        Assert.Equal(new DateOnly(2026, 10, 7), clock.BusinessDate);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
