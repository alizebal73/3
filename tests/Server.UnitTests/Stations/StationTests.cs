using GameNet.Server.Modules.Stations.Domain;
using GameNet.Shared.Contracts.V1.Stations;

namespace GameNet.Server.UnitTests.Stations;

public sealed class StationTests
{
    [Fact]
    public void Create_defaults_to_available()
    {
        var station = Station.Create(" PC-01 ", "PC 01", StationType.Pc);

        Assert.Equal("PC-01", station.Code);
        Assert.Equal("PC 01", station.Name);
        Assert.Equal(StationOperationalState.Available, station.State);
        Assert.True(station.AgentRequired);
    }

    [Fact]
    public void Disabled_station_cannot_enter_session_directly()
    {
        var station = Station.Create("PC-01", "PC 01", StationType.Pc);
        station.SetState(StationOperationalState.Disabled);

        Assert.Throws<InvalidOperationException>(
            () => station.SetState(StationOperationalState.InUse));
    }

    [Fact]
    public void Ps5_does_not_require_agent()
    {
        var station = Station.Create("PS-01", "PS5 01", StationType.Ps5);

        Assert.False(station.AgentRequired);
    }
}