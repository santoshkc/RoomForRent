using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RoomForRent.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leasers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    ContactNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leasers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Renters",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    ContactNumber = table.Column<string>(nullable: false),
                    SeekedAsset = table.Column<int>(nullable: false),
                    Found = table.Column<bool>(nullable: true),
                    FoundDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Renters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RenterId = table.Column<int>(nullable: false),
                    LeaserId = table.Column<int>(nullable: false),
                    TransactionStatus = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Asset",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    IsLeased = table.Column<bool>(nullable: true),
                    LeasedDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Asset_Leasers_ID",
                        column: x => x.ID,
                        principalTable: "Leasers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asset");

            migrationBuilder.DropTable(
                name: "Renters");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Leasers");
        }
    }
}
