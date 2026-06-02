using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnsNotNullRiskZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DB_RISK_ZONE_DB_RISK_ASSESSMENTS_RISK_ASSESSMENT_ID",
                table: "DB_RISK_ZONE");

            migrationBuilder.AlterColumn<int>(
                name: "RISK_ASSESSMENT_ID",
                table: "DB_RISK_ZONE",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RADIUS_KM",
                table: "DB_RISK_ZONE",
                type: "NUMBER(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "NUMBER(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ALERT_LEVEL",
                table: "DB_RISK_ZONE",
                type: "VARCHAR2(6)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(6)");

            migrationBuilder.AddForeignKey(
                name: "FK_DB_RISK_ZONE_DB_RISK_ASSESSMENTS_RISK_ASSESSMENT_ID",
                table: "DB_RISK_ZONE",
                column: "RISK_ASSESSMENT_ID",
                principalTable: "DB_RISK_ASSESSMENTS",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DB_RISK_ZONE_DB_RISK_ASSESSMENTS_RISK_ASSESSMENT_ID",
                table: "DB_RISK_ZONE");

            migrationBuilder.AlterColumn<int>(
                name: "RISK_ASSESSMENT_ID",
                table: "DB_RISK_ZONE",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RADIUS_KM",
                table: "DB_RISK_ZONE",
                type: "NUMBER(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "NUMBER(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ALERT_LEVEL",
                table: "DB_RISK_ZONE",
                type: "VARCHAR2(6)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR2(6)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DB_RISK_ZONE_DB_RISK_ASSESSMENTS_RISK_ASSESSMENT_ID",
                table: "DB_RISK_ZONE",
                column: "RISK_ASSESSMENT_ID",
                principalTable: "DB_RISK_ASSESSMENTS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
