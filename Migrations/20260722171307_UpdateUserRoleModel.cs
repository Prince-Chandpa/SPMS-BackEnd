using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spm_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRoleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProﬁlePicturePath",
                table: "Users",
                newName: "ProfilePicturePath");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Users",
                newName: "MobileNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "UserTypes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePicturePath",
                table: "Users",
                newName: "ProﬁlePicturePath");

            migrationBuilder.RenameColumn(
                name: "MobileNumber",
                table: "Users",
                newName: "PhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "UserTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);
        }
    }
}
