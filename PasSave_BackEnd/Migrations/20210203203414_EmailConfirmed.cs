using Microsoft.EntityFrameworkCore.Migrations;

namespace PasSave_BackEnd.Migrations
{
    public partial class EmailConfirmed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "User");
        }
    }
}
