using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MMBot.Data.Migrations;

public partial class v6 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "Donations",
            table: "Member");

        _ = migrationBuilder.DropColumn(
            name: "SHigh",
            table: "Member");

        _ = migrationBuilder.CreateTable(
            name: "Season",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                No = table.Column<int>(type: "integer", nullable: false),
                Era = table.Column<byte>(type: "smallint", nullable: false),
                Start = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                SHigh = table.Column<int>(type: "integer", nullable: true),
                Donations = table.Column<int>(type: "integer", nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Season", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "MemberSeason",
            columns: table => new
            {
                MemberId = table.Column<int>(type: "integer", nullable: false),
                SeasonId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_MemberSeason", x => new { x.MemberId, x.SeasonId });
                _ = table.ForeignKey(
                    name: "FK_MemberSeason_Member_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Member",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_MemberSeason_Season_SeasonId",
                    column: x => x.SeasonId,
                    principalTable: "Season",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_MemberSeason_SeasonId",
            table: "MemberSeason",
            column: "SeasonId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "MemberSeason");

        _ = migrationBuilder.DropTable(
            name: "Season");

        _ = migrationBuilder.AddColumn<int>(
            name: "Donations",
            table: "Member",
            type: "integer",
            nullable: true);

        _ = migrationBuilder.AddColumn<int>(
            name: "SHigh",
            table: "Member",
            type: "integer",
            nullable: true);
    }
}
