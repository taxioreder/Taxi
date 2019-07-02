using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class update02072019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnePointForAddressOrder",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Lat = table.Column<double>(nullable: false),
                    Lng = table.Column<double>(nullable: false),
                    typeTime = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    OrderMobileID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnePointForAddressOrder", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OnePointForAddressOrder_OrderMobiles_OrderMobileID",
                        column: x => x.OrderMobileID,
                        principalTable: "OrderMobiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnePointForAddressOrder_OrderMobileID",
                table: "OnePointForAddressOrder",
                column: "OrderMobileID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnePointForAddressOrder");
        }
    }
}
