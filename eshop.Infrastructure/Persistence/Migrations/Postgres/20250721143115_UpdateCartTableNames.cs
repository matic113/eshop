using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eshop.Infrastructure.Persistence.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class UpdateCartTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cart_item_cart_cart_id",
                table: "cart_item");

            migrationBuilder.DropForeignKey(
                name: "fk_cart_item_products_product_id",
                table: "cart_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_cart_item",
                table: "cart_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_cart",
                table: "cart");

            migrationBuilder.RenameTable(
                name: "cart_item",
                newName: "cart_items");

            migrationBuilder.RenameTable(
                name: "cart",
                newName: "carts");

            migrationBuilder.RenameIndex(
                name: "ix_cart_item_product_id",
                table: "cart_items",
                newName: "ix_cart_items_product_id");

            migrationBuilder.RenameIndex(
                name: "ix_cart_item_cart_id",
                table: "cart_items",
                newName: "ix_cart_items_cart_id");

            migrationBuilder.RenameIndex(
                name: "ix_cart_user_id",
                table: "carts",
                newName: "ix_carts_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_cart_items",
                table: "cart_items",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_carts",
                table: "carts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_cart_items_carts_cart_id",
                table: "cart_items",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cart_items_products_product_id",
                table: "cart_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cart_items_carts_cart_id",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "fk_cart_items_products_product_id",
                table: "cart_items");

            migrationBuilder.DropPrimaryKey(
                name: "pk_carts",
                table: "carts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_cart_items",
                table: "cart_items");

            migrationBuilder.RenameTable(
                name: "carts",
                newName: "cart");

            migrationBuilder.RenameTable(
                name: "cart_items",
                newName: "cart_item");

            migrationBuilder.RenameIndex(
                name: "ix_carts_user_id",
                table: "cart",
                newName: "ix_cart_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_cart_items_product_id",
                table: "cart_item",
                newName: "ix_cart_item_product_id");

            migrationBuilder.RenameIndex(
                name: "ix_cart_items_cart_id",
                table: "cart_item",
                newName: "ix_cart_item_cart_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_cart",
                table: "cart",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_cart_item",
                table: "cart_item",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_cart_item_cart_cart_id",
                table: "cart_item",
                column: "cart_id",
                principalTable: "cart",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cart_item_products_product_id",
                table: "cart_item",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
