using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_BiologyEducation.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToAccessTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessBiologies",
                columns: table => new
                {
                    AccessBiology_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Oject_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VisitedNumber = table.Column<int>(type: "integer", nullable: false),
                    AccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessBiologies", x => x.AccessBiology_Id);
                    table.ForeignKey(
                        name: "FK_AccessBiologies_Plant_Biology_Animals_Oject_Id",
                        column: x => x.Oject_Id,
                        principalTable: "Plant_Biology_Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessBiologies_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessBooks",
                columns: table => new
                {
                    AccessBook_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Book_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VisitedNumber = table.Column<int>(type: "integer", nullable: false),
                    AccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessBooks", x => x.AccessBook_Id);
                    table.ForeignKey(
                        name: "FK_AccessBooks_Books_Book_Id",
                        column: x => x.Book_Id,
                        principalTable: "Books",
                        principalColumn: "Book_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessBooks_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessLessons",
                columns: table => new
                {
                    AccessLesson_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Lesson_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VisitedNumber = table.Column<int>(type: "integer", nullable: false),
                    AccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLessons", x => x.AccessLesson_Id);
                    table.ForeignKey(
                        name: "FK_AccessLessons_Lessons_Lesson_Id",
                        column: x => x.Lesson_Id,
                        principalTable: "Lessons",
                        principalColumn: "Lesson_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessLessons_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessBiologies_Oject_Id",
                table: "AccessBiologies",
                column: "Oject_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccessBiologies_User_Id",
                table: "AccessBiologies",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccessBooks_Book_Id",
                table: "AccessBooks",
                column: "Book_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccessBooks_User_Id",
                table: "AccessBooks",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLessons_Lesson_Id",
                table: "AccessLessons",
                column: "Lesson_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLessons_User_Id",
                table: "AccessLessons",
                column: "User_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessBiologies");

            migrationBuilder.DropTable(
                name: "AccessBooks");

            migrationBuilder.DropTable(
                name: "AccessLessons");
        }
    }
}
