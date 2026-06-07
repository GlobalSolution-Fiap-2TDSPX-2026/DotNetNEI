using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRiskLevelColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RISK_LEVEL",
                table: "DB_RISK_ASSESSMENTS",
                type: "VARCHAR2(8)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(7)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RISK_LEVEL",
                table: "DB_RISK_ASSESSMENTS",
                type: "VARCHAR2(7)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(8)");
        }
    }
}
