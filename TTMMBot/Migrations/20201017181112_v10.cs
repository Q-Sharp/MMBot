using Microsoft.EntityFrameworkCore.Migrations;

namespace TTMMBot.Migrations
{
    public partial class v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("AutoSignUpForFighNight", "Member", "AutoSignUpForFightNight");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.RenameColumn("AutoSignUpForFightNight", "Member", "AutoSignUpForFighNight");
        }
    }
}
