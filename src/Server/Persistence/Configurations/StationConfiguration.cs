using GameNet.Server.Modules.Stations.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNet.Server.Persistence.Configurations;

public sealed class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("stations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Code).HasMaxLength(64).IsRequired().HasColumnName("code");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("ux_stations_code");

        builder.Property(x => x.Name).HasMaxLength(120).IsRequired().HasColumnName("name");
        builder.Property(x => x.Type).HasColumnName("type");
        builder.Property(x => x.State).HasColumnName("state");
        builder.Property(x => x.AgentDeviceId).HasMaxLength(128).HasColumnName("agent_device_id");
        builder.Property(x => x.LastSeenAtUtc).HasColumnName("last_seen_at_utc");

        builder.Ignore(x => x.AgentRequired);
    }
}