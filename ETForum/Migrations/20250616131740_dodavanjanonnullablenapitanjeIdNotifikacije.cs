using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class dodavanjanonnullablenapitanjeIdNotifikacije : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifikacija_Pitanje_pitanjeId",
                table: "Notifikacija");

            migrationBuilder.AlterColumn<int>(
                name: "pitanjeId",
                table: "Notifikacija",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifikacija_Pitanje_pitanjeId",
                table: "Notifikacija",
                column: "pitanjeId",
                principalTable: "Pitanje",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifikacija_Pitanje_pitanjeId",
                table: "Notifikacija");

            migrationBuilder.AlterColumn<int>(
                name: "pitanjeId",
                table: "Notifikacija",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifikacija_Pitanje_pitanjeId",
                table: "Notifikacija",
                column: "pitanjeId",
                principalTable: "Pitanje",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
