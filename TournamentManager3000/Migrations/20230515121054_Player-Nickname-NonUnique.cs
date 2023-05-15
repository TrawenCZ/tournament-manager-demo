using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentManager3000.Migrations
{
    /// <inheritdoc />
    public partial class PlayerNicknameNonUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_Nickname",
                table: "Players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Players_Nickname",
                table: "Players",
                column: "Nickname",
                unique: true);
        }
    }
}
