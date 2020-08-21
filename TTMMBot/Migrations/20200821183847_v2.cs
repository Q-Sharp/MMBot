using Microsoft.EntityFrameworkCore.Migrations;

namespace TTMMBot.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllTimeHigh",
                table: "Member",
                newName: "AHigh");

            migrationBuilder.RenameColumn(
                name: "JoinOrder",
                table: "Member",
                newName: "Join");

            migrationBuilder.RenameColumn(
                name: "SeasonHighest",
                table: "Member",
                newName: "SHigh");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AHigh",
                table: "Member",
                newName: "AllTimeHigh");

            migrationBuilder.RenameColumn(
                name: "Join",
                table: "Member",
                newName: "JoinOrder");

            migrationBuilder.RenameColumn(
                name: "SHigh",
                table: "Member",
                newName: "SeasonHighest");
        }
    }
}
