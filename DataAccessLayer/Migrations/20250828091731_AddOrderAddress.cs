using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDesign");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Design",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CartItemId = table.Column<int>(type: "int", nullable: true),
                    OrderDetailId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Design", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Design_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Design_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(12)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAddress_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ColorId",
                table: "OrderDetails",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_DeliveryId",
                table: "OrderDetails",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MaterialId",
                table: "OrderDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_SizeId",
                table: "OrderDetails",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ColorId",
                table: "CartItems",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_DeliveryId",
                table: "CartItems",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_MaterialId",
                table: "CartItems",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_SizeId",
                table: "CartItems",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Design_CartItemId",
                table: "Design",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Design_OrderDetailId",
                table: "Design",
                column: "OrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAddress_OrderId",
                table: "OrderAddress",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Colors_ColorId",
                table: "CartItems",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Deliveries_DeliveryId",
                table: "CartItems",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Materials_MaterialId",
                table: "CartItems",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Sizes_SizeId",
                table: "CartItems",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Colors_ColorId",
                table: "OrderDetails",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Deliveries_DeliveryId",
                table: "OrderDetails",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Materials_MaterialId",
                table: "OrderDetails",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Sizes_SizeId",
                table: "OrderDetails",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Colors_ColorId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Deliveries_DeliveryId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Materials_MaterialId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Sizes_SizeId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Colors_ColorId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Deliveries_DeliveryId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Materials_MaterialId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Sizes_SizeId",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Design");

            migrationBuilder.DropTable(
                name: "OrderAddress");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ColorId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_DeliveryId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_MaterialId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_SizeId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ColorId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_DeliveryId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_MaterialId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_SizeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "CartItems");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Orders",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ProductDesign",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDesign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDesign_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDesign_ProductId",
                table: "ProductDesign",
                column: "ProductId");
        }
    }
}
