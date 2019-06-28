using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class Update28062019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderMobileID",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderMobiles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdDriver = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMobiles", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderMobileID",
                table: "Orders",
                column: "OrderMobileID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderMobiles_OrderMobileID",
                table: "Orders",
                column: "OrderMobileID",
                principalTable: "OrderMobiles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderMobiles_OrderMobileID",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderMobiles");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderMobileID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderMobileID",
                table: "Orders");
        }
    }
}
