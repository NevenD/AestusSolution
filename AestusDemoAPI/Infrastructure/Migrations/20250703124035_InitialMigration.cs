using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AestusDemoAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSuspicious = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "Comment", "IsSuspicious", "Location", "Timestamp", "UserId" },
                values: new object[,]
                {
                    { 1, 50.0, "Normalna potrošnja", false, "Zagreb", new DateTime(2024, 4, 24, 11, 0, 0, 0, DateTimeKind.Local), "user_001" },
                    { 2, 49.5, "Normalna potrošnja", false, "Zagreb", new DateTime(2024, 4, 24, 11, 5, 0, 0, DateTimeKind.Local), "user_001" },
                    { 3, 5000.0, "Neuobičajeno visok iznos + neočekivana lokacija", true, "Ljubljana", new DateTime(2024, 4, 24, 11, 7, 0, 0, DateTimeKind.Local), "user_001" },
                    { 4, 48.0, "Normalna potrošnja", false, "Zagreb", new DateTime(2024, 4, 24, 11, 20, 0, 0, DateTimeKind.Local), "user_001" },
                    { 5, 47.990000000000002, "Normalna potrošnja", false, "Zagreb", new DateTime(2024, 4, 24, 11, 30, 0, 0, DateTimeKind.Local), "user_001" },
                    { 6, 4000.0, "Neuobičajeno visok iznos", true, "Zagreb", new DateTime(2024, 4, 24, 11, 32, 0, 0, DateTimeKind.Local), "user_001" },
                    { 7, 50.100000000000001, "Normalna potrošnja", false, "Zagreb", new DateTime(2024, 4, 24, 11, 40, 0, 0, DateTimeKind.Local), "user_001" },
                    { 8, 52.0, "Normalna potrošnja", false, "Zagreb", new DateTime(2024, 4, 24, 11, 50, 0, 0, DateTimeKind.Local), "user_001" },
                    { 9, 51.0, "Normalna potrošnja", false, "Zagreb", new DateTime(2024, 4, 24, 11, 55, 0, 0, DateTimeKind.Local), "user_001" },
                    { 10, 49.990000000000002, "Neočekivana lokacija", true, "Beograd", new DateTime(2024, 4, 24, 12, 0, 0, 0, DateTimeKind.Local), "user_001" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
