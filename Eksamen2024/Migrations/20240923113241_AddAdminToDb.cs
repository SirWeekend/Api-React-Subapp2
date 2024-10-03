using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eksamen2024.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Pinpoints",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Comments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    HashedPassword = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pinpoints_AdminId",
                table: "Pinpoints",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AdminId",
                table: "Comments",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Admins_AdminId",
                table: "Comments",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pinpoints_Admins_AdminId",
                table: "Pinpoints",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Admins_AdminId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Pinpoints_Admins_AdminId",
                table: "Pinpoints");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_Pinpoints_AdminId",
                table: "Pinpoints");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AdminId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Pinpoints");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Comments");
        }
    }
}
