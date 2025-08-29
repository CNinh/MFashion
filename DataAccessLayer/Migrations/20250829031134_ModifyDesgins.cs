using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDesgins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Period",
                table: "Deliveries",
                newName: "DeliveryType");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Design",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "Design",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraFees",
                table: "Deliveries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "Deliveries",
                columns: new[] { "Id", "DeliveryType", "ExtraFees" },
                values: new object[,]
                {
                    { 1, "Standard Shipping", 0m },
                    { 2, "Express shipping", 20m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Deliveries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Deliveries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Design");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "Design");

            migrationBuilder.DropColumn(
                name: "ExtraFees",
                table: "Deliveries");

            migrationBuilder.RenameColumn(
                name: "DeliveryType",
                table: "Deliveries",
                newName: "Period");
        }
    }
}
