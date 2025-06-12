using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class TBJ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dostignuce_Korisnik_korisnikId",
                table: "Dostignuce");

            migrationBuilder.DropIndex(
                name: "IX_Dostignuce_korisnikId",
                table: "Dostignuce");

            migrationBuilder.DropColumn(
                name: "korisnikId",
                table: "Dostignuce");

            migrationBuilder.AlterColumn<string>(
                name: "opis",
                table: "Dostignuce",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "naziv",
                table: "Dostignuce",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "KorisnikDostignuca",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    korisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    dostignuceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikDostignuca", x => x.id);
                    table.ForeignKey(
                        name: "FK_KorisnikDostignuca_Dostignuce_dostignuceId",
                        column: x => x.dostignuceId,
                        principalTable: "Dostignuce",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorisnikDostignuca_Korisnik_korisnikId",
                        column: x => x.korisnikId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikDostignuca_dostignuceId",
                table: "KorisnikDostignuca",
                column: "dostignuceId");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikDostignuca_korisnikId",
                table: "KorisnikDostignuca",
                column: "korisnikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KorisnikDostignuca");

            migrationBuilder.AlterColumn<string>(
                name: "opis",
                table: "Dostignuce",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "naziv",
                table: "Dostignuce",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "korisnikId",
                table: "Dostignuce",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dostignuce_korisnikId",
                table: "Dostignuce",
                column: "korisnikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dostignuce_Korisnik_korisnikId",
                table: "Dostignuce",
                column: "korisnikId",
                principalTable: "Korisnik",
                principalColumn: "Id");
        }
    }
}
