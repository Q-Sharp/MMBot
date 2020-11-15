using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Member_Name",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Clan_SortOrder",
                table: "Clan");

            migrationBuilder.DropIndex(
                name: "IX_Clan_Tag",
                table: "Clan");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Name_GuildId",
                table: "Member",
                columns: new[] { "Name", "GuildId" },
                unique: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Member_Name_GuildId",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Clan_SortOrder_GuildId",
                table: "Clan");

            migrationBuilder.DropIndex(
                name: "IX_Clan_Tag_GuildId",
                table: "Clan");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Name",
                table: "Member",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clan_SortOrder",
                table: "Clan",
                column: "SortOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clan_Tag",
                table: "Clan",
                column: "Tag",
                unique: true);
        }
    }
}
