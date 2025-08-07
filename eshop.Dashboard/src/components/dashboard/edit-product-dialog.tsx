'use client'

import { useState, useEffect } from 'react'
import { useAuth } from '@/providers/auth-provider'
import { useCategories, useUpdateProduct, useImageUpload, useProduct } from '@/hooks/use-api'
import { type Product, type ProductUpdateData } from '@/lib/api'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Card, CardContent } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Loader2, Upload, X, Package, Edit } from 'lucide-react'
import { toast } from 'sonner'

interface ProductFormData {
  name: string
  description: string
  nameArabic: string
  descriptionArabic: string
  price: number | ''
  stock: number | ''
  weight: number | ''
  color: string
  discountPercentage: number | ''
  categoryIds: string[]
  coverPictureUrl: string
  productPictureUrls: string[]
}

interface EditProductDialogProps {
  productId: string | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function EditProductDialog({ productId, open, onOpenChange }: EditProductDialogProps) {
  const { user } = useAuth()
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [isUploading, setIsUploading] = useState(false)
  
  const [formData, setFormData] = useState<ProductFormData>({
    name: '',
    description: '',
    nameArabic: '',
    descriptionArabic: '',
    price: '',
    stock: '',
    weight: '',
    color: '',
    discountPercentage: 0,
    categoryIds: [],
    coverPictureUrl: '',
    productPictureUrls: [],
  })

  const { data: categoriesData } = useCategories()
  const { data: productData, isLoading: isLoadingProduct } = useProduct(productId)
  const updateProductMutation = useUpdateProduct()
  const imageUploadMutation = useImageUpload()

  // Load product data into form when dialog opens
  useEffect(() => {
    if (productData && open && categoriesData?.categories) {
      // Match product categories by name to get the correct IDs
      const selectedCategoryIds: string[] = []
      if (productData.categories) {
        productData.categories.forEach(categoryName => {
          const matchedCategory = categoriesData.categories.find(cat => cat.name === categoryName)
          if (matchedCategory) {
            selectedCategoryIds.push(matchedCategory.id)
          }
        })
      }
      
      setFormData({
        name: productData.name || '',
        description: productData.description || '',
        nameArabic: productData.nameArabic || '',
        descriptionArabic: productData.descriptionArabic || '',
        price: productData.price || '',
        stock: productData.stock || '',
        weight: productData.weight || '',
        color: productData.color || '',
        discountPercentage: productData.discountPercentage || 0,
        categoryIds: selectedCategoryIds,
        coverPictureUrl: productData.coverPictureUrl || '',
        productPictureUrls: productData.productPictures || productData.productPictureUrls || [],
      })
      setErrors({})
    }
  }, [productData, open, categoriesData])

  const handleInputChange = (field: keyof ProductFormData) => (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const value = e.target.value
    setFormData(prev => ({ 
      ...prev, 
      [field]: field === 'price' || field === 'stock' || field === 'weight' || field === 'discountPercentage'
        ? value === '' ? '' : Number(value)
        : value
    }))
    
    // Clear error for this field when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }))
    }
  }

  const handleCategoryToggle = (categoryId: string) => {
    setFormData(prev => ({
      ...prev,
      categoryIds: prev.categoryIds.includes(categoryId)
        ? prev.categoryIds.filter(id => id !== categoryId)
        : [...prev.categoryIds, categoryId]
    }))
  }

  const handleImageUpload = async (file: File, type: 'cover' | 'product') => {
    if (!file) return

    try {
      setIsUploading(true)
      const publicUrl = await imageUploadMutation.mutateAsync(file)
      
      if (type === 'cover') {
        setFormData(prev => ({ ...prev, coverPictureUrl: publicUrl }))
      } else {
        setFormData(prev => ({ 
          ...prev, 
          productPictureUrls: [...prev.productPictureUrls, publicUrl]
        }))
      }
      toast.success('Image uploaded successfully')
    } catch (error) {
      console.error('Image upload failed:', error)
      toast.error('Image upload failed. Please try again.')
      setErrors(prev => ({ 
        ...prev, 
        [`${type}Image`]: 'Image upload failed. Please try again.'
      }))
    } finally {
      setIsUploading(false)
    }
  }

  const removeProductImage = (index: number) => {
    setFormData(prev => ({
      ...prev,
      productPictureUrls: prev.productPictureUrls.filter((_, i) => i !== index)
    }))
  }

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {}

