using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiBlIA.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProgressByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProgress_SessionId",
                table: "UserProgress");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "UserProgress");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "UserProgress",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserProgress",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_UserId_ModuleId",
                table: "UserProgress",
                columns: new[] { "UserId", "ModuleId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgress_Users_UserId",
                table: "UserProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProgress_Users_UserId",
                table: "UserProgress");

            migrationBuilder.DropIndex(
                name: "IX_UserProgress_UserId_ModuleId",
                table: "UserProgress");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "UserProgress");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserProgress");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "UserProgress",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_SessionId",
                table: "UserProgress",
                column: "SessionId");
        }
    }
}
