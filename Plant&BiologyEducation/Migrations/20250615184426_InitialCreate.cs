using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_BiologyEducation.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Book_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Book_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cover_img = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Book_Id);
                });

            migrationBuilder.CreateTable(
                name: "Plant_Biology_Animals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailsInformation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plant_Biology_Animals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Chapter_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chapter_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Book_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Book_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Chapter_Id);
                    table.ForeignKey(
                        name: "FK_Chapters_Books_Book_Id1",
                        column: x => x.Book_Id1,
                        principalTable: "Books",
                        principalColumn: "Book_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessBookHistories",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Book_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Book_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Visited_Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessBookHistories", x => new { x.User_Id, x.Book_Id });
                    table.ForeignKey(
                        name: "FK_AccessBookHistories_Books_Book_Id1",
                        column: x => x.Book_Id1,
                        principalTable: "Books",
                        principalColumn: "Book_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessBookHistories_Users_User_Id1",
                        column: x => x.User_Id1,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManageBooks",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Book_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Book_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageBooks", x => new { x.User_Id, x.Book_Id });
                    table.ForeignKey(
                        name: "FK_ManageBooks_Books_Book_Id1",
                        column: x => x.Book_Id1,
                        principalTable: "Books",
                        principalColumn: "Book_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManageBooks_Users_User_Id1",
                        column: x => x.User_Id1,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Lesson_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Lesson_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Chapter_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chapter_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Lesson_Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Chapters_Chapter_Id1",
                        column: x => x.Chapter_Id1,
                        principalTable: "Chapters",
                        principalColumn: "Chapter_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManageChapters",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chapter_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chapter_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageChapters", x => new { x.User_Id, x.Chapter_Id });
                    table.ForeignKey(
                        name: "FK_ManageChapters_Chapters_Chapter_Id1",
                        column: x => x.Chapter_Id1,
                        principalTable: "Chapters",
                        principalColumn: "Chapter_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManageChapters_Users_User_Id1",
                        column: x => x.User_Id1,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessLessonHistories",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Lesson_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Lesson_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Visited_Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLessonHistories", x => new { x.User_Id, x.Lesson_Id });
                    table.ForeignKey(
                        name: "FK_AccessLessonHistories_Lessons_Lesson_Id1",
                        column: x => x.Lesson_Id1,
                        principalTable: "Lessons",
                        principalColumn: "Lesson_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessLessonHistories_Users_User_Id1",
                        column: x => x.User_Id1,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManageLessons",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Lesson_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Lesson_Id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageLessons", x => new { x.User_Id, x.Lesson_Id });
                    table.ForeignKey(
                        name: "FK_ManageLessons_Lessons_Lesson_Id1",
                        column: x => x.Lesson_Id1,
                        principalTable: "Lessons",
                        principalColumn: "Lesson_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManageLessons_Users_User_Id1",
                        column: x => x.User_Id1,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessBookHistories_Book_Id1",
                table: "AccessBookHistories",
                column: "Book_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_AccessBookHistories_User_Id1",
                table: "AccessBookHistories",
                column: "User_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLessonHistories_Lesson_Id1",
                table: "AccessLessonHistories",
                column: "Lesson_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLessonHistories_User_Id1",
                table: "AccessLessonHistories",
                column: "User_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_Book_Id1",
                table: "Chapters",
                column: "Book_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Chapter_Id1",
                table: "Lessons",
                column: "Chapter_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_ManageBooks_Book_Id1",
                table: "ManageBooks",
                column: "Book_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_ManageBooks_User_Id1",
                table: "ManageBooks",
                column: "User_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_ManageChapters_Chapter_Id1",
                table: "ManageChapters",
                column: "Chapter_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_ManageChapters_User_Id1",
                table: "ManageChapters",
                column: "User_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_ManageLessons_Lesson_Id1",
                table: "ManageLessons",
                column: "Lesson_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_ManageLessons_User_Id1",
                table: "ManageLessons",
                column: "User_Id1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessBookHistories");

            migrationBuilder.DropTable(
                name: "AccessLessonHistories");

            migrationBuilder.DropTable(
                name: "ManageBooks");

            migrationBuilder.DropTable(
                name: "ManageChapters");

            migrationBuilder.DropTable(
                name: "ManageLessons");

            migrationBuilder.DropTable(
                name: "Plant_Biology_Animals");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
