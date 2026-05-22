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

            migrationBuilder.AddColumn<int>(
                name: "StopType",
                table: "BusRoutePoints",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StopType",
                table: "OfferRoutePoints");

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
        }
    }
}
