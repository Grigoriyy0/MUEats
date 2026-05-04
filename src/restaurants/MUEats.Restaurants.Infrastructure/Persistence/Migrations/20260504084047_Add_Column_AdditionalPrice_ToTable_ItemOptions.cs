using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUEats.Restaurants.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Column_AdditionalPrice_ToTable_ItemOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "additional_price",
                table: "item_option",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "additional_price",
                table: "item_option");
        }
    }
}
