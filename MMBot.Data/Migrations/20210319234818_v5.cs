using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

public partial class v5 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<decimal>(
            name: "GuildId",
            table: "BossRaider",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
            name: "GuildId",
            table: "BossRaider");
}
