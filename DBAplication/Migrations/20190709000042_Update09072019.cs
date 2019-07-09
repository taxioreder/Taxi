using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class Update09072019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OnePointForAddressOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OnePointForAddressOrder");
        }
    }
}
