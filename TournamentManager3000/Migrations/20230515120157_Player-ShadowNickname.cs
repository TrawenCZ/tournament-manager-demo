using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentManager3000.Migrations
{
    /// <inheritdoc />
    public partial class PlayerShadowNickname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShadowNickname",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShadowNickname",
                table: "Players");
        }
    }
}
