using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

public partial class v7 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<int>(
            name: "ClanId",
            table: "BossRaid",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        _ = migrationBuilder.CreateIndex(
            name: "IX_BossRaid_ClanId",
            table: "BossRaid",
            column: "ClanId");

        _ = migrationBuilder.AddForeignKey(
            name: "FK_BossRaid_Clan_ClanId",
            table: "BossRaid",
            column: "ClanId",
            principalTable: "Clan",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_BossRaid_Clan_ClanId",
            table: "BossRaid");

        _ = migrationBuilder.DropIndex(
            name: "IX_BossRaid_ClanId",
            table: "BossRaid");

        _ = migrationBuilder.DropColumn(
            name: "ClanId",
            table: "BossRaid");
    }
}
