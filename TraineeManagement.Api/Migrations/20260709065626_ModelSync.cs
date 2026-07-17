using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModelSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 9, 6, 56, 26, 62, DateTimeKind.Utc).AddTicks(1007), new DateTime(2026, 7, 9, 6, 56, 26, 62, DateTimeKind.Utc).AddTicks(1198) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 9, 6, 28, 51, 37, DateTimeKind.Utc).AddTicks(4782), new DateTime(2026, 7, 9, 6, 28, 51, 37, DateTimeKind.Utc).AddTicks(4933) });
        }
    }
}
