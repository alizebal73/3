using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameNet.Server.Persistence.Migrations;

[DbContext(typeof(GameNetDbContext))]
[Migration("202610070002_FoundationClosureHardening")]
public partial class FoundationClosureHardening : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "agent_connection_leases",
            columns: table => new
            {
                device_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                connection_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                lease_token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                lease_expires_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                last_heartbeat_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                agent_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                station_state = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
            },
            constraints: table => table.PrimaryKey("pk_agent_connection_leases", x => x.device_id));

        migrationBuilder.CreateIndex(
            name: "ix_agent_connection_leases_lease_expires_at_utc",
            table: "agent_connection_leases",
            column: "lease_expires_at_utc");

        migrationBuilder.Sql(
            """
            CREATE OR REPLACE FUNCTION gamenet_reject_audit_mutation()
            RETURNS trigger
            LANGUAGE plpgsql
            AS $function$
            BEGIN
                RAISE EXCEPTION 'audit_entries is append-only; UPDATE/DELETE is forbidden';
            END;
            $function$;

            CREATE TRIGGER audit_entries_append_only
            BEFORE UPDATE OR DELETE
            ON audit_entries
            FOR EACH ROW
            EXECUTE FUNCTION gamenet_reject_audit_mutation();
            """);

        migrationBuilder.Sql(
            """
            REVOKE UPDATE, DELETE ON audit_entries FROM PUBLIC;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DROP TRIGGER IF EXISTS audit_entries_append_only ON audit_entries;
            DROP FUNCTION IF EXISTS gamenet_reject_audit_mutation();
            """);

        migrationBuilder.Sql(
            """
            GRANT UPDATE, DELETE ON audit_entries TO PUBLIC;
            """);

        migrationBuilder.DropTable(name: "agent_connection_leases");
    }
}
