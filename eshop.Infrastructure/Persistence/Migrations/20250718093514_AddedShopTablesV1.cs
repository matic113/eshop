using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eshop.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedShopTablesV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    state = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    city = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    street = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apartment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_address", x => x.id);
                    table.ForeignKey(
                        name: "fk_address_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "offer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cover_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_offer", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    stock = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    cover_picture_url = table.Column<string>(type: "text", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(8,3)", nullable: false),
                    color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    discount_percentage = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_users_seller_id",
                        column: x => x.seller_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seller",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    store_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seller", x => x.id);
                    table.ForeignKey(
                        name: "fk_seller_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "category_product",
                columns: table => new
                {
                    categories_id = table.Column<Guid>(type: "uuid", nullable: false),
                    products_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_product", x => new { x.categories_id, x.products_id });
                    table.ForeignKey(
                        name: "fk_category_product_category_categories_id",
                        column: x => x.categories_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_category_product_product_products_id",
                        column: x => x.products_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_item_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_item_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_item_users_seller_id",
                        column: x => x.seller_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_pictures",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    picture_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_pictures", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_pictures_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<short>(type: "smallint", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_review", x => x.id);
                    table.ForeignKey(
                        name: "fk_review_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_review_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_address_user_id",
                table: "address",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_category_product_products_id",
                table: "category_product",
                column: "products_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_order_number",
                table: "order",
                column: "order_number");

            migrationBuilder.CreateIndex(
                name: "ix_order_user_id",
                table: "order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_item_order_id",
                table: "order_item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_item_product_id",
                table: "order_item",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_item_seller_id",
                table: "order_item",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_product_code",
                table: "product",
                column: "product_code");

            migrationBuilder.CreateIndex(
                name: "ix_product_seller_id",
                table: "product",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_pictures_product_id",
                table: "product_pictures",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_review_product_id",
                table: "review",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_review_user_id",
                table: "review",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_seller_user_id",
                table: "seller",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "category_product");

            migrationBuilder.DropTable(
                name: "offer");

            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "product_pictures");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "seller");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "product");
        }
    }
}
