'use client'

import { useProductsList } from '@/hooks/use-api'
import { type Product } from '@/lib/api'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Loader2, Plus, Package } from 'lucide-react'

export function ProductsOverview() {
  const { data: products, isLoading, error, refetch } = useProductsList()

  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Products</CardTitle>
          <CardDescription>Loading product data...</CardDescription>
        </CardHeader>
        <CardContent className="flex items-center justify-center py-8">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </CardContent>
      </Card>
    )
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Products</CardTitle>
          <CardDescription>Failed to load products</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-sm text-muted-foreground">
            {error.message || 'Unable to fetch product data'}
          </p>
          <Button onClick={() => refetch()} variant="outline">
            Try Again
          </Button>
        </CardContent>
      </Card>
    )
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <div>
          <CardTitle>Products</CardTitle>
          <CardDescription>Manage your product catalog</CardDescription>
        </div>
        <Button size="sm">
          <Plus className="h-4 w-4 mr-1" />
          Add Product
        </Button>
      </CardHeader>
      <CardContent>
        {products && products.items && products.items.length > 0 ? (
          <div className="space-y-4">
            <div className="flex items-center space-x-2">
              <Package className="h-5 w-5 text-primary" />
              <span className="text-lg font-semibold">{products.totalCount}</span>
              <span className="text-sm text-muted-foreground">
                {products.totalCount === 1 ? 'product' : 'products'} total
              </span>
            </div>
            <div className="grid gap-2">
              {products.items.slice(0, 3).map((product: Product) => (
                <div
                  key={product.id}
                  className="flex items-center justify-between p-2 border rounded-md"
                >
                  <div>
                    <p className="font-medium">{product.name}</p>
                    <p className="text-sm text-muted-foreground">
                      ${product.price.toFixed(2)}
                    </p>
                  </div>
                  <div className="flex gap-1">
                    {product.categories && product.categories.length > 0 ? (
                      <>
                        {product.categories.slice(0, 2).map((category, index) => (
                          <span key={index} className="text-xs bg-secondary px-2 py-1 rounded-full">
                            {category}
                          </span>
                        ))}
                        {product.categories.length > 2 && (
                          <span className="text-xs bg-muted px-2 py-1 rounded-full">
                            +{product.categories.length - 2}
                          </span>
                        )}
                      </>
                    ) : (
                      <span className="text-xs bg-muted px-2 py-1 rounded-full">
                        No category
                      </span>
                    )}
                  </div>
                </div>
              ))}
              {products.totalCount > 3 && (
                <Button variant="ghost" size="sm" className="justify-start">
                  View all {products.totalCount} products
                </Button>
              )}
            </div>
          </div>
        ) : (
          <div className="text-center py-8">
            <Package className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
            <p className="text-sm text-muted-foreground mb-4">
              No products found. Create your first product to get started.
            </p>
            <Button>
              <Plus className="h-4 w-4 mr-1" />
              Create Product
            </Button>
          </div>
        )}
      </CardContent>
    </Card>
  )
}