using Microsoft.EntityFrameworkCore.Migrations;

namespace TTMMBot.Migrations
{
    public partial class v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberMovementQty",
                table: "guildSettings",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Clan",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("update Clan set SortOrder = ClanId");
                
            migrationBuilder.CreateIndex(
                name: "IX_Clan_SortOrder",
                table: "Clan",
                column: "SortOrder",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clan_SortOrder",
                table: "Clan");

            migrationBuilder.DropColumn(
                name: "MemberMovementQty",
                table: "guildSettings");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Clan");
        }
    }
}
