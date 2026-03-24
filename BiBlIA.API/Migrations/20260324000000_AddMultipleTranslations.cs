using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiBlIA.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adiciona TextAA (Almeida Revisada) logo após TextKJV
            migrationBuilder.AddColumn<string>(
                name: "TextAA",
                table: "BibleVerses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            // Adiciona TextNVI (Nova Versão Internacional)
            migrationBuilder.AddColumn<string>(
                name: "TextNVI",
                table: "BibleVerses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextAA",
                table: "BibleVerses");

            migrationBuilder.DropColumn(
                name: "TextNVI",
                table: "BibleVerses");
        }
    }
}
