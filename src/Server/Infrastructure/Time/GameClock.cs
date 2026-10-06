using GameNet.Server.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace GameNet.Server.Infrastructure.Time;

public interface IGameClock
{
    DateTimeOffset UtcNow { get; }
    DateTimeOffset LocalNow { get; }
    DateOnly BusinessDate { get; }
}

public sealed class GameClock(
    TimeProvider timeProvider,
    IOptions<GameNetOptions> options) : IGameClock
{
    private readonly TimeZoneInfo _timeZone =
        GameTimeZoneResolver.Resolve(options.Value.BusinessTimeZone);

    public DateTimeOffset UtcNow => timeProvider.GetUtcNow();

    public DateTimeOffset LocalNow =>
        TimeZoneInfo.ConvertTime(UtcNow, _timeZone);

    public DateOnly BusinessDate => DateOnly.FromDateTime(LocalNow.DateTime);
}
