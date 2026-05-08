using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUEats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Columns_ToTable_InboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LockId",
                table: "InboxMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryAt",
                table: "InboxMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "InboxMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "InboxMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockId",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "NextRetryAt",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InboxMessages");
        }
    }
}
