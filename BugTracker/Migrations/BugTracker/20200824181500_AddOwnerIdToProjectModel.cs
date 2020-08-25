using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Migrations.BugTracker
{
    public partial class AddOwnerIdToProjectModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Projects");
        }
    }
}
