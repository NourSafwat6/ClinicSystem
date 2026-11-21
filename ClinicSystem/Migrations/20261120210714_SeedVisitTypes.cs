using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedVisitTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "visit_types",
                columns: new[] { "code", "duration_min", "name_ar", "patient_can_book", "price", "sort" },
                values: new object[,]
                {
                    { "followup", 30, "متابعة", true, 75, 3 },
                    { "kashf", 20, "كشف", true, 300, 1 },
                    { "reexam", 20, "إعادة كشف", true, 200, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "visit_types",
                keyColumn: "code",
                keyValue: "followup");

            migrationBuilder.DeleteData(
                table: "visit_types",
                keyColumn: "code",
                keyValue: "kashf");

            migrationBuilder.DeleteData(
                table: "visit_types",
                keyColumn: "code",
                keyValue: "reexam");
        }
    }
}
