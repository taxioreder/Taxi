using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class Update080819 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DateAccept",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeAccept",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAccept",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TimeAccept",
                table: "Orders");
        }
    }
}