using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class update010519 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentStatus = table.Column<string>(nullable: true),
                    NoName = table.Column<string>(nullable: true),
                    NoName1 = table.Column<string>(nullable: true),
                    NameCustomer = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    NoName2 = table.Column<string>(nullable: true),
                    TimeOfPickup = table.Column<string>(nullable: true),
                    TimeOfAppointment = table.Column<string>(nullable: true),
                    FromAddress = table.Column<string>(nullable: true),
                    ToAddress = table.Column<string>(nullable: true),
                    Milisse = table.Column<string>(nullable: true),
                    Price = table.Column<string>(nullable: true),
                    NoName3 = table.Column<string>(nullable: true),
                    NoName4 = table.Column<string>(nullable: true),
                    NoName5 = table.Column<string>(nullable: true),
                    NoName6 = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
