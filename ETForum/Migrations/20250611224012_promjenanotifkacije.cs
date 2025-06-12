using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class promjenanotifkacije : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifikacija_Korisnik_korisnikId",
                table: "Notifikacija");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifikacija_Poruka_porukaId",
                table: "Notifikacija");

            migrationBuilder.DropIndex(
                name: "IX_Notifikacija_porukaId",
                table: "Notifikacija");

            migrationBuilder.DropColumn(
                name: "porukaId",
                table: "Notifikacija");

            migrationBuilder.RenameColumn(
                name: "korisnikId",
                table: "Notifikacija",
                newName: "KorisnikId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Notifikacija",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "vrijemeKreiranja",
                table: "Notifikacija",
                newName: "Vrijeme");

            migrationBuilder.RenameColumn(
                name: "procitana",
                table: "Notifikacija",
                newName: "Procitano");

            migrationBuilder.RenameIndex(
                name: "IX_Notifikacija_korisnikId",
                table: "Notifikacija",
                newName: "IX_Notifikacija_KorisnikId");

            migrationBuilder.AlterColumn<string>(
                name: "KorisnikId",
                table: "Notifikacija",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Notifikacija",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tekst",
                table: "Notifikacija",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifikacija_Korisnik_KorisnikId",
                table: "Notifikacija",
                column: "KorisnikId",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifikacija_Korisnik_KorisnikId",
                table: "Notifikacija");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Notifikacija");

            migrationBuilder.DropColumn(
                name: "Tekst",
                table: "Notifikacija");

            migrationBuilder.RenameColumn(
                name: "KorisnikId",
                table: "Notifikacija",
                newName: "korisnikId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Notifikacija",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Vrijeme",
                table: "Notifikacija",
                newName: "vrijemeKreiranja");

            migrationBuilder.RenameColumn(
                name: "Procitano",
                table: "Notifikacija",
                newName: "procitana");

            migrationBuilder.RenameIndex(
                name: "IX_Notifikacija_KorisnikId",
                table: "Notifikacija",
                newName: "IX_Notifikacija_korisnikId");

            migrationBuilder.AlterColumn<string>(
                name: "korisnikId",
                table: "Notifikacija",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "porukaId",
                table: "Notifikacija",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifikacija_porukaId",
                table: "Notifikacija",
                column: "porukaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifikacija_Korisnik_korisnikId",
                table: "Notifikacija",
                column: "korisnikId",
                principalTable: "Korisnik",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifikacija_Poruka_porukaId",
                table: "Notifikacija",
                column: "porukaId",
                principalTable: "Poruka",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
