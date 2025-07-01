using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_BiologyEducation.Migrations
{
    /// <inheritdoc />
    public partial class RemoveManageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManageBooks");

            migrationBuilder.DropTable(
                name: "ManageChapters");

            migrationBuilder.DropTable(
                name: "ManageLessons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManageBooks",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Book_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageBooks", x => new { x.User_Id, x.Book_Id });
                    table.ForeignKey(
                        name: "FK_ManageBooks_Books_Book_Id",
                        column: x => x.Book_Id,
                        principalTable: "Books",
                        principalColumn: "Book_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManageBooks_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManageChapters",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Chapter_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageChapters", x => new { x.User_Id, x.Chapter_Id });
                    table.ForeignKey(
                        name: "FK_ManageChapters_Chapters_Chapter_Id",
                        column: x => x.Chapter_Id,
                        principalTable: "Chapters",
                        principalColumn: "Chapter_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManageChapters_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManageLessons",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Lesson_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageLessons", x => new { x.User_Id, x.Lesson_Id });
                    table.ForeignKey(
                        name: "FK_ManageLessons_Lessons_Lesson_Id",
                        column: x => x.Lesson_Id,
                        principalTable: "Lessons",
                        principalColumn: "Lesson_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManageLessons_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManageBooks_Book_Id",
                table: "ManageBooks",
                column: "Book_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ManageChapters_Chapter_Id",
                table: "ManageChapters",
                column: "Chapter_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ManageLessons_Lesson_Id",
                table: "ManageLessons",
                column: "Lesson_Id");
        }
    }
}
