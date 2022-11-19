using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

/// <inheritdoc />
public partial class rowversion : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
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
            table: "SeasonResult");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Season");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "Restart");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "RaidParticipation");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "RaidBoss");

        _ = migrationBuilder.DropColumn(
            name: "xmin",
            table: "MemberRoom");

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

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Vacation",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Timer",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Strike",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "SeasonResult",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Season",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Restart",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "RaidParticipation",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "RaidBoss",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "MemberRoom",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "MemberGroup",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Member",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "GuildSettings",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Clan",
            type: "bytea",
            rowVersion: true,
            nullable: true);

        _ = migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Channel",
            type: "bytea",
            rowVersion: true,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "Vacation");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "Timer");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "Strike");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "SeasonResult");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "Season");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "Restart");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "RaidParticipation");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "RaidBoss");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "MemberRoom");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "MemberGroup");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "Member");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "GuildSettings");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "Clan");

        _ = migrationBuilder.DropColumn(
            name: "Version",
            table: "Channel");

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
            table: "SeasonResult",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Season",
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
            table: "RaidParticipation",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "RaidBoss",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        _ = migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "MemberRoom",
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
}
