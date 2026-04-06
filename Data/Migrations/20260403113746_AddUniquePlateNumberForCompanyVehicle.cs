using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquePlateNumberForCompanyVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CompanyVehicles_PlateNumber",
                table: "CompanyVehicles",
                column: "PlateNumber",
                unique: true,
                filter: "[DeleteAt] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyVehicles_PlateNumber",
                table: "CompanyVehicles");
        }
    }
}
