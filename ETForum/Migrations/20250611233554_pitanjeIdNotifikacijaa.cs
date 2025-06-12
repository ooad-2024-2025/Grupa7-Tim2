using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class pitanjeIdNotifikacijaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notifikacija_pitanjeId",
                table: "Notifikacija",
                column: "pitanjeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifikacija_Pitanje_pitanjeId",
                table: "Notifikacija",
                column: "pitanjeId",
                principalTable: "Pitanje",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifikacija_Pitanje_pitanjeId",
                table: "Notifikacija");

            migrationBuilder.DropIndex(
                name: "IX_Notifikacija_pitanjeId",
                table: "Notifikacija");
        }
    }
}
