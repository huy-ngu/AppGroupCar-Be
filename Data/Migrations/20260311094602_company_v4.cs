using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class company_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedBy",
                table: "Companies",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
