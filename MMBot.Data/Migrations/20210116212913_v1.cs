using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

public partial class v1 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Vacation",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Timer",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Strike",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Restart",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "MemberGroup",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Member",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "GuildSettings",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Clan",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Channel",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Vacation");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Timer");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Strike");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Restart");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "MemberGroup");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Member");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "GuildSettings");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Clan");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Channel");
    }
}
