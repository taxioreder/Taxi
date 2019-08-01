using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBAplication.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Login = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    KeyAuthorized = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Geolocations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geolocations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OrderMobiles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdDriver = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StatusDrive = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMobiles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    TokenShope = table.Column<string>(nullable: true),
                    ZipCod = table.Column<string>(nullable: true),
                    IsWork = table.Column<bool>(nullable: false),
                    geolocationsID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Drivers_Geolocations_geolocationsID",
                        column: x => x.geolocationsID,
                        principalTable: "Geolocations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OnePointForAddressOrder",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IDorder = table.Column<int>(nullable: false),
                    Lat = table.Column<double>(nullable: false),
                    Lng = table.Column<double>(nullable: false),
                    PTime = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
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

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentStatus = table.Column<string>(nullable: true),
                    CurrentOrder = table.Column<string>(nullable: true),
                    NoName = table.Column<string>(nullable: true),
                    NoName1 = table.Column<string>(nullable: true),
                    NameCustomer = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    NoName2 = table.Column<string>(nullable: true),
                    TimeOfPickup = table.Column<string>(nullable: true),
                    TimeOfAppointment = table.Column<string>(nullable: true),
                    FromAddress = table.Column<string>(nullable: true),
                    FromZip = table.Column<int>(nullable: false),
                    ToAddress = table.Column<string>(nullable: true),
                    ToZip = table.Column<int>(nullable: false),
                    Milisse = table.Column<string>(nullable: true),
                    Price = table.Column<string>(nullable: true),
                    NoName3 = table.Column<string>(nullable: true),
                    NoName4 = table.Column<string>(nullable: true),
                    NoName5 = table.Column<string>(nullable: true),
                    NoName6 = table.Column<string>(nullable: true),
                    CountCustomer = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    FB = table.Column<string>(nullable: true),
                    isValid = table.Column<bool>(nullable: false),
                    isAccept = table.Column<bool>(nullable: false),
                    IsVisableAccept = table.Column<bool>(nullable: false),
                    DriverID = table.Column<int>(nullable: true),
                    OrderMobileID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orders_Drivers_DriverID",
                        column: x => x.DriverID,
                        principalTable: "Drivers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderMobiles_OrderMobileID",
                        column: x => x.OrderMobileID,
                        principalTable: "OrderMobiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_geolocationsID",
                table: "Drivers",
                column: "geolocationsID");

            migrationBuilder.CreateIndex(
                name: "IX_OnePointForAddressOrder_OrderMobileID",
                table: "OnePointForAddressOrder",
                column: "OrderMobileID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DriverID",
                table: "Orders",
                column: "DriverID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderMobileID",
                table: "Orders",
                column: "OrderMobileID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "OnePointForAddressOrder");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "OrderMobiles");

            migrationBuilder.DropTable(
                name: "Geolocations");
        }
    }
}
