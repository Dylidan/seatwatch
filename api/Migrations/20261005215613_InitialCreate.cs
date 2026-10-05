using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SeatWatch.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Venue = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LowestPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Date", "LowestPrice", "Name", "Venue" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 89.00m, "Yankees vs. Red Sox", "Yankee Stadium" },
                    { 2, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc), 185.00m, "Knicks vs. Celtics", "Madison Square Garden" },
                    { 3, new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Utc), 249.00m, "Hamilton", "Richard Rodgers Theatre" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
