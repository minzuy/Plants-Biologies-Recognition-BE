using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_BiologyEducation.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantBiologyAnimalsLessonRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Plant_Biology_Animals",
                newName: "SpecieType");

            migrationBuilder.RenameColumn(
                name: "DetailsInformation",
                table: "Plant_Biology_Animals",
                newName: "ScientificName");

            migrationBuilder.AddColumn<int>(
                name: "AverageLifeSpan",
                table: "Plant_Biology_Animals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommonName",
                table: "Plant_Biology_Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Plant_Biology_Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DiscoveredAt",
                table: "Plant_Biology_Animals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Habitat",
                table: "Plant_Biology_Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsExtinct",
                table: "Plant_Biology_Animals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LessonId",
                table: "Plant_Biology_Animals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Plant_Biology_Animals_LessonId",
                table: "Plant_Biology_Animals",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plant_Biology_Animals_Lessons_LessonId",
                table: "Plant_Biology_Animals",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Lesson_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plant_Biology_Animals_Lessons_LessonId",
                table: "Plant_Biology_Animals");

            migrationBuilder.DropIndex(
                name: "IX_Plant_Biology_Animals_LessonId",
                table: "Plant_Biology_Animals");

            migrationBuilder.DropColumn(
                name: "AverageLifeSpan",
                table: "Plant_Biology_Animals");

            migrationBuilder.DropColumn(
                name: "CommonName",
                table: "Plant_Biology_Animals");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Plant_Biology_Animals");

            migrationBuilder.DropColumn(
                name: "DiscoveredAt",
                table: "Plant_Biology_Animals");

            migrationBuilder.DropColumn(
                name: "Habitat",
                table: "Plant_Biology_Animals");

            migrationBuilder.DropColumn(
                name: "IsExtinct",
                table: "Plant_Biology_Animals");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "Plant_Biology_Animals");

            migrationBuilder.RenameColumn(
                name: "SpecieType",
                table: "Plant_Biology_Animals",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ScientificName",
                table: "Plant_Biology_Animals",
                newName: "DetailsInformation");
        }
    }
}
