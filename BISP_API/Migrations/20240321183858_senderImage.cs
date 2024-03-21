using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BISP_API.Migrations
{
    public partial class senderImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenderImage",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderImage",
                table: "Messages");
        }
    }
}
