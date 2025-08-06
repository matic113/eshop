'use client'

import { useState } from 'react'
import { useAuth } from '@/providers/auth-provider'
import { useCategories, useCreateProduct, useImageUpload } from '@/hooks/use-api'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Loader2, Plus, Upload, X, Package } from 'lucide-react'

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

interface AddProductDialogProps {
  children?: React.ReactNode
}

export function AddProductDialog({ children }: AddProductDialogProps) {
  const { user } = useAuth()
  const [open, setOpen] = useState(false)
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
  const createProductMutation = useCreateProduct()
  const imageUploadMutation = useImageUpload()

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
    } catch (error) {
      console.error('Image upload failed:', error)
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

    if (!formData.name.trim()) newErrors.name = 'Product name is required'
    if (!formData.description.trim()) newErrors.description = 'Description is required'
    if (!formData.nameArabic.trim()) newErrors.nameArabic = 'Arabic name is required'
    if (!formData.descriptionArabic.trim()) newErrors.descriptionArabic = 'Arabic description is required'
    if (formData.price === '' || formData.price <= 0) newErrors.price = 'Valid price is required'
    if (formData.stock === '' || formData.stock < 0) newErrors.stock = 'Valid stock quantity is required'
    if (formData.weight === '' || formData.weight <= 0) newErrors.weight = 'Valid weight is required'
    if (!formData.color.trim()) newErrors.color = 'Color is required'
    if (!formData.coverPictureUrl) newErrors.coverImage = 'Cover image is required'
    if (formData.categoryIds.length === 0) newErrors.categories = 'At least one category is required'

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    
    if (!validateForm()) return
    if (!user?.userId) {
      setErrors({ general: 'User not authenticated' })
      return
    }

    try {
      const productData = {
        sellerId: user.userId,
        name: formData.name,
        description: formData.description,
        nameArabic: formData.nameArabic,
        descriptionArabic: formData.descriptionArabic,
        coverPictureUrl: formData.coverPictureUrl,
        price: Number(formData.price),
        stock: Number(formData.stock),
        weight: Number(formData.weight),
        color: formData.color,
        discountPercentage: Number(formData.discountPercentage) || 0,
        categoryIds: formData.categoryIds,
        productPictureUrls: formData.productPictureUrls,
      }

      await createProductMutation.mutateAsync(productData)
      
      // Reset form and close dialog
      setFormData({
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
      setErrors({})
      setOpen(false)
    } catch (error: any) {
      if (error.errors) {
        setErrors(error.errors)
      } else {
        setErrors({ general: error.message || 'Failed to create product' })
      }
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {children || (
          <Button>
            <Plus className="h-4 w-4 mr-2" />
            Add Product
          </Button>
        )}
      </DialogTrigger>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Package className="h-5 w-5" />
            Add New Product
          </DialogTitle>
          <DialogDescription>
            Create a new product for your store. Fill in all required information.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Basic Information */}
          <Card>
            <CardContent className="pt-6 space-y-4">
              <h3 className="text-lg font-medium">Basic Information</h3>
              
              <div className="grid grid-cols-2 gap-4">
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
                  <Label htmlFor="nameArabic">Arabic Name *</Label>
                  <Input
                    id="nameArabic"
                    value={formData.nameArabic}
                    onChange={handleInputChange('nameArabic')}
                    className={errors.nameArabic ? 'border-red-500' : ''}
                    placeholder="أدخل اسم المنتج بالعربية"
                    dir="rtl"
                  />
                  {errors.nameArabic && <p className="text-sm text-red-500">{errors.nameArabic}</p>}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="description">Description *</Label>
                  <textarea
                    id="description"
                    value={formData.description}
                    onChange={handleInputChange('description')}
                    className={`flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 ${errors.description ? 'border-red-500' : ''}`}
                    placeholder="Enter product description"
                    rows={3}
                  />
                  {errors.description && <p className="text-sm text-red-500">{errors.description}</p>}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="descriptionArabic">Arabic Description *</Label>
                  <textarea
                    id="descriptionArabic"
                    value={formData.descriptionArabic}
                    onChange={handleInputChange('descriptionArabic')}
                    className={`flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 ${errors.descriptionArabic ? 'border-red-500' : ''}`}
                    placeholder="أدخل وصف المنتج بالعربية"
                    dir="rtl"
                    rows={3}
                  />
                  {errors.descriptionArabic && <p className="text-sm text-red-500">{errors.descriptionArabic}</p>}
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Pricing and Inventory */}
          <Card>
            <CardContent className="pt-6 space-y-4">
              <h3 className="text-lg font-medium">Pricing & Inventory</h3>
              
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="price">Price (USD) *</Label>
                  <Input
                    id="price"
                    type="number"
                    step="0.01"
                    min="0"
                    value={formData.price}
                    onChange={handleInputChange('price')}
                    className={errors.price ? 'border-red-500' : ''}
                    placeholder="0.00"
                  />
                  {errors.price && <p className="text-sm text-red-500">{errors.price}</p>}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="discountPercentage">Discount %</Label>
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

              <div className="grid grid-cols-3 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="stock">Stock Quantity *</Label>
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
                  <Label htmlFor="weight">Weight (g) *</Label>
                  <Input
                    id="weight"
                    type="number"
                    min="0"
                    value={formData.weight}
                    onChange={handleInputChange('weight')}
                    className={errors.weight ? 'border-red-500' : ''}
                    placeholder="0"
                  />
                  {errors.weight && <p className="text-sm text-red-500">{errors.weight}</p>}
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

          {/* Images */}
          <Card>
            <CardContent className="pt-6 space-y-4">
              <h3 className="text-lg font-medium">Images</h3>
              
              {/* Cover Image */}
              <div className="space-y-2">
                <Label>Cover Image *</Label>
                {formData.coverPictureUrl ? (
                  <div className="relative inline-block">
                    <img 
                      src={formData.coverPictureUrl} 
                      alt="Cover"
                      className="w-32 h-32 object-cover rounded-lg border"
                    />
                    <Button
                      type="button"
                      variant="destructive"
                      size="icon"
                      className="absolute -top-2 -right-2 h-6 w-6"
                      onClick={() => setFormData(prev => ({ ...prev, coverPictureUrl: '' }))}
                    >
                      <X className="h-3 w-3" />
                    </Button>
                  </div>
                ) : (
                  <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center">
                    <input
                      type="file"
                      accept="image/*"
                      onChange={(e) => {
                        const file = e.target.files?.[0]
                        if (file) handleImageUpload(file, 'cover')
                      }}
                      className="hidden"
                      id="cover-upload"
                    />
                    <label htmlFor="cover-upload" className="cursor-pointer">
                      <Upload className="h-8 w-8 mx-auto mb-2 text-gray-400" />
                      <p className="text-sm text-gray-600">Click to upload cover image</p>
                    </label>
                  </div>
                )}
                {errors.coverImage && <p className="text-sm text-red-500">{errors.coverImage}</p>}
              </div>

              {/* Product Images */}
              <div className="space-y-2">
                <Label>Product Images</Label>
                <div className="flex flex-wrap gap-2">
                  {formData.productPictureUrls.map((url, index) => (
                    <div key={index} className="relative">
                      <img 
                        src={url} 
                        alt={`Product ${index + 1}`}
                        className="w-24 h-24 object-cover rounded-lg border"
                      />
                      <Button
                        type="button"
                        variant="destructive"
                        size="icon"
                        className="absolute -top-2 -right-2 h-5 w-5"
                        onClick={() => removeProductImage(index)}
                      >
                        <X className="h-3 w-3" />
                      </Button>
                    </div>
                  ))}
                  
                  <div className="w-24 h-24 border-2 border-dashed border-gray-300 rounded-lg flex items-center justify-center">
                    <input
                      type="file"
                      accept="image/*"
                      onChange={(e) => {
                        const file = e.target.files?.[0]
                        if (file) handleImageUpload(file, 'product')
                      }}
                      className="hidden"
                      id="product-upload"
                    />
                    <label htmlFor="product-upload" className="cursor-pointer">
                      <Plus className="h-4 w-4 text-gray-400" />
                    </label>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Error Messages */}
          {errors.general && (
            <div className="text-sm text-red-500 text-center bg-red-50 p-3 rounded-lg">
              {errors.general}
            </div>
          )}

          <DialogFooter className="gap-2">
            <Button type="button" variant="outline" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button 
              type="submit" 
              disabled={createProductMutation.isPending || isUploading}
            >
              {createProductMutation.isPending || isUploading ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  {isUploading ? 'Uploading...' : 'Creating...'}
                </>
              ) : (
                'Create Product'
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}