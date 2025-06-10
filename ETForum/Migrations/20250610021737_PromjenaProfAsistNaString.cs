using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class PromjenaProfAsistNaString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Predmeti_Korisnik_asistentId",
                table: "Predmeti");

            migrationBuilder.DropForeignKey(
                name: "FK_Predmeti_Korisnik_profesorId",
                table: "Predmeti");

            migrationBuilder.DropIndex(
                name: "IX_Predmeti_asistentId",
                table: "Predmeti");

            migrationBuilder.DropIndex(
                name: "IX_Predmeti_profesorId",
                table: "Predmeti");

            migrationBuilder.DropColumn(
                name: "asistentId",
                table: "Predmeti");

            migrationBuilder.DropColumn(
                name: "profesorId",
                table: "Predmeti");

            migrationBuilder.AddColumn<string>(
                name: "asistentImePrezime",
                table: "Predmeti",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profesorImePrezime",
                table: "Predmeti",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asistentImePrezime",
                table: "Predmeti");

            migrationBuilder.DropColumn(
                name: "profesorImePrezime",
                table: "Predmeti");

            migrationBuilder.AddColumn<string>(
                name: "asistentId",
                table: "Predmeti",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profesorId",
                table: "Predmeti",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Predmeti_asistentId",
                table: "Predmeti",
                column: "asistentId");

            migrationBuilder.CreateIndex(
                name: "IX_Predmeti_profesorId",
                table: "Predmeti",
                column: "profesorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Predmeti_Korisnik_asistentId",
                table: "Predmeti",
                column: "asistentId",
                principalTable: "Korisnik",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Predmeti_Korisnik_profesorId",
                table: "Predmeti",
                column: "profesorId",
                principalTable: "Korisnik",
                principalColumn: "Id");
        }
    }
}
