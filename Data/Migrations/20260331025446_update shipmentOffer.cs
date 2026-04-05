using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class updateshipmentOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvoidSunLight",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "IsPerishable",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "RequiresCooler",
                table: "ShipmentRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AvoidSunLight",
                table: "ShipmentRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPerishable",
                table: "ShipmentRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresCooler",
                table: "ShipmentRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
