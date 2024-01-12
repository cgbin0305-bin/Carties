using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionService.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SOldAmount",
                table: "Auctions",
                newName: "SoldAmount");

            migrationBuilder.RenameColumn(
                name: "CurrenntHighBid",
                table: "Auctions",
                newName: "CurrentHighBid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SoldAmount",
                table: "Auctions",
                newName: "SOldAmount");

            migrationBuilder.RenameColumn(
                name: "CurrentHighBid",
                table: "Auctions",
                newName: "CurrenntHighBid");
        }
    }
}
