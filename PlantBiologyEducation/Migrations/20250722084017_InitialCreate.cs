using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantBiologyEducation.Migrations
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
                    Book_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Book_Title = table.Column<string>(type: "text", nullable: false),
                    Cover_img = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Book_Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Account = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    resetToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Chapter_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Chapter_Title = table.Column<string>(type: "text", nullable: false),
                    Book_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Chapter_Id);
                    table.ForeignKey(
                        name: "FK_Chapters_Books_Book_Id",
                        column: x => x.Book_Id,
                        principalTable: "Books",
                        principalColumn: "Book_Id",
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
                name: "Lessons",
                columns: table => new
                {
                    Lesson_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Lesson_Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Chapter_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Lesson_Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Chapters_Chapter_Id",
                        column: x => x.Chapter_Id,
                        principalTable: "Chapters",
                        principalColumn: "Chapter_Id",
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

            migrationBuilder.CreateTable(
                name: "Plant_Biology_Animals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommonName = table.Column<string>(type: "text", nullable: false),
                    ScientificName = table.Column<string>(type: "text", nullable: false),
                    SpecieType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Habitat = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    IsExtinct = table.Column<bool>(type: "boolean", nullable: false),
                    DiscoveredAt = table.Column<string>(type: "text", nullable: false),
                    AverageLifeSpan = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plant_Biology_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plant_Biology_Animals_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Lesson_Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_Book_Id",
                table: "Chapters",
                column: "Book_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Chapter_Id",
                table: "Lessons",
                column: "Chapter_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Plant_Biology_Animals_LessonId",
                table: "Plant_Biology_Animals",
                column: "LessonId");
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

            migrationBuilder.DropTable(
                name: "Plant_Biology_Animals");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
