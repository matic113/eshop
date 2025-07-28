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
                var searchQuery = from product in _context.Products
                                  join fts in ftsResults on product.Id equals fts.ProductId
                                  select new { product, fts.Rank };

                // B. Apply FILTERS to the search results (accessing product via 'x.product')

                // TODO: Look into category filtering
                //if (request.CategoryId.HasValue)
                //{
                //    searchQuery = searchQuery.Where(x => x.product.CategoryId == request.CategoryId.Value);
                //}
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

                // TODO: look into category filtering
                //if (request.CategoryId.HasValue)
                //{
                //    productsQuery = productsQuery.Where(p => p.CategoryId == request.CategoryId.Value);
                //}
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
                        // TODO: switch to creation date --> add CreatedAt To Model
                        _ => productsQuery.OrderByDescending(p => p.Id) // Default sort
                    };
                }
                else
                {
                    // TODO: switch to creation date --> add CreatedAt To Model
                    // Default sort when no search or sort criteria are provided
                    productsQuery = productsQuery.OrderByDescending(p => p.Id);
                }

                finalQuery = productsQuery;
            }

            // FINAL STEP: Create the paged list from the prepared finalQuery
            var pagedList = await finalQuery
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
                    DiscountPercentage = p.DiscountPercentage,
                    SellerId = p.SellerId
                }).ToPagedListAsync(request.Page, request.PageSize);

            return pagedList;
        }

        public async Task UpdateProductsBulkAsync(IEnumerable<Product> products)
        {
            _context.Products.UpdateRange(products);
            return;
        }
    }
}
