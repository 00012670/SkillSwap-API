using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BISP_API.Migrations
{
    public partial class s1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Prerequsties",
                table: "Skills",
                newName: "Prerequisites");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Prerequisites",
                table: "Skills",
                newName: "Prerequsties");
        }
    }
}
