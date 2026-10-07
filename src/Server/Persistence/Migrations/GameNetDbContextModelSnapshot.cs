using GameNet.Server.Infrastructure.Audit;
using GameNet.Server.Infrastructure.Outbox;
using GameNet.Server.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GameNet.Server.Persistence.Migrations;

[DbContext(typeof(GameNetDbContext))]
partial class GameNetDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.12")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity<AgentCredential>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.DeviceId)
                .IsUnique()
                .HasFilter("revoked_at_utc IS NULL")
                .HasDatabaseName("IX_agent_credentials_ActiveDevice");
            b.HasIndex(x => new { x.DeviceId, x.CreatedAtUtc })
                .HasDatabaseName("IX_agent_credentials_Device_Created");

            b.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
            b.Property(x => x.DeviceId).HasMaxLength(128).IsRequired().HasColumnName("device_id");
            b.Property(x => x.SecretHash).HasMaxLength(128).IsRequired().HasColumnName("secret_hash");
            b.Property(x => x.CreatedAtUtc).IsRequired().HasColumnName("created_at_utc");
            b.Property(x => x.RevokedAtUtc).HasColumnName("revoked_at_utc");
            b.Property(x => x.LastAuthenticatedAtUtc).HasColumnName("last_authenticated_at_utc");
            b.ToTable("agent_credentials");
        });

        modelBuilder.Entity<AuditEntry>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.OccurredAtUtc, x.Operation });
            b.HasIndex(x => new { x.ReferenceType, x.ReferenceId });

            b.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id");
            b.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc");
            b.Property(x => x.ActorType).HasMaxLength(64).IsRequired().HasColumnName("actor_type");
            b.Property(x => x.ActorId).HasMaxLength(128).HasColumnName("actor_id");
            b.Property(x => x.Operation).HasMaxLength(200).IsRequired().HasColumnName("operation");
            b.Property(x => x.ReferenceType).HasMaxLength(100).HasColumnName("reference_type");
            b.Property(x => x.ReferenceId).HasMaxLength(128).HasColumnName("reference_id");
            b.Property(x => x.Reason).HasMaxLength(1000).HasColumnName("reason");
            b.Property(x => x.CorrelationId).HasMaxLength(128).IsRequired().HasColumnName("correlation_id");
            b.Property(x => x.BeforeJson).HasColumnType("jsonb").HasColumnName("before_json");
            b.Property(x => x.AfterJson).HasColumnType("jsonb").HasColumnName("after_json");
            b.ToTable("audit_entries");
        });

        modelBuilder.Entity<IdempotencyRecord>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.Scope, x.Key }).IsUnique();
            b.HasIndex(x => x.ExpiresAtUtc);

            b.Property(x => x.Id).ValueGeneratedOnAdd().HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn).HasColumnName("id");
            b.Property(x => x.Scope).HasMaxLength(160).IsRequired().HasColumnName("scope");
            b.Property(x => x.Key).HasMaxLength(200).IsRequired().HasColumnName("key");
            b.Property(x => x.Operation).HasMaxLength(200).IsRequired().HasColumnName("operation");
            b.Property(x => x.State).HasMaxLength(32).IsRequired().HasColumnName("state");
            b.Property(x => x.LeaseToken).HasMaxLength(128).IsRequired().HasColumnName("lease_token");
            b.Property(x => x.StatusCode).HasColumnName("status_code");
            b.Property(x => x.ResponseJson).HasColumnType("jsonb").IsRequired().HasColumnName("response_json");
            b.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            b.Property(x => x.LeaseExpiresAtUtc).HasColumnName("lease_expires_at_utc");
            b.Property(x => x.ExpiresAtUtc).HasColumnName("expires_at_utc");
            b.ToTable("idempotency_records");
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.EventId).IsUnique();
            b.HasIndex(x => new { x.PublishedAtUtc, x.OccurredAtUtc });
            b.HasIndex(x => new { x.LeaseExpiresAtUtc, x.Id });

            b.Property(x => x.Id).ValueGeneratedOnAdd().HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn).HasColumnName("id");
            b.Property(x => x.EventId).HasColumnName("event_id");
            b.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc");
            b.Property(x => x.Type).HasMaxLength(200).IsRequired().HasColumnName("type");
            b.Property(x => x.PayloadJson).HasColumnType("jsonb").IsRequired().HasColumnName("payload_json");
            b.Property(x => x.PublishedAtUtc).HasColumnName("published_at_utc");
            b.Property(x => x.LeaseToken).HasMaxLength(128).HasColumnName("lease_token");
            b.Property(x => x.LeaseExpiresAtUtc).HasColumnName("lease_expires_at_utc");
            b.ToTable("outbox_messages");
        });
    }
}
