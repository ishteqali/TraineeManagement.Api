using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminHashPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 9, 7, 35, 9, 498, DateTimeKind.Utc).AddTicks(9611), "$2a$11$yY9QZ4I1f5IRTD8sevfaGOEYTaWTXxeCqjDAIzvAEteD5XOAO8TcW", new DateTime(2026, 7, 9, 7, 35, 9, 498, DateTimeKind.Utc).AddTicks(9913) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 9, 6, 56, 26, 62, DateTimeKind.Utc).AddTicks(1007), "admin@123", new DateTime(2026, 7, 9, 6, 56, 26, 62, DateTimeKind.Utc).AddTicks(1198) });
        }
    }
}
