using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEI.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitialTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DB_ASTEROIDS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NASA_ID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESTIMATED_DIAMETER_MIN_KM = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    ESTIMATED_DIAMETER_MAX_KM = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    ABSOLUTE_MAGNITUDE = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    IS_POTENTIALLY_DANGEROUS = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DB_ASTEROIDS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DB_CLOSE_APPROACH",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ASTEROID_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    APPROACH_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    MISS_DISTANCE_KM = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    RELATIVE_VELOCITY_KM = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    ORBITING_BODY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DB_CLOSE_APPROACH", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DB_CLOSE_APPROACH_DB_ASTEROIDS_ASTEROID_ID",
                        column: x => x.ASTEROID_ID,
                        principalTable: "DB_ASTEROIDS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DB_RISK_ASSESSMENTS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AsteroidId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RISK_LEVEL = table.Column<string>(type: "VARCHAR2(7)", nullable: false),
                    MISS_DISTANCE_KM = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    SAFE_DISTANCE_THRESHOLD_KM = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    ASSESSED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DB_RISK_ASSESSMENTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DB_RISK_ASSESSMENTS_DB_ASTEROIDS_AsteroidId",
                        column: x => x.AsteroidId,
                        principalTable: "DB_ASTEROIDS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DB_RISK_ZONE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RISK_ASSESSMENT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    REGION_NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LATITUDE = table.Column<decimal>(type: "NUMBER(5,2)", nullable: false),
                    LONGITUDE = table.Column<decimal>(type: "NUMBER(5,2)", nullable: false),
                    RADIUS_KM = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    ALERT_LEVEL = table.Column<string>(type: "VARCHAR2(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DB_RISK_ZONE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DB_RISK_ZONE_DB_RISK_ASSESSMENTS_RISK_ASSESSMENT_ID",
                        column: x => x.RISK_ASSESSMENT_ID,
                        principalTable: "DB_RISK_ASSESSMENTS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DB_ASTEROIDS_NASA_ID",
                table: "DB_ASTEROIDS",
                column: "NASA_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DB_CLOSE_APPROACH_ASTEROID_ID",
                table: "DB_CLOSE_APPROACH",
                column: "ASTEROID_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DB_RISK_ASSESSMENTS_AsteroidId",
                table: "DB_RISK_ASSESSMENTS",
                column: "AsteroidId");

            migrationBuilder.CreateIndex(
                name: "IX_DB_RISK_ZONE_RISK_ASSESSMENT_ID",
                table: "DB_RISK_ZONE",
                column: "RISK_ASSESSMENT_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DB_CLOSE_APPROACH");

            migrationBuilder.DropTable(
                name: "DB_RISK_ZONE");

            migrationBuilder.DropTable(
                name: "DB_RISK_ASSESSMENTS");

            migrationBuilder.DropTable(
                name: "DB_ASTEROIDS");
        }
    }
}
