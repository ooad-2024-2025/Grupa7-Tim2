using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class pitanjeIdNotifikacija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "pitanjeId",
                table: "Notifikacija",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pitanjeId",
                table: "Notifikacija");
        }
    }
}
