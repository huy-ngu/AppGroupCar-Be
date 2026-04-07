using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class updatedbbooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BusBookingSeats_SeatId",
                table: "BusBookingSeats");

            migrationBuilder.AlterColumn<string>(
                name: "SeatNumber",
                table: "BusBookingSeats",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireAt",
                table: "BusBookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusBookingSeats_SeatId",
                table: "BusBookingSeats",
                column: "SeatId",
                unique: true,
                filter: "[DeleteAt] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BusBookingSeats_SeatId",
                table: "BusBookingSeats");

            migrationBuilder.DropColumn(
                name: "ExpireAt",
                table: "BusBookings");

            migrationBuilder.AlterColumn<int>(
                name: "SeatNumber",
                table: "BusBookingSeats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_BusBookingSeats_SeatId",
                table: "BusBookingSeats",
                column: "SeatId");
        }
    }
}
