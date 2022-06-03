using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMBot.Data.Migrations;

public partial class v12 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "WaitForReaction",
            table: "GuildSettings");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<TimeSpan>(
            name: "WaitForReaction",
            table: "GuildSettings",
            type: "interval",
            nullable: false,
            defaultValue: new TimeSpan(0, 0, 0, 0, 0));
    }
}
