using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_GuildId",
                table: "GuildSettings",
                column: "GuildId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GuildSettings_GuildId",
                table: "GuildSettings");
        }
    }
}
