using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class Lajkovi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tekst",
                table: "Pitanje",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "naslov",
                table: "Pitanje",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "predmetId",
                table: "Pitanje",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "tekst",
                table: "Odgovor",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "OdgovorLajk",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    korisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    odgovorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdgovorLajk", x => x.id);
                    table.ForeignKey(
                        name: "FK_OdgovorLajk_Korisnik_korisnikId",
                        column: x => x.korisnikId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OdgovorLajk_Odgovor_odgovorId",
                        column: x => x.odgovorId,
                        principalTable: "Odgovor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PitanjeLajk",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    korisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    pitanjeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PitanjeLajk", x => x.id);
                    table.ForeignKey(
                        name: "FK_PitanjeLajk_Korisnik_korisnikId",
                        column: x => x.korisnikId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PitanjeLajk_Pitanje_pitanjeId",
                        column: x => x.pitanjeId,
                        principalTable: "Pitanje",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pitanje_predmetId",
                table: "Pitanje",
                column: "predmetId");

            migrationBuilder.CreateIndex(
                name: "IX_OdgovorLajk_korisnikId_odgovorId",
                table: "OdgovorLajk",
                columns: new[] { "korisnikId", "odgovorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OdgovorLajk_odgovorId",
                table: "OdgovorLajk",
                column: "odgovorId");

            migrationBuilder.CreateIndex(
                name: "IX_PitanjeLajk_korisnikId_pitanjeId",
                table: "PitanjeLajk",
                columns: new[] { "korisnikId", "pitanjeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PitanjeLajk_pitanjeId",
                table: "PitanjeLajk",
                column: "pitanjeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pitanje_Predmeti_predmetId",
                table: "Pitanje",
                column: "predmetId",
                principalTable: "Predmeti",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pitanje_Predmeti_predmetId",
                table: "Pitanje");

            migrationBuilder.DropTable(
                name: "OdgovorLajk");

            migrationBuilder.DropTable(
                name: "PitanjeLajk");

            migrationBuilder.DropIndex(
                name: "IX_Pitanje_predmetId",
                table: "Pitanje");

            migrationBuilder.DropColumn(
                name: "naslov",
                table: "Pitanje");

            migrationBuilder.DropColumn(
                name: "predmetId",
                table: "Pitanje");

            migrationBuilder.AlterColumn<string>(
                name: "tekst",
                table: "Pitanje",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "tekst",
                table: "Odgovor",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);
        }
    }
}
