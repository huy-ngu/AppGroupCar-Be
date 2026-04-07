using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class CompanyVehicle_UseBusinessKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusShowtimes_CompanyVehicles_CompanyVehicleId",
                table: "BusShowtimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyVehicles",
                table: "CompanyVehicles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CompanyVehicles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyVehicles",
                table: "CompanyVehicles",
                column: "CompanyVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ShowtimeId_SeatNumber",
                table: "Seats",
                columns: new[] { "ShowtimeId", "SeatNumber" },
                unique: true,
                filter: "[DeleteAt] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BusShowtimes_CompanyVehicles_CompanyVehicleId",
                table: "BusShowtimes",
                column: "CompanyVehicleId",
                principalTable: "CompanyVehicles",
                principalColumn: "CompanyVehicleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusShowtimes_CompanyVehicles_CompanyVehicleId",
                table: "BusShowtimes");

            migrationBuilder.DropIndex(
                name: "IX_Seats_ShowtimeId_SeatNumber",
                table: "Seats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyVehicles",
                table: "CompanyVehicles");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CompanyVehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyVehicles",
                table: "CompanyVehicles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusShowtimes_CompanyVehicles_CompanyVehicleId",
                table: "BusShowtimes",
                column: "CompanyVehicleId",
                principalTable: "CompanyVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
