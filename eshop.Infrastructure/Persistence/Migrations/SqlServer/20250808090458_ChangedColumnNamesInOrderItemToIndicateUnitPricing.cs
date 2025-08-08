using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eshop.Infrastructure.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class ChangedColumnNamesInOrderItemToIndicateUnitPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "OrderItems",
                newName: "UnitTotalPrice");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                table: "OrderItems",
                newName: "UnitSubTotal");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "OrderItems",
                newName: "UnitDiscountAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitTotalPrice",
                table: "OrderItems",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "UnitSubTotal",
                table: "OrderItems",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "UnitDiscountAmount",
                table: "OrderItems",
                newName: "DiscountAmount");
        }
    }
}
