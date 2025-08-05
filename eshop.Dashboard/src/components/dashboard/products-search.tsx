'use client'

import { useState } from 'react'
import { useProducts } from '@/hooks/use-api'
import { type ProductSearchParams } from '@/lib/api'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Loader2, Search, Filter, ChevronLeft, ChevronRight } from 'lucide-react'

export function ProductsSearch() {
  const [searchParams, setSearchParams] = useState<ProductSearchParams>({
    searchTerm: null,
    category: null,
    minPrice: null,
    maxPrice: null,
    isInStock: null,
    sortBy: null,
    sortOrder: null,
    page: 1,
    pageSize: 10,
  })

  const { data: products, isLoading, error, refetch } = useProducts(searchParams)

  const handleSearchChange = (field: keyof ProductSearchParams, value: any) => {
    setSearchParams(prev => ({
      ...prev,
      [field]: value === '' ? null : value,
      page: field !== 'page' ? 1 : value, // Reset to page 1 when changing filters
    }))
  }

  const nextPage = () => {
    handleSearchChange('page', (searchParams.page || 1) + 1)
  }

  const prevPage = () => {
    handleSearchChange('page', Math.max((searchParams.page || 1) - 1, 1))
  }

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Search className="h-5 w-5" />
          Product Search
        </CardTitle>
        <CardDescription>
          Search and filter products with advanced options
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Search Filters */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div className="space-y-2">
            <Label htmlFor="searchTerm">Search Term</Label>
            <Input
              id="searchTerm"
              placeholder="Search products..."
              value={searchParams.searchTerm || ''}
              onChange={(e) => handleSearchChange('searchTerm', e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="category">Category</Label>
            <Input
              id="category"
              placeholder="Category name..."
              value={searchParams.category || ''}
              onChange={(e) => handleSearchChange('category', e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="minPrice">Min Price</Label>
            <Input
              id="minPrice"
              type="number"
              placeholder="0.00"
              value={searchParams.minPrice || ''}
              onChange={(e) => handleSearchChange('minPrice', e.target.value ? parseFloat(e.target.value) : null)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="maxPrice">Max Price</Label>
            <Input
              id="maxPrice"
              type="number"
              placeholder="999.99"
              value={searchParams.maxPrice || ''}
              onChange={(e) => handleSearchChange('maxPrice', e.target.value ? parseFloat(e.target.value) : null)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="sortBy">Sort By</Label>
            <select
              id="sortBy"
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              value={searchParams.sortBy || ''}
              onChange={(e) => handleSearchChange('sortBy', e.target.value)}
            >
              <option value="">Default</option>
              <option value="name">Name</option>
              <option value="price">Price</option>
              <option value="category">Category</option>
              <option value="createdAt">Created Date</option>
            </select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="sortOrder">Sort Order</Label>
            <select
              id="sortOrder"
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              value={searchParams.sortOrder || ''}
              onChange={(e) => handleSearchChange('sortOrder', e.target.value as 'asc' | 'desc')}
            >
              <option value="">Default</option>
              <option value="asc">Ascending</option>
              <option value="desc">Descending</option>
            </select>
          </div>
        </div>

        {/* Filter Actions */}
        <div className="flex gap-2">
          <Button
            onClick={() => setSearchParams({
              searchTerm: null,
              category: null,
              minPrice: null,
              maxPrice: null,
              isInStock: null,
              sortBy: null,
              sortOrder: null,
              page: 1,
              pageSize: 10,
            })}
            variant="outline"
          >
            <Filter className="h-4 w-4 mr-2" />
            Clear Filters
          </Button>
          <Button onClick={() => refetch()} variant="outline">
            Refresh
          </Button>
        </div>

        {/* Results */}
        {isLoading ? (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
            <span className="ml-2">Searching products...</span>
          </div>
        ) : error ? (
          <div className="text-center py-8">
            <p className="text-sm text-muted-foreground mb-4">
              {error.message || 'Failed to search products'}
            </p>
            <Button onClick={() => refetch()} variant="outline">
              Try Again
            </Button>
          </div>
        ) : products ? (
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <p className="text-sm text-muted-foreground">
                Found {products.totalCount} products (showing {products.items?.length || 0})
              </p>
              <div className="flex items-center gap-2">
                <Button
                  onClick={prevPage}
                  disabled={!products.hasPreviousPage}
                  variant="outline"
                  size="sm"
                >
                  <ChevronLeft className="h-4 w-4" />
                  Previous
                </Button>
                <span className="text-sm text-muted-foreground">
                  Page {products.page || 1}
                </span>
                <Button
                  onClick={nextPage}
                  disabled={!products.hasNextPage}
                  variant="outline"
                  size="sm"
                >
                  Next
                  <ChevronRight className="h-4 w-4" />
                </Button>
              </div>
            </div>

            <div className="grid gap-4">
              {products.items?.map((product: any, index: number) => (
                <div
                  key={product.id || index}
                  className="flex items-center justify-between p-4 border rounded-lg hover:bg-accent/50 transition-colors"
                >
                  <div className="space-y-1">
                    <h3 className="font-semibold">{product.name || 'Unnamed Product'}</h3>
                    <p className="text-sm text-muted-foreground">
                      {product.description || 'No description'}
                    </p>
                    <div className="flex items-center gap-2 text-xs text-muted-foreground">
                      <span>Code: {product.productCode}</span>
                      <span>Categories: {product.categories?.join(', ') || 'None'}</span>
                      <span className={product.stock > 0 ? 'text-green-600' : 'text-red-600'}>
                        Stock: {product.stock}
                      </span>
                      {product.discountPercentage > 0 && (
                        <span className="text-orange-600">
                          {product.discountPercentage}% off
                        </span>
                      )}
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="font-semibold">${product.price || '0.00'}</p>
                    <Button size="sm" variant="outline">
                      Edit
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ) : (
          <div className="text-center py-8">
            <p className="text-sm text-muted-foreground">
              No products found. Try adjusting your search criteria.
            </p>
          </div>
        )}
      </CardContent>
    </Card>
  )
}