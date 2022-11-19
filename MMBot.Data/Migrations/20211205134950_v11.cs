using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MMBot.Data.Migrations;

public partial class v11 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "StartDate",
            table: "Vacation",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "EndDate",
            table: "Vacation",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "StartTime",
            table: "Timer",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone",
            oldNullable: true);

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "EndTime",
            table: "Timer",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone",
            oldNullable: true);

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "StrikeDate",
            table: "Strike",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone",
            oldNullable: true);

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "Start",
            table: "Season",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "LastUpdated",
            table: "Member",
            type: "timestamp with time zone",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone",
            oldNullable: true);

        _ = migrationBuilder.AddColumn<decimal>(
            name: "CategoryId",
            table: "GuildSettings",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m);

        _ = migrationBuilder.AddColumn<decimal>(
            name: "MemberRoleId",
            table: "GuildSettings",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m);

        _ = migrationBuilder.CreateTable(
            name: "MemberRoom",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                RoomId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_MemberRoom", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "MemberRoom");

        _ = migrationBuilder.DropColumn(
            name: "CategoryId",
            table: "GuildSettings");

        _ = migrationBuilder.DropColumn(
            name: "MemberRoleId",
            table: "GuildSettings");

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "StartDate",
            table: "Vacation",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "EndDate",
            table: "Vacation",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "StartTime",
            table: "Timer",
            type: "timestamp without time zone",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldNullable: true);

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "EndTime",
            table: "Timer",
            type: "timestamp without time zone",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldNullable: true);

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "StrikeDate",
            table: "Strike",
            type: "timestamp without time zone",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldNullable: true);

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "Start",
            table: "Season",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "LastUpdated",
            table: "Member",
            type: "timestamp without time zone",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldNullable: true);
    }
}
