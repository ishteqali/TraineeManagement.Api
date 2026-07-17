using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubmissionFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "longtext", nullable: false),
                    StorageFileName = table.Column<string>(type: "longtext", nullable: false),
                    ContentType = table.Column<string>(type: "longtext", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "longtext", nullable: true),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionFiles_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 15, 15, 5, 0, 404, DateTimeKind.Utc).AddTicks(6610), "$2a$11$IblzUPRvCGQZKBt3PMNpx.L8nfQYPooBOoACK8rCuGtaxifRhL1J6", new DateTime(2026, 7, 15, 15, 5, 0, 404, DateTimeKind.Utc).AddTicks(7192) });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFiles",
                column: "SubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionFiles");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 15, 12, 17, 19, 259, DateTimeKind.Utc).AddTicks(24), "$2a$11$u75VH7DOBqO/0YnoiacWwuJ2B6FybQwRBThBMxyMXFgUwM3MREMuO", new DateTime(2026, 7, 15, 12, 17, 19, 259, DateTimeKind.Utc).AddTicks(288) });
        }
    }
}
