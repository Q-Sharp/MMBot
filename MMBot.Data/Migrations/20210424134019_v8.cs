using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

public partial class v8 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<string>(
            name: "GuildName",
            table: "GuildSettings",
            type: "text",
            nullable: true);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
            name: "GuildName",
            table: "GuildSettings");
}
