'use client'

import { useState } from 'react'
import { useProducts, useDeleteProduct } from '@/hooks/use-api'
import { type ProductSearchParams, type Product } from '@/lib/api'
import { ProductViewDialog } from '@/components/dashboard/product-view-dialog'
import { EditProductDialog } from '@/components/dashboard/edit-product-dialog'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar'
import { Badge } from '@/components/ui/badge'
import { HoverCard, HoverCardContent, HoverCardTrigger } from '@/components/ui/hover-card'
import { 
  AlertDialog, 
  AlertDialogAction, 
  AlertDialogCancel, 
  AlertDialogContent, 
  AlertDialogDescription, 
  AlertDialogFooter, 
  AlertDialogHeader, 
  AlertDialogTitle 
} from '@/components/ui/alert-dialog'
import { Loader2, Search, Filter, ChevronLeft, ChevronRight, Edit, Package, Trash2 } from 'lucide-react'
import { toast } from 'sonner'



export function ProductsDataTable() {
  const [searchParams, setSearchParams] = useState<ProductSearchParams>({
    searchTerm: null,
    category: null,
    minPrice: null,
    maxPrice: null,
    isInStock: null,
    sortBy: null,
    sortOrder: null,
    page: 1,
    pageSize: 20, // Increased for better data table experience
  })

  const { data: products, isLoading, error, refetch } = useProducts(searchParams)
  
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null)
  const [viewDialogOpen, setViewDialogOpen] = useState(false)
  const [editDialogOpen, setEditDialogOpen] = useState(false)
  const [editProductId, setEditProductId] = useState<string | null>(null)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [productToDelete, setProductToDelete] = useState<Product | null>(null)

  const deleteProduct = useDeleteProduct()

  const handleProductClick = (product: Product) => {
    setSelectedProductId(product.id)
    setViewDialogOpen(true)
  }

  const handleEditClick = (product: Product, event: React.MouseEvent) => {
    event.stopPropagation() // Prevent row click
    setEditProductId(product.id)
    setEditDialogOpen(true)
  }

  const handleDeleteClick = (product: Product, event: React.MouseEvent) => {
    event.stopPropagation() // Prevent row click
    setProductToDelete(product)
    setDeleteDialogOpen(true)
  }

  const handleDeleteConfirm = () => {
    if (productToDelete) {
      deleteProduct.mutate(productToDelete.id, {
        onSuccess: () => {
          toast.success(`Product "${productToDelete.name}" has been deleted successfully`)
          setDeleteDialogOpen(false)
          setProductToDelete(null)
        },
        onError: (error) => {
          toast.error(`Failed to delete product: ${error.message}`)
        }
      })
    }
  }

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

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(price)
  }

  const getStockStatus = (stock: number) => {
    if (stock === 0) return { variant: 'destructive' as const, text: 'Out of Stock' }
    if (stock < 10) return { variant: 'secondary' as const, text: 'Low Stock' }
    return { variant: 'default' as const, text: 'In Stock' }
  }

  const truncateText = (text: string, maxLength: number = 80) => {
    if (text.length <= maxLength) return text
    return text.substring(0, maxLength).trim() + '...'
  }

  return (
    <>
      <Card className="w-full">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Package className="h-5 w-5" />
          Products Management
        </CardTitle>
        <CardDescription>
          Browse, search, and manage your product catalog
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Search Filters */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div className="space-y-2">
            <Label htmlFor="searchTerm">Search Products</Label>
            <Input
              id="searchTerm"
              placeholder="Search by name or code..."
              value={searchParams.searchTerm || ''}
              onChange={(e) => handleSearchChange('searchTerm', e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="category">Category</Label>
            <Input
              id="category"
              placeholder="Filter by category..."
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
        </div>

        {/* Sort Options */}
        <div className="flex gap-4">
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
              <option value="stock">Stock</option>
              <option value="createdAt">Created Date</option>
            </select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="sortOrder">Order</Label>
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

        {/* Actions */}
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
              pageSize: 20,
            })}
            variant="outline"
          >
            <Filter className="h-4 w-4 mr-2" />
            Clear Filters
          </Button>
          <Button onClick={() => refetch()} variant="outline">
            <Search className="h-4 w-4 mr-2" />
            Refresh
          </Button>
        </div>

        {/* Data Table */}
        {isLoading ? (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
            <span className="ml-2">Loading products...</span>
          </div>
        ) : error ? (
          <div className="text-center py-8">
            <p className="text-sm text-muted-foreground mb-4">
              {error.message || 'Failed to load products'}
            </p>
            <Button onClick={() => refetch()} variant="outline">
              Try Again
            </Button>
          </div>
        ) : products?.items ? (
          <div className="space-y-4">
            {/* Pagination Info */}
            <div className="flex items-center justify-between">
              <p className="text-sm text-muted-foreground">
                Showing {products.items.length} of {products.totalCount} products
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
                  Page {products.page}
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

            {/* Table */}
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="w-[100px]">Image</TableHead>
                    <TableHead>Product</TableHead>
                    <TableHead>Code</TableHead>
                    <TableHead>Price</TableHead>
                    <TableHead>Stock</TableHead>
                    <TableHead>Categories</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {products.items.map((product: Product) => {
                    const stockStatus = getStockStatus(product.stock)
                    const finalPrice = product.discountPercentage > 0
                      ? product.price * (1 - product.discountPercentage / 100)
                      : product.price

                    return (
                      <TableRow 
                        key={product.id}
                        className="cursor-pointer hover:bg-muted/50 transition-colors"
                        onClick={() => handleProductClick(product)}
                      >
                        <TableCell>
                          <HoverCard>
                            <HoverCardTrigger asChild>
                              <div className="cursor-pointer">
                                <Avatar className="h-12 w-12">
                                  <AvatarImage 
                                    src={product.coverPictureUrl} 
                                    alt={product.name}
                                    className="object-cover"
                                  />
                                  <AvatarFallback>
                                    <Package className="h-6 w-6" />
                                  </AvatarFallback>
                                </Avatar>
                              </div>
                            </HoverCardTrigger>
                            <HoverCardContent className="w-80 p-0">
                              <div className="relative">
                                {product.coverPictureUrl ? (
                                  <img
                                    src={product.coverPictureUrl}
                                    alt={product.name}
                                    className="w-full h-64 object-cover rounded-md"
                                  />
                                ) : (
                                  <div className="w-full h-64 bg-muted rounded-md flex items-center justify-center">
                                    <Package className="h-16 w-16 text-muted-foreground" />
                                  </div>
                                )}
                                <div className="p-3">
                                  <h4 className="text-sm font-semibold">{product.name}</h4>
                                  <p className="text-xs text-muted-foreground mt-1">
                                    {product.productCode}
                                  </p>
                                </div>
                              </div>
                            </HoverCardContent>
                          </HoverCard>
                        </TableCell>
                        
                        <TableCell>
                          <div className="space-y-1">
                            <p className="font-medium leading-none">{product.name}</p>
                            <p className="text-sm text-muted-foreground" title={product.description}>
                              {truncateText(product.description, 40)}
                            </p>
                          </div>
                        </TableCell>
                        
                        <TableCell>
                          <code className="text-sm bg-muted px-2 py-1 rounded">
                            {product.productCode}
                          </code>
                        </TableCell>
                        
                        <TableCell>
                          <div className="space-y-1">
                            <p className="font-medium">
                              {formatPrice(finalPrice)}
                            </p>
                            {product.discountPercentage > 0 && (
                              <div className="flex items-center gap-2">
                                <p className="text-sm text-muted-foreground line-through">
                                  {formatPrice(product.price)}
                                </p>
                                <Badge variant="destructive" className="text-xs">
                                  -{product.discountPercentage}%
                                </Badge>
                              </div>
                            )}
                          </div>
                        </TableCell>
                        
                        <TableCell>
                          <div className="space-y-1">
                            <p className="font-medium">{product.stock}</p>
                            <Badge variant={stockStatus.variant} className="text-xs">
                              {stockStatus.text}
                            </Badge>
                          </div>
                        </TableCell>
                        
                        <TableCell>
                          <div className="flex flex-wrap gap-1">
                            {product.categories && product.categories.length > 0 ? (
                              product.categories.slice(0, 2).map((category, index) => (
                                <Badge key={index} variant="outline" className="text-xs">
                                  {category}
                                </Badge>
                              ))
                            ) : (
                              <Badge variant="outline" className="text-xs">
                                No category
                              </Badge>
                            )}
                            {product.categories && product.categories.length > 2 && (
                              <Badge variant="outline" className="text-xs">
                                +{product.categories.length - 2}
                              </Badge>
                            )}
                          </div>
                        </TableCell>
                        
                        <TableCell className="text-right">
                          <div className="flex flex-col gap-1">
                            <Button 
                              size="sm" 
                              variant="outline" 
                              className="h-7 px-2 text-xs"
                              onClick={(e) => handleEditClick(product, e)}
                            >
                              <Edit className="h-3 w-3 mr-1" />
                              Edit
                            </Button>
                            <Button 
                              size="sm" 
                              variant="outline" 
                              className="h-7 px-2 text-xs text-red-600 hover:text-red-700 hover:bg-red-50"
                              onClick={(e) => handleDeleteClick(product, e)}
                            >
                              <Trash2 className="h-3 w-3 mr-1" />
                              Delete
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    )
                  })}
                </TableBody>
              </Table>
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
      
      {/* Product View Dialog */}
      <ProductViewDialog
        productId={selectedProductId}
        open={viewDialogOpen}
        onOpenChange={(open) => {
          setViewDialogOpen(open)
          if (!open) {
            setSelectedProductId(null) // Clear selected product when dialog closes
          }
        }}
      />

      {/* Edit Product Dialog */}
      <EditProductDialog
        productId={editProductId}
        open={editDialogOpen}
        onOpenChange={(open) => {
          setEditDialogOpen(open)
          if (!open) {
            setEditProductId(null) // Clear selected product when dialog closes
          }
        }}
      />

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete the product 
              <strong> "{productToDelete?.name}"</strong> from your store.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction 
              onClick={handleDeleteConfirm}
              className="bg-red-600 hover:bg-red-700"
              disabled={deleteProduct.isPending}
            >
              {deleteProduct.isPending ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Deleting...
                </>
              ) : (
                'Delete Product'
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  )
}