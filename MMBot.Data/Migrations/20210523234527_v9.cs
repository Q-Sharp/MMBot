using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MMBot.Data.Migrations;

public partial class v9 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_BossRaid_Clan_ClanId",
            table: "BossRaid");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_BossRaider_BossRaid_RaidParticipationId",
            table: "BossRaider");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_BossRaider_Member_RaidParticipationId",
            table: "BossRaider");

        _ = migrationBuilder.DropTable(
            name: "MemberSeason");

        _ = migrationBuilder.DropIndex(
            name: "IX_GuildSettings_GuildId",
            table: "GuildSettings");

        _ = migrationBuilder.DropPrimaryKey(
            name: "PK_BossRaider",
            table: "BossRaider");

        _ = migrationBuilder.DropPrimaryKey(
            name: "PK_BossRaid",
            table: "BossRaid");

        _ = migrationBuilder.DropColumn(
            name: "Donations",
            table: "Season");

        _ = migrationBuilder.DropColumn(
            name: "GuildId",
            table: "Season");

        _ = migrationBuilder.DropColumn(
            name: "SHigh",
            table: "Season");

        _ = migrationBuilder.RenameTable(
            name: "BossRaider",
            newName: "RaidParticipation");

        _ = migrationBuilder.RenameTable(
            name: "BossRaid",
            newName: "RaidBoss");

        _ = migrationBuilder.RenameIndex(
            name: "IX_BossRaider_RaidParticipationId",
            table: "RaidParticipation",
            newName: "IX_RaidParticipation_RaidParticipationId");

        _ = migrationBuilder.RenameIndex(
            name: "IX_BossRaid_ClanId",
            table: "RaidBoss",
            newName: "IX_RaidBoss_ClanId");

        _ = migrationBuilder.AddColumn<int>(
            name: "RaidBossId",
            table: "RaidParticipation",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        _ = migrationBuilder.AddPrimaryKey(
            name: "PK_RaidParticipation",
            table: "RaidParticipation",
            column: "Id");

        _ = migrationBuilder.AddPrimaryKey(
            name: "PK_RaidBoss",
            table: "RaidBoss",
            column: "Id");

        _ = migrationBuilder.CreateTable(
            name: "SeasonResult",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                No = table.Column<int>(type: "integer", nullable: false),
                GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                SeasonId = table.Column<int>(type: "integer", nullable: false),
                SHigh = table.Column<int>(type: "integer", nullable: true),
                Donations = table.Column<int>(type: "integer", nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_SeasonResult", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_SeasonResult_Member_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Member",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                _ = table.ForeignKey(
                    name: "FK_SeasonResult_Season_SeasonId",
                    column: x => x.SeasonId,
                    principalTable: "Season",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_SeasonResult_MemberId",
            table: "SeasonResult",
            column: "MemberId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_SeasonResult_SeasonId",
            table: "SeasonResult",
            column: "SeasonId");

        _ = migrationBuilder.AddForeignKey(
            name: "FK_RaidBoss_Clan_ClanId",
            table: "RaidBoss",
            column: "ClanId",
            principalTable: "Clan",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_RaidParticipation_Member_RaidParticipationId",
            table: "RaidParticipation",
            column: "RaidParticipationId",
            principalTable: "Member",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_RaidParticipation_RaidBoss_RaidParticipationId",
            table: "RaidParticipation",
            column: "RaidParticipationId",
            principalTable: "RaidBoss",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_RaidBoss_Clan_ClanId",
            table: "RaidBoss");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_RaidParticipation_Member_RaidParticipationId",
            table: "RaidParticipation");

        _ = migrationBuilder.DropForeignKey(
            name: "FK_RaidParticipation_RaidBoss_RaidParticipationId",
            table: "RaidParticipation");

        _ = migrationBuilder.DropTable(
            name: "SeasonResult");

        _ = migrationBuilder.DropPrimaryKey(
            name: "PK_RaidParticipation",
            table: "RaidParticipation");

        _ = migrationBuilder.DropPrimaryKey(
            name: "PK_RaidBoss",
            table: "RaidBoss");

        _ = migrationBuilder.DropColumn(
            name: "RaidBossId",
            table: "RaidParticipation");

        _ = migrationBuilder.RenameTable(
            name: "RaidParticipation",
            newName: "BossRaider");

        _ = migrationBuilder.RenameTable(
            name: "RaidBoss",
            newName: "BossRaid");

        _ = migrationBuilder.RenameIndex(
            name: "IX_RaidParticipation_RaidParticipationId",
            table: "BossRaider",
            newName: "IX_BossRaider_RaidParticipationId");

        _ = migrationBuilder.RenameIndex(
            name: "IX_RaidBoss_ClanId",
            table: "BossRaid",
            newName: "IX_BossRaid_ClanId");

        _ = migrationBuilder.AddColumn<int>(
            name: "Donations",
            table: "Season",
            type: "integer",
            nullable: true);

        _ = migrationBuilder.AddColumn<decimal>(
            name: "GuildId",
            table: "Season",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m);

        _ = migrationBuilder.AddColumn<int>(
            name: "SHigh",
            table: "Season",
            type: "integer",
            nullable: true);

        _ = migrationBuilder.AddPrimaryKey(
            name: "PK_BossRaider",
            table: "BossRaider",
            column: "Id");

        _ = migrationBuilder.AddPrimaryKey(
            name: "PK_BossRaid",
            table: "BossRaid",
            column: "Id");

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
            name: "IX_GuildSettings_GuildId",
            table: "GuildSettings",
            column: "GuildId",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_MemberSeason_SeasonId",
            table: "MemberSeason",
            column: "SeasonId");

        _ = migrationBuilder.AddForeignKey(
            name: "FK_BossRaid_Clan_ClanId",
            table: "BossRaid",
            column: "ClanId",
            principalTable: "Clan",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_BossRaider_BossRaid_RaidParticipationId",
            table: "BossRaider",
            column: "RaidParticipationId",
            principalTable: "BossRaid",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        _ = migrationBuilder.AddForeignKey(
            name: "FK_BossRaider_Member_RaidParticipationId",
            table: "BossRaider",
            column: "RaidParticipationId",
            principalTable: "Member",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
