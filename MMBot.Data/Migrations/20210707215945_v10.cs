using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

public partial class v10 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AutoSignUpForFightNight",
            table: "Member");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "AutoSignUpForFightNight",
            table: "Member",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
