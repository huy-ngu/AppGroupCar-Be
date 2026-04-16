using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class update_table_shipmentOffer_PK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentOffers_Offers_OfferId",
                table: "ShipmentOffers");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentOffers_Offers_OfferId",
                table: "ShipmentOffers",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentOffers_Offers_OfferId",
                table: "ShipmentOffers");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentOffers_Offers_OfferId",
                table: "ShipmentOffers",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
