using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class StoreCloudinaryAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewUrl",
                table: "ReviewsImages",
                newName: "ResourceType");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ReviewsImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "ReviewsImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "BlogsImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "BlogsImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ReviewsImages");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "ReviewsImages");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "BlogsImages");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "BlogsImages");

            migrationBuilder.RenameColumn(
                name: "ResourceType",
                table: "ReviewsImages",
                newName: "ReviewUrl");
        }
    }
}
