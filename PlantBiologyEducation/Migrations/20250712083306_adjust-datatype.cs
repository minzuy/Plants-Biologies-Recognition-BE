using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_BiologyEducation.Migrations
{
    /// <inheritdoc />
    public partial class adjustdatatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DiscoveredAt",
                table: "Plant_Biology_Animals",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DiscoveredAt",
                table: "Plant_Biology_Animals",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