    if (!formData.name.trim()) {
      newErrors.name = 'Product name is required'
    }

    if (!formData.description.trim()) {
      newErrors.description = 'Product description is required'
    }

    if (formData.price === '' || formData.price <= 0) {
      newErrors.price = 'Valid price is required'
    }

    if (formData.stock === '' || formData.stock < 0) {
      newErrors.stock = 'Valid stock quantity is required'
    }

    if (formData.weight === '' || formData.weight <= 0) {
      newErrors.weight = 'Valid weight is required'
    }

    if (!formData.color.trim()) {
      newErrors.color = 'Product color is required'
    }

    if (!formData.coverPictureUrl) {
      newErrors.coverImage = 'Cover image is required'
    }

    if (formData.categoryIds.length === 0) {
      newErrors.categories = 'At least one category must be selected'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async () => {
    if (!validateForm() || !productId) {
      return
    }

    try {
      // Create update payload with only changed fields
      const updateData: ProductUpdateData = {}
      
      // Only include fields that have actual values
      if (formData.name.trim()) updateData.name = formData.name.trim()
      if (formData.nameArabic.trim()) updateData.nameArabic = formData.nameArabic.trim()
      if (formData.description.trim()) updateData.description = formData.description.trim()
      if (formData.descriptionArabic.trim()) updateData.descriptionArabic = formData.descriptionArabic.trim()
      if (formData.coverPictureUrl.trim()) updateData.coverPictureUrl = formData.coverPictureUrl.trim()
      if (typeof formData.price === 'number') updateData.price = formData.price
      if (typeof formData.stock === 'number') updateData.stock = formData.stock
      if (typeof formData.weight === 'number') updateData.weight = formData.weight
      if (formData.color.trim()) updateData.color = formData.color.trim()
      if (typeof formData.discountPercentage === 'number') updateData.discountPercentage = formData.discountPercentage
      if (formData.categoryIds.length > 0) updateData.categoryIds = formData.categoryIds
      if (formData.productPictureUrls.length > 0) updateData.productPictures = formData.productPictureUrls

      await updateProductMutation.mutateAsync({
        id: productId,
        data: updateData
      })

      toast.success('Product updated successfully!')
      onOpenChange(false)
      
    } catch (error: any) {
      console.error('Product update failed:', error)
      
      if (error?.errors) {
        setErrors(error.errors)
      } else {
        toast.error(error?.message || 'Failed to update product. Please try again.')
      }
    }
  }

  const resetForm = () => {
    if (productData && categoriesData?.categories) {
      // Match product categories by name to get the correct IDs
      const selectedCategoryIds: string[] = []
      if (productData.categories) {
        productData.categories.forEach(categoryName => {
          const matchedCategory = categoriesData.categories.find(cat => cat.name === categoryName)
          if (matchedCategory) {
            selectedCategoryIds.push(matchedCategory.id)
          }
        })
      }
      
      setFormData({
        name: productData.name || '',
        description: productData.description || '',
        nameArabic: productData.nameArabic || '',
        descriptionArabic: productData.descriptionArabic || '',
        price: productData.price || '',
        stock: productData.stock || '',
        weight: productData.weight || '',
        color: productData.color || '',
        discountPercentage: productData.discountPercentage || 0,
        categoryIds: selectedCategoryIds,
        coverPictureUrl: productData.coverPictureUrl || '',
        productPictureUrls: productData.productPictures || productData.productPictureUrls || [],
      })
    }
    setErrors({})
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="!max-w-6xl max-h-[90vh] overflow-y-auto sm:!max-w-6xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Edit className="h-5 w-5" />
            Edit Product
          </DialogTitle>
          <DialogDescription>
            Update product information. Only modified fields will be updated.
          </DialogDescription>
        </DialogHeader>

        {isLoadingProduct ? (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-6 w-6 animate-spin" />
            <span className="ml-2">Loading product data...</span>
          </div>
        ) : (
          <div className="space-y-6">
            {/* Images */}
            <Card>
              <CardContent className="pt-6 space-y-4">
                <h3 className="text-lg font-medium">Images</h3>
                
                <div className="grid grid-cols-2 gap-6">
                  {/* Cover Image - Left Half */}
                  <div>
                    <h4 className="text-sm font-medium mb-3">Cover Image *</h4>
                    {formData.coverPictureUrl ? (
                      <div className="relative">
                        <img 
                          src={formData.coverPictureUrl} 
                          alt="Cover"
                          className="w-full h-auto max-h-80 object-contain rounded-lg border bg-muted/20"
                        />
                        <Button
                          type="button"
                          variant="destructive"
                          size="icon"
                          className="absolute top-2 right-2 h-6 w-6"
                          onClick={() => setFormData(prev => ({ ...prev, coverPictureUrl: '' }))}
                        >
                          <X className="h-3 w-3" />
                        </Button>
                      </div>
                    ) : (
                      <div className="w-full h-64 bg-muted rounded-lg flex items-center justify-center border-2 border-dashed">
                        <input
                          type="file"
                          accept="image/*"
                          onChange={(e) => {
                            const file = e.target.files?.[0]
                            if (file) handleImageUpload(file, 'cover')
                          }}
                          className="hidden"
                          id="cover-upload"
                          disabled={isUploading}
                        />
                        <label htmlFor="cover-upload" className="cursor-pointer text-center">
                          <Upload className="h-8 w-8 mx-auto mb-2 text-muted-foreground" />
                          <p className="text-sm text-muted-foreground">Click to upload cover image</p>
                        </label>
                      </div>
                    )}
                    {errors.coverImage && <p className="text-sm text-red-500 mt-2">{errors.coverImage}</p>}
                  </div>

                  {/* Product Images - Right Half */}
                  <div>
                    <h4 className="text-sm font-medium mb-3">Product Images</h4>
                    <div className="grid grid-cols-2 gap-2 max-h-80 overflow-y-auto">
                      {/* Add new image slot - FIRST */}
                      <div className="aspect-square border-2 border-dashed border-muted-foreground/25 rounded-lg flex items-center justify-center bg-muted/10 hover:bg-muted/20 transition-colors">
                        <input
                          type="file"
                          accept="image/*"
                          onChange={(e) => {
                            const file = e.target.files?.[0]
                            if (file) handleImageUpload(file, 'product')
                          }}
                          className="hidden"
                          id="product-upload"
                          disabled={isUploading}
                        />
                        <label htmlFor="product-upload" className="cursor-pointer text-center p-4">
                          <Upload className="h-4 w-4 mx-auto mb-1 text-muted-foreground" />
                          <p className="text-xs text-muted-foreground">Add Image</p>
                        </label>
                      </div>

                      {/* Existing product images */}
                      {formData.productPictureUrls.map((url, index) => (
                        <div key={index} className="relative group">
                          <div className="aspect-square bg-muted/20 rounded-lg border overflow-hidden">
                            <img 
                              src={url} 
                              alt={`Product ${index + 1}`}
                              className="w-full h-full object-contain"
                            />
                            <Button
                              type="button"
                              variant="destructive"
                              size="icon"
                              className="absolute top-1 right-1 h-5 w-5 opacity-0 group-hover:opacity-100 transition-opacity"
                              onClick={() => removeProductImage(index)}
                            >
                              <X className="h-3 w-3" />
                            </Button>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Basic Information */}
            <Card>
              <CardContent className="pt-6 space-y-4">
                <h3 className="text-lg font-medium">Basic Information</h3>
                
                <div className="space-y-6">
                  {/* English Fields */}
                  <div className="space-y-4">
                    <h4 className="text-md font-medium text-muted-foreground">English</h4>
                    <div className="space-y-2">
                      <Label htmlFor="name">Product Name *</Label>
                      <Input
                        id="name"
                        value={formData.name}
                        onChange={handleInputChange('name')}
                        className={errors.name ? 'border-red-500' : ''}
                        placeholder="Enter product name"
                      />
                      {errors.name && <p className="text-sm text-red-500">{errors.name}</p>}
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="description">Description *</Label>
                      <Textarea
                        id="description"
                        value={formData.description}
                        onChange={handleInputChange('description')}
                        className={errors.description ? 'border-red-500' : ''}
                        placeholder="Enter product description"
                        rows={3}
                      />
                      {errors.description && <p className="text-sm text-red-500">{errors.description}</p>}
                    </div>
                  </div>

                  {/* Arabic Fields */}
                  <div className="space-y-4">
                    <h4 className="text-md font-medium text-muted-foreground">Arabic</h4>
                    <div className="space-y-2">
                      <Label htmlFor="nameArabic">Arabic Name</Label>
                      <Input
                        id="nameArabic"
                        value={formData.nameArabic}
                        onChange={handleInputChange('nameArabic')}
                        placeholder="Enter Arabic name"
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="descriptionArabic">Arabic Description</Label>
                      <Textarea
                        id="descriptionArabic"
                        value={formData.descriptionArabic}
                        onChange={handleInputChange('descriptionArabic')}
                        placeholder="Enter Arabic description"
                        rows={3}
                      />
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Pricing & Stock */}
            <Card>
              <CardContent className="pt-6 space-y-4">
                <h3 className="text-lg font-medium">Pricing & Stock</h3>
                
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="price">Price * ($)</Label>
                    <Input
                      id="price"
                      type="number"
                      min="0"
                      step="0.01"
                      value={formData.price}
                      onChange={handleInputChange('price')}
                      className={errors.price ? 'border-red-500' : ''}
                      placeholder="0.00"
                    />
                    {errors.price && <p className="text-sm text-red-500">{errors.price}</p>}
                  </div>
                  
                  <div className="space-y-2">
                    <Label htmlFor="stock">Stock *</Label>
                    <Input
                      id="stock"
                      type="number"
                      min="0"
                      value={formData.stock}
                      onChange={handleInputChange('stock')}
                      className={errors.stock ? 'border-red-500' : ''}
                      placeholder="0"
                    />
                    {errors.stock && <p className="text-sm text-red-500">{errors.stock}</p>}
                  </div>
                  
                  <div className="space-y-2">
                    <Label htmlFor="weight">Weight * (kg)</Label>
                    <Input
                      id="weight"
                      type="number"
                      min="0"
                      step="0.01"
                      value={formData.weight}
                      onChange={handleInputChange('weight')}
                      className={errors.weight ? 'border-red-500' : ''}
                      placeholder="0.00"
                    />
                    {errors.weight && <p className="text-sm text-red-500">{errors.weight}</p>}
                  </div>
                  
                  <div className="space-y-2">
                    <Label htmlFor="discountPercentage">Discount (%)</Label>
                    <Input
                      id="discountPercentage"
                      type="number"
                      min="0"
                      max="100"
                      value={formData.discountPercentage}
                      onChange={handleInputChange('discountPercentage')}
                      placeholder="0"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="color">Color *</Label>
                  <Input
                    id="color"
                    value={formData.color}
                    onChange={handleInputChange('color')}
                    className={errors.color ? 'border-red-500' : ''}
                    placeholder="Enter color"
                  />
                  {errors.color && <p className="text-sm text-red-500">{errors.color}</p>}
                </div>
              </CardContent>
            </Card>

            {/* Categories */}
            <Card>
              <CardContent className="pt-6 space-y-4">
                <h3 className="text-lg font-medium">Categories *</h3>
                
                {categoriesData?.categories && categoriesData.categories.length > 0 ? (
                  <div className="flex flex-wrap gap-2">
                    {categoriesData.categories.map((category) => (
                      <Badge
                        key={category.id}
                        variant={formData.categoryIds.includes(category.id) ? "default" : "outline"}
                        className="cursor-pointer"
                        onClick={() => handleCategoryToggle(category.id)}
                      >
                        {category.name}
                      </Badge>
                    ))}
                  </div>
                ) : (
                  <p className="text-sm text-muted-foreground">Loading categories...</p>
                )}
                
                {errors.categories && <p className="text-sm text-red-500">{errors.categories}</p>}
              </CardContent>
            </Card>
          </div>
        )}

        <DialogFooter className="gap-2">
          <Button
            variant="outline"
            onClick={() => {
              resetForm()
              onOpenChange(false)
            }}
            disabled={updateProductMutation.isPending}
          >
            Cancel
          </Button>
          <Button
            variant="outline"
            onClick={resetForm}
            disabled={updateProductMutation.isPending}
          >
            Reset
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={updateProductMutation.isPending || isUploading || isLoadingProduct}
          >
            {updateProductMutation.isPending ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Updating...
              </>
            ) : (
              <>
                <Package className="mr-2 h-4 w-4" />
                Update Product
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
