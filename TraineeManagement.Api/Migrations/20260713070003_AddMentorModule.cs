using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMentorModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mentors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "longtext", nullable: false),
                    LastName = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Expertise = table.Column<string>(type: "longtext", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentors", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 7, 0, 2, 556, DateTimeKind.Utc).AddTicks(3406), "$2a$11$o5TVj.ImFWPwQLcB/mueL.BBRiagt21AaiWumqAnDyk5qk7YW27Fq", new DateTime(2026, 7, 13, 7, 0, 2, 556, DateTimeKind.Utc).AddTicks(3651) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mentors");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 9, 7, 35, 9, 498, DateTimeKind.Utc).AddTicks(9611), "$2a$11$yY9QZ4I1f5IRTD8sevfaGOEYTaWTXxeCqjDAIzvAEteD5XOAO8TcW", new DateTime(2026, 7, 9, 7, 35, 9, 498, DateTimeKind.Utc).AddTicks(9913) });
        }
    }
}
