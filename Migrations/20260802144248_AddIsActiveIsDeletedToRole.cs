using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spm_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveIsDeletedToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProjectAllocations",
                columns: table => new
                {
                    ProjectAllocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    FacultyID = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalTasksGiven = table.Column<int>(type: "int", nullable: false),
                    TotalCompletedTasks = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverAllGrade = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAllocations", x => x.ProjectAllocationID);
                    table.ForeignKey(
                        name: "FK_ProjectAllocations_ProjectMasters_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMasters",
                        principalColumn: "ProjectMasterID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectAllocations_Users_FacultyID",
                        column: x => x.FacultyID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_ProjectAllocations_Users_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectAllocationID = table.Column<int>(type: "int", nullable: false),
                    TaskTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TaskStatusID = table.Column<int>(type: "int", nullable: false),
                    TaskPriorityID = table.Column<int>(type: "int", nullable: false),
                    AssignedScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EarnedScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaskAssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaskDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaskCompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextFollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FacultyRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StudentRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskID);
                    table.ForeignKey(
                        name: "FK_Tasks_ProjectAllocations_ProjectAllocationID",
                        column: x => x.ProjectAllocationID,
                        principalTable: "ProjectAllocations",
                        principalColumn: "ProjectAllocationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskPriorities_TaskPriorityID",
                        column: x => x.TaskPriorityID,
                        principalTable: "TaskPriorities",
                        principalColumn: "TaskPriorityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskStatuses_TaskStatusID",
                        column: x => x.TaskStatusID,
                        principalTable: "TaskStatuses",
                        principalColumn: "TaskStatusID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_FacultyID",
                table: "ProjectAllocations",
                column: "FacultyID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_ProjectID",
                table: "ProjectAllocations",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_StudentID",
                table: "ProjectAllocations",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectAllocationID",
                table: "Tasks",
                column: "ProjectAllocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskPriorityID",
                table: "Tasks",
                column: "TaskPriorityID");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskStatusID",
                table: "Tasks",
                column: "TaskStatusID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "ProjectAllocations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Roles");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
