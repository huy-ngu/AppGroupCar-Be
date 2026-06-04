using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class pointBusBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DropoffLocationId",
                table: "BusBookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PickupLocationId",
                table: "BusBookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BusBookings_DropoffLocationId",
                table: "BusBookings",
                column: "DropoffLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BusBookings_PickupLocationId",
                table: "BusBookings",
                column: "PickupLocationId");

            migrationBuilder.Sql(@"
UPDATE b
SET
    PickupLocationId = startPoint.LocationId,
    DropoffLocationId = endPoint.LocationId
FROM BusBookings b
INNER JOIN BusShowtimes s ON b.ShowtimeId = s.Id
INNER JOIN BusRoutes r ON s.RouteId = r.Id
OUTER APPLY (
    SELECT TOP (1) rp.LocationId
    FROM BusRoutePoints rp
    WHERE rp.RouteId = r.Id AND rp.DeleteAt IS NULL
    ORDER BY rp.Sequence ASC
) startPoint
OUTER APPLY (
    SELECT TOP (1) rp.LocationId
    FROM BusRoutePoints rp
    WHERE rp.RouteId = r.Id AND rp.DeleteAt IS NULL
    ORDER BY rp.Sequence DESC
) endPoint
WHERE b.PickupLocationId = '00000000-0000-0000-0000-000000000000'
   OR b.DropoffLocationId = '00000000-0000-0000-0000-000000000000';");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM BusBookings
    WHERE PickupLocationId = '00000000-0000-0000-0000-000000000000'
       OR DropoffLocationId = '00000000-0000-0000-0000-000000000000'
)
BEGIN
    THROW 50000, 'BusBookings location backfill failed before adding FK constraints.', 1;
END");

            migrationBuilder.AddForeignKey(
                name: "FK_BusBookings_Locations_DropoffLocationId",
                table: "BusBookings",
                column: "DropoffLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BusBookings_Locations_PickupLocationId",
                table: "BusBookings",
                column: "PickupLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusBookings_Locations_DropoffLocationId",
                table: "BusBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BusBookings_Locations_PickupLocationId",
                table: "BusBookings");

            migrationBuilder.DropIndex(
                name: "IX_BusBookings_DropoffLocationId",
                table: "BusBookings");

            migrationBuilder.DropIndex(
                name: "IX_BusBookings_PickupLocationId",
                table: "BusBookings");

            migrationBuilder.DropColumn(
                name: "DropoffLocationId",
                table: "BusBookings");

            migrationBuilder.DropColumn(
                name: "PickupLocationId",
                table: "BusBookings");
        }
    }
}
