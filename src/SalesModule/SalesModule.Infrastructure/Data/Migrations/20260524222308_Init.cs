using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SalesModule.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sales");

            migrationBuilder.CreateTable(
                name: "Deals",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ContactId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesPersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ExpectedCloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PipelineId = table.Column<string>(type: "text", nullable: false),
                    CurrentStageId = table.Column<string>(type: "text", nullable: false),
                    Outcome = table.Column<int>(type: "integer", nullable: false),
                    Value_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Value_Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outbox",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    JsonData = table.Column<string>(type: "text", nullable: false),
                    OccurredOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pipelines",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pipelines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DealMovements",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    MovementType = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    ReturnedToStageId = table.Column<string>(type: "text", nullable: true),
                    FromStageId = table.Column<string>(type: "text", nullable: true),
                    ToStageId = table.Column<string>(type: "text", nullable: true),
                    Outcome = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealMovements_Deals_DealId",
                        column: x => x.DealId,
                        principalSchema: "sales",
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StageSnapshots",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DealId = table.Column<string>(type: "text", nullable: false),
                    StageId = table.Column<string>(type: "text", nullable: false),
                    PipelineId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Probability = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageSnapshots_Deals_DealId",
                        column: x => x.DealId,
                        principalSchema: "sales",
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stages",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PipelineId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Probability = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stages_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalSchema: "sales",
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DealMovements_DealId",
                schema: "sales",
                table: "DealMovements",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_PipelineId",
                schema: "sales",
                table: "Stages",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_StageSnapshots_DealId",
                schema: "sales",
                table: "StageSnapshots",
                column: "DealId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DealMovements",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Outbox",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Stages",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "StageSnapshots",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Pipelines",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Deals",
                schema: "sales");
        }
    }
}
