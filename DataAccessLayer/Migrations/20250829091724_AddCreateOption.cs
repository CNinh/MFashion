using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Deliveries_DeliveryId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Deliveries_DeliveryId",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ProductDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_DeliveryId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_DeliveryId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "ThemeColor",
                table: "Colors",
                newName: "PrimaryColor");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Deliveries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryHex",
                table: "Colors",
                type: "nvarchar(9)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryHex",
                table: "Colors",
                type: "nvarchar(9)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PrimaryHex", "SecondaryColor", "SecondaryHex" },
                values: new object[] { "#000000", null, null });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PrimaryColor", "PrimaryHex", "SecondaryColor", "SecondaryHex" },
                values: new object[] { "Black", "#000000", "Green", "#008000" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PrimaryHex", "SecondaryColor", "SecondaryHex" },
                values: new object[] { "#A52A2A", null, null });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "PrimaryHex", "SecondaryColor", "SecondaryHex" },
                values: new object[] { "#FF0000", null, null });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "PrimaryHex", "SecondaryColor", "SecondaryHex" },
                values: new object[] { "#FFFFFF", null, null });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "PrimaryColor", "PrimaryHex", "SecondaryColor", "SecondaryHex" },
                values: new object[] { "White", "#FFFFFF", "Green", "#008000" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "PrimaryHex", "SecondaryColor", "SecondaryHex" },
                values: new object[] { "#FFFF00", null, null });

            migrationBuilder.UpdateData(
                table: "Deliveries",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProductId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Deliveries",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProductId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryId",
                table: "Orders",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_ProductId",
                table: "Deliveries",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Products_ProductId",
                table: "Deliveries",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Deliveries_DeliveryId",
                table: "Orders",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Products_ProductId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Deliveries_DeliveryId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_ProductId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "PrimaryHex",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "SecondaryHex",
                table: "Colors");

            migrationBuilder.RenameColumn(
                name: "PrimaryColor",
                table: "Colors",
                newName: "ThemeColor");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductDeliveries",
                columns: table => new
                {
                    DeliveriesId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDeliveries", x => new { x.DeliveriesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_ProductDeliveries_Deliveries_DeliveriesId",
                        column: x => x.DeliveriesId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductDeliveries_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 2,
                column: "ThemeColor",
                value: "Black/Green");

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 6,
                column: "ThemeColor",
                value: "White/Green");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_DeliveryId",
                table: "OrderDetails",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_DeliveryId",
                table: "CartItems",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDeliveries_ProductsId",
                table: "ProductDeliveries",
                column: "ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Deliveries_DeliveryId",
                table: "CartItems",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Deliveries_DeliveryId",
                table: "OrderDetails",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");
        }
    }
}
