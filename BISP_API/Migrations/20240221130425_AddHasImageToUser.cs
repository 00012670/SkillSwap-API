using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BISP_API.Migrations
{
    public partial class AddHasImageToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasImage",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasImage",
                table: "Users");
        }
    }
}
