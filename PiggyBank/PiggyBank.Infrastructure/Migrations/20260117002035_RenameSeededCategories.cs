using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PiggyBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSeededCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2026, 1, 17, 0, 20, 34, 778, DateTimeKind.Utc).AddTicks(560), "Entertainment" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2026, 1, 17, 0, 20, 34, 778, DateTimeKind.Utc).AddTicks(564), "Groceries" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2026, 1, 17, 0, 20, 34, 778, DateTimeKind.Utc).AddTicks(565), "Bills" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 17, 0, 20, 34, 778, DateTimeKind.Utc).AddTicks(567));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2026, 1, 17, 0, 20, 34, 778, DateTimeKind.Utc).AddTicks(568), "Health" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2026, 1, 17, 0, 20, 34, 778, DateTimeKind.Utc).AddTicks(569), "Restaurant" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2026, 1, 17, 0, 20, 34, 778, DateTimeKind.Utc).AddTicks(570), "Other" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2025, 12, 29, 1, 5, 13, 404, DateTimeKind.Utc).AddTicks(1025), "Zabava" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2025, 12, 29, 1, 5, 13, 404, DateTimeKind.Utc).AddTicks(1031), "Grosari" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2025, 12, 29, 1, 5, 13, 404, DateTimeKind.Utc).AddTicks(1032), "Računi" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 29, 1, 5, 13, 404, DateTimeKind.Utc).AddTicks(1034));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2025, 12, 29, 1, 5, 13, 404, DateTimeKind.Utc).AddTicks(1035), "Zdravlje" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2025, 12, 29, 1, 5, 13, 404, DateTimeKind.Utc).AddTicks(1037), "Restoran" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Name" },
                values: new object[] { new DateTime(2025, 12, 29, 1, 5, 13, 404, DateTimeKind.Utc).AddTicks(1038), "Ostalo" });
        }
    }
}
