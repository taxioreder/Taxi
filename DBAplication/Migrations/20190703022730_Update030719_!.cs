using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class Update030719_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "typeTime",
                table: "OnePointForAddressOrder",
                newName: "TypeTime");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "OnePointForAddressOrder",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "IDorder",
                table: "OnePointForAddressOrder",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDorder",
                table: "OnePointForAddressOrder");

            migrationBuilder.RenameColumn(
                name: "TypeTime",
                table: "OnePointForAddressOrder",
                newName: "typeTime");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "OnePointForAddressOrder",
                newName: "type");
        }
    }
}
