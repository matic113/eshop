'use client'

import { useState, useEffect } from 'react'
import { Product } from '@/lib/api'
import { useImageUpload, useProduct } from '@/hooks/use-api'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'

import { 
  Package, 
  DollarSign, 
  Hash, 
  Weight, 
  Palette, 
  Archive, 
  Tag, 
  Calendar,
  Globe,
  Loader2,
  Star
} from 'lucide-react'

interface ProductViewDialogProps {
  productId: string | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function ProductViewDialog({ productId, open, onOpenChange }: ProductViewDialogProps) {
  const { data: product, isLoading, error } = useProduct(productId)
  const [productImages, setProductImages] = useState<string[]>([])

  // Update product images when product data loads
  useEffect(() => {
    if (product?.productPictures) {
      setProductImages(product.productPictures)
    } else if (product?.productPictureUrls) {
      setProductImages(product.productPictureUrls)
    }
  }, [product])

  if (!productId) return null

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price)
  }

  const formatDate = (dateString: string | null | undefined) => {
    if (!dateString) return 'N/A'
    
    try {
      const date = new Date(dateString)
      if (isNaN(date.getTime())) {
        return 'Invalid date'
      }
      
      return new Intl.DateTimeFormat('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }).format(date)
    } catch (error) {
      return 'Invalid date'
    }
  }

  if (isLoading) {
    return (
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="!max-w-6xl max-h-[90vh] overflow-y-auto sm:!max-w-6xl">
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin" />
            <span className="ml-2">Loading product details...</span>
          </div>
        </DialogContent>
      </Dialog>
    )
  }

  if (error) {
    return (
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="!max-w-6xl max-h-[90vh] overflow-y-auto sm:!max-w-6xl">
          <div className="flex flex-col items-center justify-center py-12">
            <div className="text-red-500 mb-2">Failed to load product details</div>
            <Button variant="outline" onClick={() => onOpenChange(false)}>
              Close
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    )
  }

  if (!product) return null

  const discountedPrice = product.discountPercentage > 0 
    ? product.price * (1 - product.discountPercentage / 100)
    : null

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="!max-w-6xl max-h-[90vh] overflow-y-auto sm:!max-w-6xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-3">
            <Package className="h-5 w-5" />
            {product.name}
            <Badge variant="outline">
              <Hash className="h-3 w-3 mr-1" />
              {product.productCode}
            </Badge>
          </DialogTitle>
          <DialogDescription>
            View and manage product details, images, and attributes.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          {/* Product Images */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Globe className="h-4 w-4" />
                Product Images
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 gap-6">
                {/* Cover Image - Left Half */}
                <div>
                  <h4 className="text-sm font-medium mb-3">Cover Image</h4>
                  {product.coverPictureUrl ? (
                    <div className="relative">
                      <img 
                        src={product.coverPictureUrl} 
                        alt={product.name}
                        className="w-full h-auto max-h-80 object-contain rounded-lg border bg-muted/20"
                      />
                    </div>
                  ) : (
                    <div className="w-full h-64 bg-muted rounded-lg flex items-center justify-center">
                      <Package className="h-12 w-12 text-muted-foreground" />
                    </div>
                  )}
                </div>

                {/* Product Images - Right Half */}
                <div>
                  <h4 className="text-sm font-medium mb-3">Product Images</h4>
                  {productImages.length > 0 ? (
                    <div className="grid grid-cols-2 gap-2 max-h-80 overflow-y-auto">
                      {productImages.map((url, index) => (
                        <div key={index} className="relative group">
                          <div className="aspect-square bg-muted/20 rounded-lg border overflow-hidden">
                            <img 
                              src={url} 
                              alt={`Product ${index + 1}`}
                              className="w-full h-full object-contain"
                            />
                          </div>

                        </div>
                      ))}
                    </div>
                  ) : (
                    <div className="w-full h-24 bg-muted rounded-lg flex items-center justify-center">
                      <p className="text-sm text-muted-foreground">No additional images</p>
                    </div>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Basic Information */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* English Information */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Globe className="h-4 w-4" />
                  English Information
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <h4 className="text-sm font-medium text-muted-foreground">Name</h4>
                  <p className="text-sm">{product.name}</p>
                </div>
                <div>
                  <h4 className="text-sm font-medium text-muted-foreground">Description</h4>
                  <p className="text-sm whitespace-pre-wrap">{product.description}</p>
                </div>
              </CardContent>
            </Card>

            {/* Arabic Information */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Globe className="h-4 w-4" />
                  Arabic Information
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <h4 className="text-sm font-medium text-muted-foreground">Arabic Name</h4>
                  <p className="text-sm" dir="rtl">{product.nameArabic}</p>
                </div>
                <div>
                  <h4 className="text-sm font-medium text-muted-foreground">Arabic Description</h4>
                  <p className="text-sm whitespace-pre-wrap" dir="rtl">{product.descriptionArabic}</p>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Pricing & Inventory */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <DollarSign className="h-4 w-4" />
                Pricing & Inventory
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
                <div className="space-y-1">
                  <h4 className="text-sm font-medium text-muted-foreground">Original Price</h4>
                  <p className="text-lg font-semibold">{formatPrice(product.price)}</p>
                </div>
                
                {product.discountPercentage > 0 && (
                  <>
                    <div className="space-y-1">
                      <h4 className="text-sm font-medium text-muted-foreground">Discount</h4>
                      <Badge variant="destructive">{product.discountPercentage}% OFF</Badge>
                    </div>
                    <div className="space-y-1">
                      <h4 className="text-sm font-medium text-muted-foreground">Sale Price</h4>
                      <p className="text-lg font-semibold text-green-600">{formatPrice(discountedPrice!)}</p>
                    </div>
                  </>
                )}
                
                <div className="space-y-1">
                  <h4 className="text-sm font-medium text-muted-foreground flex items-center gap-1">
                    <Archive className="h-3 w-3" />
                    Stock
                  </h4>
                  <p className="text-lg font-semibold">{product.stock}</p>
                </div>
                
                <div className="space-y-1">
                  <h4 className="text-sm font-medium text-muted-foreground flex items-center gap-1">
                    <Weight className="h-3 w-3" />
                    Weight
                  </h4>
                  <p className="text-sm">{product.weight}g</p>
                </div>
                
                <div className="space-y-1">
                  <h4 className="text-sm font-medium text-muted-foreground flex items-center gap-1">
                    <Palette className="h-3 w-3" />
                    Color
                  </h4>
                  <Badge variant="outline">{product.color}</Badge>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Categories and Metadata - Reorganized */}
          <div className="grid grid-cols-1 gap-6">
            {/* Categories - Compact */}
            <Card>
              <CardHeader className="pb-3">
                <CardTitle className="flex items-center gap-2 text-base">
                  <Tag className="h-4 w-4" />
                  Categories
                </CardTitle>
              </CardHeader>
              <CardContent className="pt-0">
                <div className="flex flex-wrap gap-2">
                  {product.categories && product.categories.length > 0 ? (
                    product.categories.map((category, index) => (
                      <Badge key={index} variant="secondary" className="text-sm">
                        {category}
                      </Badge>
                    ))
                  ) : (
                    <span className="text-sm text-muted-foreground">No categories assigned</span>
                  )}
                </div>
              </CardContent>
            </Card>

            {/* Metadata & IDs - Compact Grid */}
            <Card>
              <CardHeader className="pb-3">
                <CardTitle className="flex items-center gap-2 text-base">
                  <Calendar className="h-4 w-4" />
                  Metadata
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 text-sm">
                  <div className="space-y-1">
                    <h4 className="text-xs font-medium text-muted-foreground">Product ID</h4>
                    <code className="text-[10px] bg-muted px-1.5 py-0.5 rounded block truncate">{product.id}</code>
                  </div>
                  <div className="space-y-1">
                    <h4 className="text-xs font-medium text-muted-foreground">Product Code</h4>
                    <code className="text-xs bg-primary/10 px-1.5 py-0.5 rounded block truncate">{product.productCode}</code>
                  </div>
                  <div className="space-y-1">
                    <h4 className="text-xs font-medium text-muted-foreground">Seller ID</h4>
                    <code className="text-[10px] bg-muted px-1.5 py-0.5 rounded block truncate">{product.sellerId}</code>
                  </div>
                  {product.rating !== undefined && (
                    <div className="space-y-1">
                      <h4 className="text-xs font-medium text-muted-foreground flex items-center gap-1">
                        <Star className="h-3 w-3" />
                        Rating
                      </h4>
                      <p className="text-xs">{product.rating}/5 ({product.reviewsCount || 0})</p>
                    </div>
                  )}
                  {product.createdAt && (
                    <div className="space-y-1">
                      <h4 className="text-xs font-medium text-muted-foreground">Created</h4>
                      <p className="text-xs">{formatDate(product.createdAt)}</p>
                    </div>
                  )}
                  {product.updatedAt && (
                    <div className="space-y-1">
                      <h4 className="text-xs font-medium text-muted-foreground">Updated</h4>
                      <p className="text-xs">{formatDate(product.updatedAt)}</p>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Close
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}