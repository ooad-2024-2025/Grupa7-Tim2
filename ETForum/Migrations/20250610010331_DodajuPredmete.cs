using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class DodajuPredmete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Smjer",
                table: "Predmeti",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Smjer",
                table: "Predmeti");
        }
    }
}
