using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentManager3000.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Rounds_RoundId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Tournaments_TournamentId",
                table: "Rounds");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentId",
                table: "Rounds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoundId",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Rounds_RoundId",
                table: "Matches",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Tournaments_TournamentId",
                table: "Rounds",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Rounds_RoundId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Tournaments_TournamentId",
                table: "Rounds");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentId",
                table: "Rounds",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "RoundId",
                table: "Matches",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Rounds_RoundId",
                table: "Matches",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Tournaments_TournamentId",
                table: "Rounds",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id");
        }
    }
}
