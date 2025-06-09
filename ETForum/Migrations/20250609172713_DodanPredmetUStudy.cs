using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class DodanPredmetUStudy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "predmetId",
                table: "StudySession",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudySession_predmetId",
                table: "StudySession",
                column: "predmetId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySession_Predmeti_predmetId",
                table: "StudySession",
                column: "predmetId",
                principalTable: "Predmeti",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySession_Predmeti_predmetId",
                table: "StudySession");

            migrationBuilder.DropIndex(
                name: "IX_StudySession_predmetId",
                table: "StudySession");

            migrationBuilder.DropColumn(
                name: "predmetId",
                table: "StudySession");
        }
    }
}
