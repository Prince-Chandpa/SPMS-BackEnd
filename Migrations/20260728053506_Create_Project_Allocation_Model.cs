using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spm_backend.Migrations
{
    /// <inheritdoc />
    public partial class Create_Project_Allocation_Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserTypeID",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeID",
                table: "Users",
                column: "UserTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserTypeID",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeID",
                table: "Users",
                column: "UserTypeID",
                unique: true);
        }
    }
}
