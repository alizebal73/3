using GameNet.Server.Modules.Stations.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNet.Server.Persistence.Configurations;

public sealed class StationConfiguration : IEntityTypeConfiguration<Station>
{
    private readonly GameNet.Server.Modules.Stations.Infrastructure.StationConfiguration _inner = new();

    public void Configure(EntityTypeBuilder<Station> builder) =>
        _inner.Configure(builder);
}