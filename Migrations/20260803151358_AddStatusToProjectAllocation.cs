using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spm_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToProjectAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleID1",
                table: "UserRoles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProjectAllocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProjectAllocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleID1",
                table: "UserRoles",
                column: "RoleID1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleID1",
                table: "UserRoles",
                column: "RoleID1",
                principalTable: "Roles",
                principalColumn: "RoleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleID1",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_RoleID1",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "RoleID1",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProjectAllocations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProjectAllocations");
        }
    }
}
