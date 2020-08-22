using Microsoft.EntityFrameworkCore.Migrations;

namespace TTMMBot.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscordRole",
                table: "Clan",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordRole",
                table: "Clan");
        }
    }
}
