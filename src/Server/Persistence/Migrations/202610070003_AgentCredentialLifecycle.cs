using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameNet.Server.Persistence.Migrations;

public partial class AgentCredentialLifecycle : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "agent_credentials",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                device_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                secret_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                revoked_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_authenticated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_agent_credentials", x => x.id));

        migrationBuilder.CreateIndex(
            name: "IX_agent_credentials_Device_Created",
            table: "agent_credentials",
            columns: new[] { "device_id", "created_at_utc" });

        migrationBuilder.CreateIndex(
            name: "IX_agent_credentials_ActiveDevice",
            table: "agent_credentials",
            column: "device_id",
            unique: true,
            filter: "revoked_at_utc IS NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "agent_credentials");
    }
}
