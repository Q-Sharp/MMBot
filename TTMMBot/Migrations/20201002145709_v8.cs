using Microsoft.EntityFrameworkCore.Migrations;

namespace TTMMBot.Migrations
{
    public partial class v8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Channel",
                columns: table => new
                {
                    ChannelId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(nullable: false),
                    TextChannelId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channel", x => x.ChannelId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channel");
        }
    }
}
