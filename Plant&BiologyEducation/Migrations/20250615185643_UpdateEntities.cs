using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_BiologyEducation.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessBookHistories_Books_Book_Id1",
                table: "AccessBookHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessBookHistories_Users_User_Id1",
                table: "AccessBookHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessLessonHistories_Lessons_Lesson_Id1",
                table: "AccessLessonHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessLessonHistories_Users_User_Id1",
                table: "AccessLessonHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Books_Book_Id1",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Chapters_Chapter_Id1",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageBooks_Books_Book_Id1",
                table: "ManageBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageBooks_Users_User_Id1",
                table: "ManageBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageChapters_Chapters_Chapter_Id1",
                table: "ManageChapters");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageChapters_Users_User_Id1",
                table: "ManageChapters");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageLessons_Lessons_Lesson_Id1",
                table: "ManageLessons");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageLessons_Users_User_Id1",
                table: "ManageLessons");

            migrationBuilder.DropIndex(
                name: "IX_ManageLessons_Lesson_Id1",
                table: "ManageLessons");

            migrationBuilder.DropIndex(
                name: "IX_ManageLessons_User_Id1",
                table: "ManageLessons");

            migrationBuilder.DropIndex(
                name: "IX_ManageChapters_Chapter_Id1",
                table: "ManageChapters");

            migrationBuilder.DropIndex(
                name: "IX_ManageChapters_User_Id1",
                table: "ManageChapters");

            migrationBuilder.DropIndex(
                name: "IX_ManageBooks_Book_Id1",
                table: "ManageBooks");

            migrationBuilder.DropIndex(
                name: "IX_ManageBooks_User_Id1",
                table: "ManageBooks");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_Chapter_Id1",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_Book_Id1",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_AccessLessonHistories_Lesson_Id1",
                table: "AccessLessonHistories");

            migrationBuilder.DropIndex(
                name: "IX_AccessLessonHistories_User_Id1",
                table: "AccessLessonHistories");

            migrationBuilder.DropIndex(
                name: "IX_AccessBookHistories_Book_Id1",
                table: "AccessBookHistories");

            migrationBuilder.DropIndex(
                name: "IX_AccessBookHistories_User_Id1",
                table: "AccessBookHistories");

            migrationBuilder.DropColumn(
                name: "Lesson_Id1",
                table: "ManageLessons");

            migrationBuilder.DropColumn(
                name: "User_Id1",
                table: "ManageLessons");

            migrationBuilder.DropColumn(
                name: "Chapter_Id1",
                table: "ManageChapters");

            migrationBuilder.DropColumn(
                name: "User_Id1",
                table: "ManageChapters");

            migrationBuilder.DropColumn(
                name: "Book_Id1",
                table: "ManageBooks");

            migrationBuilder.DropColumn(
                name: "User_Id1",
                table: "ManageBooks");

            migrationBuilder.DropColumn(
                name: "Chapter_Id1",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Book_Id1",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "Lesson_Id1",
                table: "AccessLessonHistories");

            migrationBuilder.DropColumn(
                name: "User_Id1",
                table: "AccessLessonHistories");

            migrationBuilder.DropColumn(
                name: "Book_Id1",
                table: "AccessBookHistories");

            migrationBuilder.DropColumn(
                name: "User_Id1",
                table: "AccessBookHistories");

            migrationBuilder.CreateIndex(
                name: "IX_ManageLessons_Lesson_Id",
                table: "ManageLessons",
                column: "Lesson_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ManageChapters_Chapter_Id",
                table: "ManageChapters",
                column: "Chapter_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ManageBooks_Book_Id",
                table: "ManageBooks",
                column: "Book_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Chapter_Id",
                table: "Lessons",
                column: "Chapter_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_Book_Id",
                table: "Chapters",
                column: "Book_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLessonHistories_Lesson_Id",
                table: "AccessLessonHistories",
                column: "Lesson_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccessBookHistories_Book_Id",
                table: "AccessBookHistories",
                column: "Book_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessBookHistories_Books_Book_Id",
                table: "AccessBookHistories",
                column: "Book_Id",
                principalTable: "Books",
                principalColumn: "Book_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessBookHistories_Users_User_Id",
                table: "AccessBookHistories",
                column: "User_Id",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLessonHistories_Lessons_Lesson_Id",
                table: "AccessLessonHistories",
                column: "Lesson_Id",
                principalTable: "Lessons",
                principalColumn: "Lesson_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLessonHistories_Users_User_Id",
                table: "AccessLessonHistories",
                column: "User_Id",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Books_Book_Id",
                table: "Chapters",
                column: "Book_Id",
                principalTable: "Books",
                principalColumn: "Book_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Chapters_Chapter_Id",
                table: "Lessons",
                column: "Chapter_Id",
                principalTable: "Chapters",
                principalColumn: "Chapter_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageBooks_Books_Book_Id",
                table: "ManageBooks",
                column: "Book_Id",
                principalTable: "Books",
                principalColumn: "Book_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageBooks_Users_User_Id",
                table: "ManageBooks",
                column: "User_Id",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageChapters_Chapters_Chapter_Id",
                table: "ManageChapters",
                column: "Chapter_Id",
                principalTable: "Chapters",
                principalColumn: "Chapter_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageChapters_Users_User_Id",
                table: "ManageChapters",
                column: "User_Id",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageLessons_Lessons_Lesson_Id",
                table: "ManageLessons",
                column: "Lesson_Id",
                principalTable: "Lessons",
                principalColumn: "Lesson_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageLessons_Users_User_Id",
                table: "ManageLessons",
                column: "User_Id",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessBookHistories_Books_Book_Id",
                table: "AccessBookHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessBookHistories_Users_User_Id",
                table: "AccessBookHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessLessonHistories_Lessons_Lesson_Id",
                table: "AccessLessonHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessLessonHistories_Users_User_Id",
                table: "AccessLessonHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Books_Book_Id",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Chapters_Chapter_Id",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageBooks_Books_Book_Id",
                table: "ManageBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageBooks_Users_User_Id",
                table: "ManageBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageChapters_Chapters_Chapter_Id",
                table: "ManageChapters");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageChapters_Users_User_Id",
                table: "ManageChapters");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageLessons_Lessons_Lesson_Id",
                table: "ManageLessons");

            migrationBuilder.DropForeignKey(
                name: "FK_ManageLessons_Users_User_Id",
                table: "ManageLessons");

            migrationBuilder.DropIndex(
                name: "IX_ManageLessons_Lesson_Id",
                table: "ManageLessons");

            migrationBuilder.DropIndex(
                name: "IX_ManageChapters_Chapter_Id",
                table: "ManageChapters");

            migrationBuilder.DropIndex(
                name: "IX_ManageBooks_Book_Id",
                table: "ManageBooks");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_Chapter_Id",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_Book_Id",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_AccessLessonHistories_Lesson_Id",
                table: "AccessLessonHistories");

            migrationBuilder.DropIndex(
                name: "IX_AccessBookHistories_Book_Id",
                table: "AccessBookHistories");

            migrationBuilder.AddColumn<Guid>(
                name: "Lesson_Id1",
                table: "ManageLessons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "User_Id1",
                table: "ManageLessons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Chapter_Id1",
                table: "ManageChapters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "User_Id1",
                table: "ManageChapters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Book_Id1",
                table: "ManageBooks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "User_Id1",
                table: "ManageBooks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Chapter_Id1",
                table: "Lessons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Book_Id1",
                table: "Chapters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Lesson_Id1",
                table: "AccessLessonHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "User_Id1",
                table: "AccessLessonHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Book_Id1",
                table: "AccessBookHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "User_Id1",
                table: "AccessBookHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ManageLessons_Lesson_Id1",
                table: "ManageLessons",
                column: "Lesson_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_ManageLessons_User_Id1",
                table: "ManageLessons",
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
                name: "IX_ManageBooks_Book_Id1",
                table: "ManageBooks",
                column: "Book_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_ManageBooks_User_Id1",
                table: "ManageBooks",
                column: "User_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Chapter_Id1",
                table: "Lessons",
                column: "Chapter_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_Book_Id1",
                table: "Chapters",
                column: "Book_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLessonHistories_Lesson_Id1",
                table: "AccessLessonHistories",
                column: "Lesson_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLessonHistories_User_Id1",
                table: "AccessLessonHistories",
                column: "User_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_AccessBookHistories_Book_Id1",
                table: "AccessBookHistories",
                column: "Book_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_AccessBookHistories_User_Id1",
                table: "AccessBookHistories",
                column: "User_Id1");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessBookHistories_Books_Book_Id1",
                table: "AccessBookHistories",
                column: "Book_Id1",
                principalTable: "Books",
                principalColumn: "Book_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessBookHistories_Users_User_Id1",
                table: "AccessBookHistories",
                column: "User_Id1",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLessonHistories_Lessons_Lesson_Id1",
                table: "AccessLessonHistories",
                column: "Lesson_Id1",
                principalTable: "Lessons",
                principalColumn: "Lesson_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLessonHistories_Users_User_Id1",
                table: "AccessLessonHistories",
                column: "User_Id1",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Books_Book_Id1",
                table: "Chapters",
                column: "Book_Id1",
                principalTable: "Books",
                principalColumn: "Book_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Chapters_Chapter_Id1",
                table: "Lessons",
                column: "Chapter_Id1",
                principalTable: "Chapters",
                principalColumn: "Chapter_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageBooks_Books_Book_Id1",
                table: "ManageBooks",
                column: "Book_Id1",
                principalTable: "Books",
                principalColumn: "Book_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageBooks_Users_User_Id1",
                table: "ManageBooks",
                column: "User_Id1",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageChapters_Chapters_Chapter_Id1",
                table: "ManageChapters",
                column: "Chapter_Id1",
                principalTable: "Chapters",
                principalColumn: "Chapter_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageChapters_Users_User_Id1",
                table: "ManageChapters",
                column: "User_Id1",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageLessons_Lessons_Lesson_Id1",
                table: "ManageLessons",
                column: "Lesson_Id1",
                principalTable: "Lessons",
                principalColumn: "Lesson_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManageLessons_Users_User_Id1",
                table: "ManageLessons",
                column: "User_Id1",
                principalTable: "Users",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
