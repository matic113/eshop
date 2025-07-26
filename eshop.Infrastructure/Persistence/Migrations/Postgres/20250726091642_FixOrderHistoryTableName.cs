using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eshop.Infrastructure.Persistence.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class FixOrderHistoryTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_status_history_orders_order_id",
                table: "order_status_history");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_status_history",
                table: "order_status_history");

            migrationBuilder.RenameTable(
                name: "order_status_history",
                newName: "order_status_histories");

            migrationBuilder.RenameIndex(
                name: "ix_order_status_history_order_id",
                table: "order_status_histories",
                newName: "ix_order_status_histories_order_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_status_histories",
                table: "order_status_histories",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_status_histories_orders_order_id",
                table: "order_status_histories",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_status_histories_orders_order_id",
                table: "order_status_histories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_status_histories",
                table: "order_status_histories");

            migrationBuilder.RenameTable(
                name: "order_status_histories",
                newName: "order_status_history");

            migrationBuilder.RenameIndex(
                name: "ix_order_status_histories_order_id",
                table: "order_status_history",
                newName: "ix_order_status_history_order_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_status_history",
                table: "order_status_history",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_status_history_orders_order_id",
                table: "order_status_history",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
