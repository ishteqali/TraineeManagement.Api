using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 15, 12, 17, 19, 259, DateTimeKind.Utc).AddTicks(24), "$2a$11$u75VH7DOBqO/0YnoiacWwuJ2B6FybQwRBThBMxyMXFgUwM3MREMuO", new DateTime(2026, 7, 15, 12, 17, 19, 259, DateTimeKind.Utc).AddTicks(288) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 15, 9, 10, 21, 733, DateTimeKind.Utc).AddTicks(9232), "$2a$11$1HePtlu5ORj8mqIO7zwMuuQI//3ulXdl7Qy7vjZq3kaEGmGw1cS.m", new DateTime(2026, 7, 15, 9, 10, 21, 733, DateTimeKind.Utc).AddTicks(9428) });
        }
    }
}
