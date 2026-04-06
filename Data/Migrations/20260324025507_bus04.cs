using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class bus04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDrivers_Companies_CompanyId",
                table: "CompanyDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDrivers_Users_UserId",
                table: "CompanyDrivers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyDrivers_CompanyId",
                table: "CompanyDrivers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyDrivers_UserId",
                table: "CompanyDrivers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CompanyDrivers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CompanyDrivers");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationDocumentUrl",
                table: "CompanyVehicles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlImage",
                table: "CompanyVehicles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "BusRoutes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BusRoutes_CompanyId",
                table: "BusRoutes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusRoutes_Companies_CompanyId",
                table: "BusRoutes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusShowtimes_Drivers_Id",
                table: "BusShowtimes",
                column: "Id",
                principalTable: "Drivers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusRoutes_Companies_CompanyId",
                table: "BusRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_BusShowtimes_Drivers_Id",
                table: "BusShowtimes");

            migrationBuilder.DropIndex(
                name: "IX_BusRoutes_CompanyId",
                table: "BusRoutes");

            migrationBuilder.DropColumn(
                name: "RegistrationDocumentUrl",
                table: "CompanyVehicles");

            migrationBuilder.DropColumn(
                name: "UrlImage",
                table: "CompanyVehicles");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "BusRoutes");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "CompanyDrivers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CompanyDrivers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDrivers_CompanyId",
                table: "CompanyDrivers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDrivers_UserId",
                table: "CompanyDrivers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDrivers_Companies_CompanyId",
                table: "CompanyDrivers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDrivers_Users_UserId",
                table: "CompanyDrivers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
