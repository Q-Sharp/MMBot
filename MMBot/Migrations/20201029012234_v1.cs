using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Timer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    IsRecurring = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    RingSpan = table.Column<TimeSpan>(nullable: true),
                    GuildId = table.Column<ulong>(nullable: false),
                    ChannelId = table.Column<ulong>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timer", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Timer");
        }
    }
}
