using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameNet.Server.Persistence.Migrations;

[DbContext(typeof(GameNetDbContext))]
[Migration("202610070001_FoundationInfrastructure")]
public partial class FoundationInfrastructure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "audit_entries",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                actor_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                actor_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                operation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                reference_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                reference_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                correlation_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                before_json = table.Column<string>(type: "jsonb", nullable: true),
                after_json = table.Column<string>(type: "jsonb", nullable: true)
            },
            constraints: table => table.PrimaryKey("pk_audit_entries", x => x.id));

        migrationBuilder.CreateTable(
            name: "idempotency_records",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                scope = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                operation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                state = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                lease_token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                status_code = table.Column<int>(type: "integer", nullable: false),
                response_json = table.Column<string>(type: "jsonb", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                lease_expires_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                expires_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("pk_idempotency_records", x => x.id));

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                event_id = table.Column<Guid>(type: "uuid", nullable: false),
                occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                payload_json = table.Column<string>(type: "jsonb", nullable: false),
                published_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                lease_token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                lease_expires_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("pk_outbox_messages", x => x.id));

        migrationBuilder.CreateIndex(
            name: "ix_audit_entries_occurred_at_utc_operation",
            table: "audit_entries",
            columns: new[] { "occurred_at_utc", "operation" });

        migrationBuilder.CreateIndex(
            name: "ix_audit_entries_reference_type_reference_id",
            table: "audit_entries",
            columns: new[] { "reference_type", "reference_id" });

        migrationBuilder.CreateIndex(
            name: "ix_idempotency_records_expires_at_utc",
            table: "idempotency_records",
            column: "expires_at_utc");

        migrationBuilder.CreateIndex(
            name: "ix_idempotency_records_scope_key",
            table: "idempotency_records",
            columns: new[] { "scope", "key" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_outbox_messages_event_id",
            table: "outbox_messages",
            column: "event_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_outbox_messages_lease_expires_at_utc_id",
            table: "outbox_messages",
            columns: new[] { "lease_expires_at_utc", "id" });

        migrationBuilder.CreateIndex(
            name: "ix_outbox_messages_published_at_utc_occurred_at_utc",
            table: "outbox_messages",
            columns: new[] { "published_at_utc", "occurred_at_utc" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "audit_entries");
        migrationBuilder.DropTable(name: "idempotency_records");
        migrationBuilder.DropTable(name: "outbox_messages");
    }
}
