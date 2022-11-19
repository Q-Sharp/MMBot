using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

public partial class v1 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Vacation",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Timer",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Strike",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Restart",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "MemberGroup",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Member",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "GuildSettings",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Clan",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Channel",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Vacation");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Timer");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Strike");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Restart");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "MemberGroup");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Member");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "GuildSettings");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Clan");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Channel");
    }
}
