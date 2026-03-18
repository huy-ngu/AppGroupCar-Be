using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class updateBookingItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BookingItems");

            migrationBuilder.AddColumn<Guid>(
                name: "DropoffLocationId",
                table: "BookingItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PickupLocationId",
                table: "BookingItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_DropoffLocationId",
                table: "BookingItems",
                column: "DropoffLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_PickupLocationId",
                table: "BookingItems",
                column: "PickupLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_Locations_DropoffLocationId",
                table: "BookingItems",
                column: "DropoffLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_Locations_PickupLocationId",
                table: "BookingItems",
                column: "PickupLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_Locations_DropoffLocationId",
                table: "BookingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_Locations_PickupLocationId",
                table: "BookingItems");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_DropoffLocationId",
                table: "BookingItems");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_PickupLocationId",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "DropoffLocationId",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "PickupLocationId",
                table: "BookingItems");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BookingItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
