'use client'

import { useState } from 'react'
import { useAuth } from '@/providers/auth-provider'
import { useCategories, useCreateCategory, useCreateProduct, useImageUpload } from '@/hooks/use-api'
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
import { Loader2, Plus, Upload, X, Package } from 'lucide-react'
import { CreateCategoryPayload } from '@/lib/api'

interface AddCategoryDialogProps {
  children?: React.ReactNode
}

export function AddCategoryDialog({ children }: AddCategoryDialogProps) {
  const [open, setOpen] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [isUploading, setIsUploading] = useState(false)

  const [formData, setFormData] = useState<CreateCategoryPayload>({
    name: '',
    Description: '',
    CoverPictureUrl: '',
  })

  const createCategoryMutation = useCreateCategory()
  const imageUploadMutation = useImageUpload()

  const handleInputChange = (field: keyof CreateCategoryPayload) => (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const value = e.target.value
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }))

    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }))
    }
  }

  const handleImageUpload = async (file: File) => {
    if (!file) return

    try {
      setIsUploading(true)
      const publicUrl = await imageUploadMutation.mutateAsync(file)
      setFormData(prev => ({ ...prev, CoverPictureUrl: publicUrl }))
      setErrors(prev => ({ ...prev, coverImage: '' }))
    } catch (error) {
      console.error('Image upload failed:', error)
      setErrors(prev => ({
        ...prev,
        coverImage: 'Image upload failed. Please try again.',
      }))
    } finally {
      setIsUploading(false)
    }
  }

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!formData.name.trim()) newErrors.name = 'Category name is required'
    if (!formData.Description.trim()) newErrors.description = 'Description is required'
    if (!formData.CoverPictureUrl) newErrors.coverImage = 'Cover image is required'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const resetForm = () => {
    setFormData({
      name: '',
      Description: '',
      CoverPictureUrl: '',
    })
    setErrors({})
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!validateForm()) return

    try {
      await createCategoryMutation.mutateAsync({
        name: formData.name,
        Description: formData.Description,
        CoverPictureUrl: formData.CoverPictureUrl,
      }
      )
      resetForm()
      setOpen(false)
    } catch (error: any) {
      if (error.errors) {
        setErrors(error.errors)
      } else {
        setErrors({ general: error.message || 'Failed to create category' })
      }
    }
  }

  const handleDialogChange = (isOpen: boolean) => {
    if (!isOpen) resetForm()
    setOpen(isOpen)
  }

  return (
    <Dialog open={open} onOpenChange={handleDialogChange}>
      <DialogTrigger asChild>
        {children || (
          <Button className="hover:cursor-pointer">
            <Plus className="h-4 w-4 mr-2" />
            Add Category
          </Button>
        )}
      </DialogTrigger>

      <DialogContent className="!max-w-6xl max-h-[90vh] overflow-y-auto sm:!max-w-6xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Package className="h-5 w-5" />
            Add New Category
          </DialogTitle>
          <DialogDescription>
            Create a new category for your store. Fill in all required information.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="pt-6 space-y-4">
              <div className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="name">Category Name *</Label>
                  <Input
                    id="name"
                    value={formData.name}
                    onChange={handleInputChange('name')}
                    className={errors.name ? 'border-red-500' : ''}
                    placeholder="Enter category name"
                  />
                  {errors.name && <p className="text-sm text-red-500">{errors.name}</p>}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="description">Description *</Label>
                  <textarea
                    id="description"
                    value={formData.Description}
                    onChange={handleInputChange('Description')}
                    className={`flex min-h-[80px] w-full rounded-md border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring ${errors.description ? 'border-red-500' : ''}`}
                    placeholder="Enter category description"
                    rows={3}
                  />
                  {errors.description && <p className="text-sm text-red-500">{errors.description}</p>}
                </div>

                {/* Image Upload */}
                <div className="space-y-2">
                  <Label>Cover Image *</Label>
                  <div>
                    {formData.CoverPictureUrl ? (
                      <div className="relative inline-block">
                        <img
                          src={formData.CoverPictureUrl}
                          alt="Cover"
                          className="w-32 h-32 object-cover rounded-lg border"
                        />
                        <Button
                          type="button"
                          variant="destructive"
                          size="icon"
                          className="absolute -top-2 -right-2 h-6 w-6"
                          onClick={() => setFormData(prev => ({ ...prev, CoverPictureUrl: '' }))}
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
                            if (file) handleImageUpload(file)
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
                  </div>
                  {errors.coverImage && <p className="text-sm text-red-500">{errors.coverImage}</p>}
                </div>
              </div>
            </CardContent>
          </Card>

          {/* General Error */}
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
              disabled={createCategoryMutation.isPending || isUploading}
            >
              {createCategoryMutation.isPending || isUploading ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  {isUploading ? 'Uploading...' : 'Creating...'}
                </>
              ) : (
                'Create Category'
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
