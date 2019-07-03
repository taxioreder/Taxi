using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class Update030719_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeTime",
                table: "OnePointForAddressOrder",
                newName: "PTime");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "OnePointForAddressOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "OnePointForAddressOrder");

            migrationBuilder.RenameColumn(
                name: "PTime",
                table: "OnePointForAddressOrder",
                newName: "TypeTime");
        }
    }
}
