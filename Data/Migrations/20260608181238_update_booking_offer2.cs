using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class update_booking_offer2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingOffers_Bookings_BookingId",
                table: "BookingOffers");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                table: "BookingOffers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "OfferId",
                table: "BookingOffers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BookingOffers_OfferId",
                table: "BookingOffers",
                column: "OfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingOffers_Bookings_BookingId",
                table: "BookingOffers",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingOffers_Offers_OfferId",
                table: "BookingOffers",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingOffers_Bookings_BookingId",
                table: "BookingOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingOffers_Offers_OfferId",
                table: "BookingOffers");

            migrationBuilder.DropIndex(
                name: "IX_BookingOffers_OfferId",
                table: "BookingOffers");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "BookingOffers");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                table: "BookingOffers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingOffers_Bookings_BookingId",
                table: "BookingOffers",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
