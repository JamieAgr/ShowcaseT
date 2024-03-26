using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowcaseP2.Migrations
{
    /// <inheritdoc />
    public partial class matchdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchesLost",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchesPlayer",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchesWon",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchesLost",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MatchesPlayer",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MatchesWon",
                table: "AspNetUsers");
        }
    }
}
