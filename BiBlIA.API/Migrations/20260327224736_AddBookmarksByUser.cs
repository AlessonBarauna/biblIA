using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiBlIA.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBookmarksByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookmarkVerses_SessionId",
                table: "BookmarkVerses");

            migrationBuilder.DropColumn(
                name: "Book",
                table: "BookmarkVerses");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "BookmarkVerses");

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "BookmarkVerses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BookmarkVerses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkVerses_BookId",
                table: "BookmarkVerses",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkVerses_UserId",
                table: "BookmarkVerses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookmarkVerses_BibleBooks_BookId",
                table: "BookmarkVerses",
                column: "BookId",
                principalTable: "BibleBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookmarkVerses_Users_UserId",
                table: "BookmarkVerses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookmarkVerses_BibleBooks_BookId",
                table: "BookmarkVerses");

            migrationBuilder.DropForeignKey(
                name: "FK_BookmarkVerses_Users_UserId",
                table: "BookmarkVerses");

            migrationBuilder.DropIndex(
                name: "IX_BookmarkVerses_BookId",
                table: "BookmarkVerses");

            migrationBuilder.DropIndex(
                name: "IX_BookmarkVerses_UserId",
                table: "BookmarkVerses");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BookmarkVerses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BookmarkVerses");

            migrationBuilder.AddColumn<string>(
                name: "Book",
                table: "BookmarkVerses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "BookmarkVerses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkVerses_SessionId",
                table: "BookmarkVerses",
                column: "SessionId");
        }
    }
}
