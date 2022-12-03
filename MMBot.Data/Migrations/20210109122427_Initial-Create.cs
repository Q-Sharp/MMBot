using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MMBot.Data.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
         migrationBuilder.CreateTable(
            name: "Channel",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                TextChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                AnswerTextChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_Channel", x => x.Id);
            });

         migrationBuilder.CreateTable(
            name: "Clan",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SortOrder = table.Column<int>(type: "integer", nullable: false),
                Tag = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                DiscordRole = table.Column<string>(type: "text", nullable: true),
                GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_Clan", x => x.Id);
            });

         migrationBuilder.CreateTable(
            name: "GuildSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                Prefix = table.Column<string>(type: "text", nullable: true),
                WaitForReaction = table.Column<TimeSpan>(type: "interval", nullable: false),
                FileName = table.Column<string>(type: "text", nullable: true),
                ClanSize = table.Column<int>(type: "integer", nullable: false),
                MemberMovementQty = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_GuildSettings", x => x.Id);
            });

         migrationBuilder.CreateTable(
            name: "MemberGroup",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_MemberGroup", x => x.Id);
            });

         migrationBuilder.CreateTable(
            name: "Restart",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Guild = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                Channel = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                DBImport = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_Restart", x => x.Id);
            });

         migrationBuilder.CreateTable(
            name: "Timer",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                RingSpan = table.Column<TimeSpan>(type: "interval", nullable: true),
                EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                ChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                Message = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_Timer", x => x.Id);
            });

         migrationBuilder.CreateTable(
            name: "Member",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Discord = table.Column<string>(type: "text", nullable: true),
                AHigh = table.Column<int>(type: "integer", nullable: true),
                SHigh = table.Column<int>(type: "integer", nullable: true),
                Current = table.Column<int>(type: "integer", nullable: true),
                Donations = table.Column<int>(type: "integer", nullable: true),
                Role = table.Column<int>(type: "integer", nullable: false),
                DiscordStatus = table.Column<int>(type: "integer", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                ClanId = table.Column<int>(type: "integer", nullable: true),
                LastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                Join = table.Column<int>(type: "integer", nullable: false),
                IgnoreOnMoveUp = table.Column<bool>(type: "boolean", nullable: false),
                PlayerTag = table.Column<string>(type: "text", nullable: true),
                AutoSignUpForFightNight = table.Column<bool>(type: "boolean", nullable: false),
                GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                MemberGroupId = table.Column<int>(type: "integer", nullable: true),
                LocalTimeOffSet = table.Column<double>(type: "double precision", nullable: true)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_Member", x => x.Id);
                 table.ForeignKey(
                    name: "FK_Member_Clan_ClanId",
                    column: x => x.ClanId,
                    principalTable: "Clan",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                 table.ForeignKey(
                    name: "FK_Member_MemberGroup_MemberGroupId",
                    column: x => x.MemberGroupId,
                    principalTable: "MemberGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

         migrationBuilder.CreateTable(
            name: "Strike",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Reason = table.Column<string>(type: "text", nullable: true),
                StrikeDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                MemberId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_Strike", x => x.Id);
                 table.ForeignKey(
                    name: "FK_Strike_Member_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Member",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

         migrationBuilder.CreateTable(
            name: "Vacation",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                MemberId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                 table.PrimaryKey("PK_Vacation", x => x.Id);
                 table.ForeignKey(
                    name: "FK_Vacation_Member_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Member",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

         migrationBuilder.CreateIndex(
            name: "IX_Clan_Tag_Name_GuildId_SortOrder",
            table: "Clan",
            columns: new[] { "Tag", "Name", "GuildId", "SortOrder" },
            unique: true);

         migrationBuilder.CreateIndex(
            name: "IX_GuildSettings_GuildId",
            table: "GuildSettings",
            column: "GuildId",
            unique: true);

         migrationBuilder.CreateIndex(
            name: "IX_Member_ClanId",
            table: "Member",
            column: "ClanId");

         migrationBuilder.CreateIndex(
            name: "IX_Member_MemberGroupId",
            table: "Member",
            column: "MemberGroupId");

         migrationBuilder.CreateIndex(
            name: "IX_Strike_MemberId",
            table: "Strike",
            column: "MemberId");

         migrationBuilder.CreateIndex(
            name: "IX_Vacation_MemberId",
            table: "Vacation",
            column: "MemberId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
         migrationBuilder.DropTable(
            name: "Channel");

         migrationBuilder.DropTable(
            name: "GuildSettings");

         migrationBuilder.DropTable(
            name: "Restart");

         migrationBuilder.DropTable(
            name: "Strike");

         migrationBuilder.DropTable(
            name: "Timer");

         migrationBuilder.DropTable(
            name: "Vacation");

         migrationBuilder.DropTable(
            name: "Member");

         migrationBuilder.DropTable(
            name: "Clan");

         migrationBuilder.DropTable(
            name: "MemberGroup");
    }
}
