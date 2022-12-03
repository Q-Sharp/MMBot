using Microsoft.EntityFrameworkCore.Migrations;

namespace MMBot.Data.Migrations;

/// <inheritdoc />
public partial class rowversion : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
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
            table: "SeasonResult");

         migrationBuilder.DropColumn(
            name: "xmin",
            table: "Season");

         migrationBuilder.DropColumn(
            name: "xmin",
            table: "Restart");

         migrationBuilder.DropColumn(
            name: "xmin",
            table: "RaidParticipation");

         migrationBuilder.DropColumn(
            name: "xmin",
            table: "RaidBoss");

         migrationBuilder.DropColumn(
            name: "xmin",
            table: "MemberRoom");

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

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Vacation",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Timer",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Strike",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "SeasonResult",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Season",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Restart",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "RaidParticipation",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "RaidBoss",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "MemberRoom",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "MemberGroup",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Member",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "GuildSettings",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Clan",
            type: "bytea",
            rowVersion: true,
            nullable: true);

         migrationBuilder.AddColumn<byte[]>(
            name: "Version",
            table: "Channel",
            type: "bytea",
            rowVersion: true,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
         migrationBuilder.DropColumn(
            name: "Version",
            table: "Vacation");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "Timer");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "Strike");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "SeasonResult");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "Season");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "Restart");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "RaidParticipation");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "RaidBoss");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "MemberRoom");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "MemberGroup");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "Member");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "GuildSettings");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "Clan");

         migrationBuilder.DropColumn(
            name: "Version",
            table: "Channel");

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
            table: "SeasonResult",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

         migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Season",
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
            table: "RaidParticipation",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

         migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "RaidBoss",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

         migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "MemberRoom",
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
}
