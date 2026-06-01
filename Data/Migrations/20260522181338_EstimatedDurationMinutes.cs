using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class EstimatedDurationMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimatedDurationMinutes",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseDocumentUrl",
                table: "CompanyDrivers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedDurationMinutes",
                table: "BusRoutes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedDurationMinutes",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "EstimatedDurationMinutes",
                table: "BusRoutes");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseDocumentUrl",
                table: "CompanyDrivers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
