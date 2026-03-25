using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiBlIA.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalUrlAndBibleStudyNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalUrl",
                table: "TheologyCourses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "TheologyCourses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BibleStudyNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    Chapter = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Context = table.Column<string>(type: "TEXT", nullable: false),
                    TheologicalSignificance = table.Column<string>(type: "TEXT", nullable: false),
                    KeyThemes = table.Column<string>(type: "TEXT", nullable: false),
                    CrossReferences = table.Column<string>(type: "TEXT", nullable: false),
                    Commentary = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorNote = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibleStudyNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BibleStudyNotes_BibleBooks_BookId",
                        column: x => x.BookId,
                        principalTable: "BibleBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BibleStudyNotes_BookId_Chapter",
                table: "BibleStudyNotes",
                columns: new[] { "BookId", "Chapter" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BibleStudyNotes");

            migrationBuilder.DropColumn(
                name: "ExternalUrl",
                table: "TheologyCourses");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "TheologyCourses");
        }
    }
}
