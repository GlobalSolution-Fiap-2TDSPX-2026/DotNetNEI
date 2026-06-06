using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEI.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DB_USERS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    ROLE = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DB_USERS", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DB_USERS_EMAIL",
                table: "DB_USERS",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DB_USERS_USERNAME",
                table: "DB_USERS",
                column: "USERNAME",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DB_USERS");
        }
    }
}
