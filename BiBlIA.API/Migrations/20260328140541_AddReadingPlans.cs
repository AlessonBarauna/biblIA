using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiBlIA.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReadingPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReadingPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TotalDays = table.Column<int>(type: "INTEGER", nullable: false),
                    Strategy = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReadingLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    DayNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadingLogs_ReadingPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "ReadingPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReadingLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadingLogs_PlanId",
                table: "ReadingLogs",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingLogs_UserId_PlanId_DayNumber",
                table: "ReadingLogs",
                columns: new[] { "UserId", "PlanId", "DayNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadingLogs");

            migrationBuilder.DropTable(
                name: "ReadingPlans");
        }
    }
}
