using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class update08072019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusNow",
                table: "OnePointForAddressOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusNow",
                table: "OnePointForAddressOrder",
                nullable: true);
        }
    }
}
