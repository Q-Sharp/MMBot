using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clan_SortOrder_GuildId",
                table: "Clan");

            migrationBuilder.DropIndex(
                name: "IX_Clan_Tag_GuildId",
                table: "Clan");

            migrationBuilder.CreateIndex(
                name: "IX_Clan_Tag_Name_GuildId_SortOrder",
                table: "Clan",
                columns: new[] { "Tag", "Name", "GuildId", "SortOrder" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clan_Tag_Name_GuildId_SortOrder",
                table: "Clan");

            migrationBuilder.CreateIndex(
                name: "IX_Clan_SortOrder_GuildId",
                table: "Clan",
                columns: new[] { "SortOrder", "GuildId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clan_Tag_GuildId",
                table: "Clan",
                columns: new[] { "Tag", "GuildId" },
                unique: true);
        }
    }
}
