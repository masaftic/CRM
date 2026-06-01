using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesModule.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class OutboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error",
                schema: "sales",
                table: "Outbox",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessedOn",
                schema: "sales",
                table: "Outbox",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "sales",
                table: "Outbox",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                schema: "sales",
                table: "Outbox");

            migrationBuilder.DropColumn(
                name: "ProcessedOn",
                schema: "sales",
                table: "Outbox");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "sales",
                table: "Outbox");
        }
    }
}
