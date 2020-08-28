using Microsoft.EntityFrameworkCore.Migrations;

namespace TTMMBot.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MemberID",
                table: "Vacation",
                newName: "MemberId");

            migrationBuilder.RenameColumn(
                name: "VacationID",
                table: "Vacation",
                newName: "VacationId");

            migrationBuilder.RenameIndex(
                name: "IX_Vacation_MemberID",
                table: "Vacation",
                newName: "IX_Vacation_MemberId");

            migrationBuilder.RenameColumn(
                name: "ClanID",
                table: "Member",
                newName: "ClanId");

            migrationBuilder.RenameColumn(
                name: "MemberID",
                table: "Member",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Member_ClanID",
                table: "Member",
                newName: "IX_Member_ClanId");

            migrationBuilder.RenameColumn(
                name: "ClanID",
                table: "Clan",
                newName: "ClanId");

            migrationBuilder.AddColumn<bool>(
                name: "IgnoreOnMoveUp",
                table: "Member",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoreOnMoveUp",
                table: "Member");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Vacation",
                newName: "MemberID");

            migrationBuilder.RenameColumn(
                name: "VacationId",
                table: "Vacation",
                newName: "VacationID");

            migrationBuilder.RenameIndex(
                name: "IX_Vacation_MemberId",
                table: "Vacation",
                newName: "IX_Vacation_MemberID");

            migrationBuilder.RenameColumn(
                name: "ClanId",
                table: "Member",
                newName: "ClanID");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Member",
                newName: "MemberID");

            migrationBuilder.RenameIndex(
                name: "IX_Member_ClanId",
                table: "Member",
                newName: "IX_Member_ClanID");

            migrationBuilder.RenameColumn(
                name: "ClanId",
                table: "Clan",
                newName: "ClanID");
        }
    }
}
