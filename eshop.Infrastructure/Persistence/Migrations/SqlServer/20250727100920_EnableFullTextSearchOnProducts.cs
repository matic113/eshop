using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eshop.Infrastructure.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class EnableFullTextSearchOnProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    
                    -- Check if the full-text catalog exists, if not create it
                    IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'ProductCatalog')
                    BEGIN
                        CREATE FULLTEXT CATALOG ProductCatalog AS DEFAULT;
                    END;

                    -- Check if the full-text index exists on Products table, if not create it
                    IF NOT EXISTS (
                        SELECT * FROM sys.fulltext_indexes fi
                        JOIN sys.objects o ON fi.object_id = o.object_id
                        WHERE o.name = 'Products'
                    )
                    BEGIN
                        CREATE FULLTEXT INDEX ON Products
                        (
                            NameArabic LANGUAGE 1025,
                            DescriptionArabic LANGUAGE 1025,
                            Name LANGUAGE 1033,
                            Description LANGUAGE 1033
                        )
                        KEY INDEX PK_Products
                        ON ProductCatalog;
                    END;
                    ", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    IF EXISTS (
                        SELECT * FROM sys.fulltext_indexes fi
                        JOIN sys.objects o ON fi.object_id = o.object_id
                        WHERE o.name = 'Products'
                    )
                    BEGIN
                        DROP FULLTEXT INDEX ON Products;
                    END;

                    IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'ProductCatalog')
                    BEGIN
                        DROP FULLTEXT CATALOG ProductCatalog;
                    END;
                    ", suppressTransaction: true);
        }
    }
}
