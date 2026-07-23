using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NVLearnHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxAttemptsToAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxAttempts",
                table: "Assessments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAttempts",
                table: "Assessments");
        }
    }
}
