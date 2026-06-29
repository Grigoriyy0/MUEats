using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUEats.Restaurants.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Column_Address_ToTable_Restaurants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "restaurants",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "restaurants",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }
    }
}
