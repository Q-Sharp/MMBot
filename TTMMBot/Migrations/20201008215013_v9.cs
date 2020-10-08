using Microsoft.EntityFrameworkCore.Migrations;

namespace TTMMBot.Migrations
{
    public partial class v9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoSignUpForFighNight",
                table: "Member",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PlayerTag",
                table: "Member",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoSignUpForFighNight",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "PlayerTag",
                table: "Member");
        }
    }
}
