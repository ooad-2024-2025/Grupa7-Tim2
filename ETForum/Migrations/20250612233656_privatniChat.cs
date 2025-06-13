using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class privatniChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivatniChatovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    posiljalacId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    primaocId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    poruka = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vrijeme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    procitano = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivatniChatovi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivatniChatovi_Korisnik_posiljalacId",
                        column: x => x.posiljalacId,
                        principalTable: "Korisnik",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrivatniChatovi_Korisnik_primaocId",
                        column: x => x.primaocId,
                        principalTable: "Korisnik",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivatniChatovi_posiljalacId",
                table: "PrivatniChatovi",
                column: "posiljalacId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivatniChatovi_primaocId",
                table: "PrivatniChatovi",
                column: "primaocId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivatniChatovi");
        }
    }
}
