using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PiggyBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSettlementsAndSharedAccountExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupSettlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSettlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupSettlements_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupSettlements_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupSettlements_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SharedAccountExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SharedAccountId = table.Column<int>(type: "int", nullable: false),
                    PaidByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedAccountExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedAccountExpenses_AspNetUsers_PaidByUserId",
                        column: x => x.PaidByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SharedAccountExpenses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SharedAccountExpenses_SharedAccounts_SharedAccountId",
                        column: x => x.SharedAccountId,
                        principalTable: "SharedAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SharedAccountSettlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SharedAccountId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedAccountSettlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedAccountSettlements_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SharedAccountSettlements_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SharedAccountSettlements_SharedAccounts_SharedAccountId",
                        column: x => x.SharedAccountId,
                        principalTable: "SharedAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 19, 0, 30, 5, 556, DateTimeKind.Utc).AddTicks(836));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 19, 0, 30, 5, 556, DateTimeKind.Utc).AddTicks(842));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 19, 0, 30, 5, 556, DateTimeKind.Utc).AddTicks(844));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 19, 0, 30, 5, 556, DateTimeKind.Utc).AddTicks(845));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 19, 0, 30, 5, 556, DateTimeKind.Utc).AddTicks(847));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 19, 0, 30, 5, 556, DateTimeKind.Utc).AddTicks(848));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 19, 0, 30, 5, 556, DateTimeKind.Utc).AddTicks(849));

            migrationBuilder.CreateIndex(
                name: "IX_GroupSettlements_FromUserId",
                table: "GroupSettlements",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSettlements_GroupId",
                table: "GroupSettlements",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSettlements_ToUserId",
                table: "GroupSettlements",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedAccountExpenses_CategoryId",
                table: "SharedAccountExpenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedAccountExpenses_PaidByUserId",
                table: "SharedAccountExpenses",
                column: "PaidByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedAccountExpenses_SharedAccountId",
                table: "SharedAccountExpenses",
                column: "SharedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedAccountSettlements_FromUserId",
                table: "SharedAccountSettlements",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedAccountSettlements_SharedAccountId",
                table: "SharedAccountSettlements",
                column: "SharedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedAccountSettlements_ToUserId",
                table: "SharedAccountSettlements",
                column: "ToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupSettlements");

            migrationBuilder.DropTable(
                name: "SharedAccountExpenses");

            migrationBuilder.DropTable(
                name: "SharedAccountSettlements");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 18, 23, 58, 37, 40, DateTimeKind.Utc).AddTicks(1795));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 18, 23, 58, 37, 40, DateTimeKind.Utc).AddTicks(1802));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 18, 23, 58, 37, 40, DateTimeKind.Utc).AddTicks(1804));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 18, 23, 58, 37, 40, DateTimeKind.Utc).AddTicks(1805));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 18, 23, 58, 37, 40, DateTimeKind.Utc).AddTicks(1806));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 18, 23, 58, 37, 40, DateTimeKind.Utc).AddTicks(1808));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 18, 23, 58, 37, 40, DateTimeKind.Utc).AddTicks(1809));
        }
    }
}
