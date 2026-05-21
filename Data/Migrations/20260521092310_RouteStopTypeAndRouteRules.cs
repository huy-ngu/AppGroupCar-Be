using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class RouteStopTypeAndRouteRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Old_CompanyDrivers");

            migrationBuilder.DropColumn(
                name: "DropoffAllowed",
                table: "OfferRoutePoints");

            migrationBuilder.DropColumn(
                name: "PickupAllowed",
                table: "OfferRoutePoints");

            migrationBuilder.DropColumn(
                name: "DropoffAllowed",
                table: "BusRoutePoints");

            migrationBuilder.DropColumn(
                name: "PickupAllowed",
                table: "BusRoutePoints");

            migrationBuilder.AddColumn<int>(
                name: "StopType",
                table: "OfferRoutePoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RatingCompanyAverage",
                table: "Companies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RatingCompanyCount",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyDriverId",
                table: "BusShowtimes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "StopType",
                table: "BusRoutePoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CompanyDrivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LicenseClass = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DriverVerificationStatus = table.Column<int>(type: "int", nullable: false),
                    LicenseDocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyDrivers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyDrivers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusShowtimes_CompanyDriverId",
                table: "BusShowtimes",
                column: "CompanyDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDrivers_CompanyId",
                table: "CompanyDrivers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDrivers_UserId",
                table: "CompanyDrivers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusShowtimes_CompanyDrivers_CompanyDriverId",
                table: "BusShowtimes",
                column: "CompanyDriverId",
                principalTable: "CompanyDrivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusShowtimes_CompanyDrivers_CompanyDriverId",
                table: "BusShowtimes");

            migrationBuilder.DropTable(
                name: "CompanyDrivers");

            migrationBuilder.DropIndex(
                name: "IX_BusShowtimes_CompanyDriverId",
                table: "BusShowtimes");

            migrationBuilder.DropColumn(
                name: "StopType",
                table: "OfferRoutePoints");

            migrationBuilder.DropColumn(
                name: "RatingCompanyAverage",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "RatingCompanyCount",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyDriverId",
                table: "BusShowtimes");

            migrationBuilder.DropColumn(
                name: "StopType",
                table: "BusRoutePoints");

            migrationBuilder.AddColumn<bool>(
                name: "DropoffAllowed",
                table: "OfferRoutePoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PickupAllowed",
                table: "OfferRoutePoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DropoffAllowed",
                table: "BusRoutePoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PickupAllowed",
                table: "BusRoutePoints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Old_CompanyDrivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DriverVerificationStatus = table.Column<int>(type: "int", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseClass = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LicenseDocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Old_CompanyDrivers", x => x.Id);
                });
        }
    }
}
