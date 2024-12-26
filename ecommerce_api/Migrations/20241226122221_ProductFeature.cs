using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce_api.Migrations
{
    /// <inheritdoc />
    public partial class ProductFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeatureId",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureId",
                table: "Products");
        }
    }
}
