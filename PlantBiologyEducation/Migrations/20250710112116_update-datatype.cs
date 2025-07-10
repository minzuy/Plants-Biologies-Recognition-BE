using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_BiologyEducation.Migrations
{
    /// <inheritdoc />
    public partial class updatedatatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AverageLifeSpan",
                table: "Plant_Biology_Animals",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AverageLifeSpan",
                table: "Plant_Biology_Animals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
