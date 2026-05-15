using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUEats.Restaurants.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddColumns_ToTable_OrderSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "order_snapshots",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

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

            migrationBuilder.AddColumn<Guid>(
                name: "order_id",
                table: "order_item_snapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_order_item_snapshots_order_id",
                table: "order_item_snapshots",
                column: "order_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_item_snapshots_order_snapshots_order_id",
                table: "order_item_snapshots",
                column: "order_id",
                principalTable: "order_snapshots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_item_snapshots_order_snapshots_order_id",
                table: "order_item_snapshots");

            migrationBuilder.DropIndex(
                name: "ix_order_item_snapshots_order_id",
                table: "order_item_snapshots");

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

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "order_item_snapshots");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "order_snapshots",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
