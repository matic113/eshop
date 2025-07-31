using eshop.Application.Contracts.Repositories;
using eshop.Application.Dtos;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using eshop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedList<ProductDto>> SearchAndFilterProductsAsync(SearchAndFilterProductsRequest request)
        {
            // This will hold the final query after all logic is applied
            IQueryable<Product> finalQuery;

            // PATH 1: A search term is provided.
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var ftsResults = _context.ProductFtsResults.FromSqlInterpolated($"""
                SELECT [KEY] AS ProductId, RANK
                FROM CONTAINSTABLE(Products, (Name, Description), {request.SearchTerm}, LANGUAGE 'English')

                UNION ALL

                SELECT [KEY] AS ProductId, RANK
                FROM CONTAINSTABLE(Products, (NameArabic, DescriptionArabic), {request.SearchTerm}, LANGUAGE 'Arabic')
                """);

                // A. Join products with search results to get { Product, Rank }
                var searchQuery =
                    from product in _context.Products
                    join fts in ftsResults on product.Id equals fts.ProductId
                    select new { product, fts.Rank };

                // B. Apply FILTERS to the search results (accessing product via 'x.product')

                if (!string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    searchQuery = searchQuery
                        .Where(x => x.product.Categories.Any(c => c.Name == request.CategoryName));
                }
                if (request.MinPrice.HasValue)
                {
                    searchQuery = searchQuery.Where(x => x.product.Price >= request.MinPrice.Value);
                }
                if (request.MaxPrice.HasValue)
                {
                    searchQuery = searchQuery.Where(x => x.product.Price <= request.MaxPrice.Value);
                }
                if (request.IsInStock.HasValue && request.IsInStock.Value)
                {
                    searchQuery = searchQuery.Where(x => x.product.Stock > 0);
                }

                // C. Apply SORTING (primary sort is ALWAYS by Rank)
                var orderedQuery = searchQuery.OrderByDescending(x => x.Rank);

                // Apply secondary sorting if provided
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    bool isDescending = request.SortOrder?.ToLower() == "desc";
                    orderedQuery = request.SortBy.ToLower() switch
                    {
                        "price" => isDescending ? orderedQuery.ThenByDescending(x => x.product.Price) : orderedQuery.ThenBy(x => x.product.Price),
                        "name" => isDescending ? orderedQuery.ThenByDescending(x => x.product.Name) : orderedQuery.ThenBy(x => x.product.Name),
                        "rating" => isDescending ? orderedQuery.ThenByDescending(x => x.product.Rating) : orderedQuery.ThenBy(x => x.product.Rating),
                        "newest" => isDescending ? orderedQuery.ThenByDescending(x => x.product.CreatedAt) : orderedQuery.ThenBy(x => x.product.CreatedAt),
                        _ => orderedQuery // Keep original Rank sorting if key is invalid
                    };
                }

                // D. Project back to IQueryable<Product> to finalize
                finalQuery = orderedQuery.Select(x => x.product);
            }
            // PATH 2: No search term is provided.
            else
            {
                IQueryable<Product> productsQuery = _context.Products;

                // A. Apply FILTERS directly to products

                if (!string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    productsQuery = productsQuery
                        .Where(p => p.Categories.Any(c => c.Name == request.CategoryName));
                }
                if (request.MinPrice.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Price >= request.MinPrice.Value);
                }
                if (request.MaxPrice.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Price <= request.MaxPrice.Value);
                }
                if (request.IsInStock.HasValue && request.IsInStock.Value)
                {
                    productsQuery = productsQuery.Where(p => p.Stock > 0);
                }

                // B. Apply SORTING
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    bool isDescending = request.SortOrder?.ToLower() == "desc";
                    productsQuery = request.SortBy.ToLower() switch
                    {
                        "price" => isDescending ? productsQuery.OrderByDescending(p => p.Price) : productsQuery.OrderBy(p => p.Price),
                        "name" => isDescending ? productsQuery.OrderByDescending(p => p.Name) : productsQuery.OrderBy(p => p.Name),
                        "rating" => isDescending ? productsQuery.OrderByDescending(p => p.Rating) : productsQuery.OrderBy(p => p.Rating),
                        "newest" => isDescending ? productsQuery.OrderByDescending(p => p.CreatedAt) : productsQuery.OrderBy(p => p.CreatedAt),
                        // TODO: switch to creation date --> add CreatedAt To Model
                        _ => productsQuery.OrderByDescending(p => p.Id) // Default sort
                    };
                }
                else
                {
                    // Default sort when no search or sort criteria are provided
                    productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt);
                }

                finalQuery = productsQuery;
            }

            // FINAL STEP: Create the paged list from the prepared finalQuery
            var pagedProducts = await finalQuery
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    Name = p.Name,
                    Description = p.Description,
                    ArabicName = p.NameArabic,
                    ArabicDescription = p.DescriptionArabic,
                    CoverPictureUrl = p.CoverPictureUrl,
                    Price = p.Price,
                    Stock = p.Stock,
                    Weight = p.Weight,
                    Color = p.Color,
                    Rating = p.Rating,
                    ReviewsCount = p.ReviewsCount,
                    DiscountPercentage = p.DiscountPercentage,
                    SellerId = p.SellerId
                    // Note: The 'Categories' property is still an empty list at this point.
                })
                .ToPagedListAsync(request.Page, request.PageSize);

            // run a second, very fast query to get ONLY their categories.
            var productIdsOnPage = pagedProducts.Items.Select(p => p.Id).ToList();

            var categoriesForProducts = await _context.Products
                .Where(p => productIdsOnPage.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    Categories = p.Categories.Select(c => c.Name).ToList()
                })
                .ToListAsync();

            // STEP 3: Stitch the categories onto the products in memory. This is very fast.
            var categoriesLookup = categoriesForProducts.ToDictionary(x => x.Id, x => x.Categories);

            foreach (var productDto in pagedProducts.Items)
            {
                if (categoriesLookup.TryGetValue(productDto.Id, out var categories))
                {
                    productDto.Categories = categories;
                }
            }

            return pagedProducts;
        }

        public async Task UpdateProductsBulkAsync(IEnumerable<Product> products)
        {
            _context.Products.UpdateRange(products);
            return;
        }

        public async Task<Product?> GetProductWithPicturesAsync(Guid productId)
        {
            return await _context.Products
                .Include(p => p.ProductPictures)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }
    }
}
