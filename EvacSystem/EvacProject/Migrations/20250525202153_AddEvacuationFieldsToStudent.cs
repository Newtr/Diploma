using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvacProject.Migrations
{
    /// <inheritdoc />
    public partial class AddEvacuationFieldsToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentState",
                table: "students",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedCampus",
                table: "students",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentState",
                table: "students");

            migrationBuilder.DropColumn(
                name: "SelectedCampus",
                table: "students");
        }
    }
}
