using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eshop.Infrastructure.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class FixOrderHistoryTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusHistory_Orders_OrderId",
                table: "OrderStatusHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStatusHistory",
                table: "OrderStatusHistory");

            migrationBuilder.RenameTable(
                name: "OrderStatusHistory",
                newName: "OrderStatusHistories");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatusHistory_OrderId",
                table: "OrderStatusHistories",
                newName: "IX_OrderStatusHistories_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStatusHistories",
                table: "OrderStatusHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusHistories_Orders_OrderId",
                table: "OrderStatusHistories",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusHistories_Orders_OrderId",
                table: "OrderStatusHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStatusHistories",
                table: "OrderStatusHistories");

            migrationBuilder.RenameTable(
                name: "OrderStatusHistories",
                newName: "OrderStatusHistory");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatusHistories_OrderId",
                table: "OrderStatusHistory",
                newName: "IX_OrderStatusHistory_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStatusHistory",
                table: "OrderStatusHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusHistory_Orders_OrderId",
                table: "OrderStatusHistory",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
