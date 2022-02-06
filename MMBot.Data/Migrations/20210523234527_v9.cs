using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MMBot.Data.Migrations;

public partial class v9 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_BossRaid_Clan_ClanId",
            table: "BossRaid");

        migrationBuilder.DropForeignKey(
            name: "FK_BossRaider_BossRaid_RaidParticipationId",
            table: "BossRaider");

        migrationBuilder.DropForeignKey(
            name: "FK_BossRaider_Member_RaidParticipationId",
            table: "BossRaider");

        migrationBuilder.DropTable(
            name: "MemberSeason");

        migrationBuilder.DropIndex(
            name: "IX_GuildSettings_GuildId",
            table: "GuildSettings");

        migrationBuilder.DropPrimaryKey(
            name: "PK_BossRaider",
            table: "BossRaider");

        migrationBuilder.DropPrimaryKey(
            name: "PK_BossRaid",
            table: "BossRaid");

        migrationBuilder.DropColumn(
            name: "Donations",
            table: "Season");

        migrationBuilder.DropColumn(
            name: "GuildId",
            table: "Season");

        migrationBuilder.DropColumn(
            name: "SHigh",
            table: "Season");

        migrationBuilder.RenameTable(
            name: "BossRaider",
            newName: "RaidParticipation");

        migrationBuilder.RenameTable(
            name: "BossRaid",
            newName: "RaidBoss");

        migrationBuilder.RenameIndex(
            name: "IX_BossRaider_RaidParticipationId",
            table: "RaidParticipation",
            newName: "IX_RaidParticipation_RaidParticipationId");

        migrationBuilder.RenameIndex(
            name: "IX_BossRaid_ClanId",
            table: "RaidBoss",
            newName: "IX_RaidBoss_ClanId");

        migrationBuilder.AddColumn<int>(
            name: "RaidBossId",
            table: "RaidParticipation",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddPrimaryKey(
            name: "PK_RaidParticipation",
            table: "RaidParticipation",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_RaidBoss",
            table: "RaidBoss",
            column: "Id");

        migrationBuilder.CreateTable(
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
                table.PrimaryKey("PK_SeasonResult", x => x.Id);
                table.ForeignKey(
                    name: "FK_SeasonResult_Member_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Member",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_SeasonResult_Season_SeasonId",
                    column: x => x.SeasonId,
                    principalTable: "Season",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SeasonResult_MemberId",
            table: "SeasonResult",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_SeasonResult_SeasonId",
            table: "SeasonResult",
            column: "SeasonId");

        migrationBuilder.AddForeignKey(
            name: "FK_RaidBoss_Clan_ClanId",
            table: "RaidBoss",
            column: "ClanId",
            principalTable: "Clan",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_RaidParticipation_Member_RaidParticipationId",
            table: "RaidParticipation",
            column: "RaidParticipationId",
            principalTable: "Member",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_RaidParticipation_RaidBoss_RaidParticipationId",
            table: "RaidParticipation",
            column: "RaidParticipationId",
            principalTable: "RaidBoss",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_RaidBoss_Clan_ClanId",
            table: "RaidBoss");

        migrationBuilder.DropForeignKey(
            name: "FK_RaidParticipation_Member_RaidParticipationId",
            table: "RaidParticipation");

        migrationBuilder.DropForeignKey(
            name: "FK_RaidParticipation_RaidBoss_RaidParticipationId",
            table: "RaidParticipation");

        migrationBuilder.DropTable(
            name: "SeasonResult");

        migrationBuilder.DropPrimaryKey(
            name: "PK_RaidParticipation",
            table: "RaidParticipation");

        migrationBuilder.DropPrimaryKey(
            name: "PK_RaidBoss",
            table: "RaidBoss");

        migrationBuilder.DropColumn(
            name: "RaidBossId",
            table: "RaidParticipation");

        migrationBuilder.RenameTable(
            name: "RaidParticipation",
            newName: "BossRaider");

        migrationBuilder.RenameTable(
            name: "RaidBoss",
            newName: "BossRaid");

        migrationBuilder.RenameIndex(
            name: "IX_RaidParticipation_RaidParticipationId",
            table: "BossRaider",
            newName: "IX_BossRaider_RaidParticipationId");

        migrationBuilder.RenameIndex(
            name: "IX_RaidBoss_ClanId",
            table: "BossRaid",
            newName: "IX_BossRaid_ClanId");

        migrationBuilder.AddColumn<int>(
            name: "Donations",
            table: "Season",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "GuildId",
            table: "Season",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<int>(
            name: "SHigh",
            table: "Season",
            type: "integer",
            nullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_BossRaider",
            table: "BossRaider",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_BossRaid",
            table: "BossRaid",
            column: "Id");

        migrationBuilder.CreateTable(
            name: "MemberSeason",
            columns: table => new
            {
                MemberId = table.Column<int>(type: "integer", nullable: false),
                SeasonId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemberSeason", x => new { x.MemberId, x.SeasonId });
                table.ForeignKey(
                    name: "FK_MemberSeason_Member_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Member",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_MemberSeason_Season_SeasonId",
                    column: x => x.SeasonId,
                    principalTable: "Season",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_GuildSettings_GuildId",
            table: "GuildSettings",
            column: "GuildId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_MemberSeason_SeasonId",
            table: "MemberSeason",
            column: "SeasonId");

        migrationBuilder.AddForeignKey(
            name: "FK_BossRaid_Clan_ClanId",
            table: "BossRaid",
            column: "ClanId",
            principalTable: "Clan",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_BossRaider_BossRaid_RaidParticipationId",
            table: "BossRaider",
            column: "RaidParticipationId",
            principalTable: "BossRaid",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_BossRaider_Member_RaidParticipationId",
            table: "BossRaider",
            column: "RaidParticipationId",
            principalTable: "Member",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
