using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTMMBot.Migrations
{
    public partial class v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "guildSettings",
                newName: "GuildSettings");

            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "GuildSettings",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "Member",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "Clan",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.RenameColumn(
                table: "GuildSettings",
                name: "guildSettingsId",
                newName: "GuildSettingsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
