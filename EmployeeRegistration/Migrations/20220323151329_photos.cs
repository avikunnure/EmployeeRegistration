using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeRegistration.Migrations
{
    public partial class photos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "yearOfPassing",
                table: "EmployeesEducations",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "EmployeePhoto",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeePhoto",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "yearOfPassing",
                table: "EmployeesEducations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4);
        }
    }
}
