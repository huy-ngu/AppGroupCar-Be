using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGroupCar.Migrations
{
    /// <inheritdoc />
    public partial class update_table_shipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shipments_ShipmentOfferId",
                table: "Shipments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShipmentOfferId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shipments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DropoffLocationId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "HandlingNote",
                table: "Shipments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFragile",
                table: "Shipments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OfferId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PickupLocationId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Volume",
                table: "Shipments",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "Shipments",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DropoffLocationId",
                table: "Shipments",
                column: "DropoffLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OfferId",
                table: "Shipments",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_PickupLocationId",
                table: "Shipments",
                column: "PickupLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentOfferId",
                table: "Shipments",
                column: "ShipmentOfferId",
                unique: true,
                filter: "[ShipmentOfferId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Shipment_Must_Have_Only_One_Offer_Source",
                table: "Shipments",
                sql: "(OfferId IS NOT NULL AND ShipmentOfferId IS NULL) OR (OfferId IS NULL AND ShipmentOfferId IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Locations_DropoffLocationId",
                table: "Shipments",
                column: "DropoffLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Locations_PickupLocationId",
                table: "Shipments",
                column: "PickupLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Offers_OfferId",
                table: "Shipments",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Locations_DropoffLocationId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Locations_PickupLocationId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Offers_OfferId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_DropoffLocationId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_OfferId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_PickupLocationId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_ShipmentOfferId",
                table: "Shipments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Shipment_Must_Have_Only_One_Offer_Source",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DropoffLocationId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "HandlingNote",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "IsFragile",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "PickupLocationId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Shipments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShipmentOfferId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentOfferId",
                table: "Shipments",
                column: "ShipmentOfferId",
                unique: true);
        }
    }
}
