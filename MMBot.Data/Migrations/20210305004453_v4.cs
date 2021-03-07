using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MMBot.Data.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BossRaid",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BossType = table.Column<byte>(type: "smallint", nullable: false),
                    ModifierOne = table.Column<byte>(type: "smallint", nullable: false),
                    ModifierTwo = table.Column<byte>(type: "smallint", nullable: false),
                    ModifierThree = table.Column<byte>(type: "smallint", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BossRaid", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BossRaider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RaidParticipationId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    DamageDone = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    HeartQty = table.Column<int>(type: "integer", nullable: false),
                    AttackQty = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BossRaider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BossRaider_BossRaid_RaidParticipationId",
                        column: x => x.RaidParticipationId,
                        principalTable: "BossRaid",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BossRaider_Member_RaidParticipationId",
                        column: x => x.RaidParticipationId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BossRaider_RaidParticipationId",
                table: "BossRaider",
                column: "RaidParticipationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BossRaider");

            migrationBuilder.DropTable(
                name: "BossRaid");
        }
    }
}
