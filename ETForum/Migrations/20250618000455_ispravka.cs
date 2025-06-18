using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETForum.Migrations
{
    /// <inheritdoc />
    public partial class ispravka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifKomentari",
                table: "Korisnik");

            migrationBuilder.DropColumn(
                name: "NotifPoruke",
                table: "Korisnik");

            migrationBuilder.DropColumn(
                name: "NotifPrijateljstva",
                table: "Korisnik");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotifKomentari",
                table: "Korisnik",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifPoruke",
                table: "Korisnik",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifPrijateljstva",
                table: "Korisnik",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
