using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUEats.Restaurants.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_MetadataColumns_ToTable_OrderSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "order_snapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "last_error",
                table: "order_snapshots",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "lock_id",
                table: "order_snapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "next_attempt_at",
                table: "order_snapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "retry_count",
                table: "order_snapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "order_snapshots",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "order_snapshots");

            migrationBuilder.DropColumn(
                name: "last_error",
                table: "order_snapshots");

            migrationBuilder.DropColumn(
                name: "lock_id",
                table: "order_snapshots");

            migrationBuilder.DropColumn(
                name: "next_attempt_at",
                table: "order_snapshots");

            migrationBuilder.DropColumn(
                name: "retry_count",
                table: "order_snapshots");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "order_snapshots");
        }
    }
}
