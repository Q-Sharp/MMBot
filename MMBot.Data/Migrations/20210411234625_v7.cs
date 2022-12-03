using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

public partial class v7 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
         migrationBuilder.AddColumn<int>(
            name: "ClanId",
            table: "BossRaid",
            type: "integer",
            nullable: false,
            defaultValue: 0);

         migrationBuilder.CreateIndex(
            name: "IX_BossRaid_ClanId",
            table: "BossRaid",
            column: "ClanId");

         migrationBuilder.AddForeignKey(
            name: "FK_BossRaid_Clan_ClanId",
            table: "BossRaid",
            column: "ClanId",
            principalTable: "Clan",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
         migrationBuilder.DropForeignKey(
            name: "FK_BossRaid_Clan_ClanId",
            table: "BossRaid");

         migrationBuilder.DropIndex(
            name: "IX_BossRaid_ClanId",
            table: "BossRaid");

         migrationBuilder.DropColumn(
            name: "ClanId",
            table: "BossRaid");
    }
}
