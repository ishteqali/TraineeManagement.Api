using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningTaskModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LearningTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    ExpectedTechStack = table.Column<string>(type: "longtext", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningTasks", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 9, 48, 10, 732, DateTimeKind.Utc).AddTicks(2100), "$2a$11$aO63FKjYfYmau8HWObgxgu45J/aTKQAL8sVgaV7FQe25LctdTpkBG", new DateTime(2026, 7, 13, 9, 48, 10, 732, DateTimeKind.Utc).AddTicks(2337) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningTasks");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 7, 0, 2, 556, DateTimeKind.Utc).AddTicks(3406), "$2a$11$o5TVj.ImFWPwQLcB/mueL.BBRiagt21AaiWumqAnDyk5qk7YW27Fq", new DateTime(2026, 7, 13, 7, 0, 2, 556, DateTimeKind.Utc).AddTicks(3651) });
        }
    }
}
