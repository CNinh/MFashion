using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryName",
                value: "T-shirt");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryName",
                value: "Sweatshirt");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryName",
                value: "Hoodie");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CategoryName",
                value: "Tank Top");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CategoryName",
                value: "Long_sleeves");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryName",
                value: "Design Services");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryName",
                value: "HaruTheme");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryName",
                value: "Print Company");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CategoryName",
                value: "Print Shop");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CategoryName",
                value: "Uncategorized");
        }
    }
}
