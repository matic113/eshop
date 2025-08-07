'use client'

import { useState, useEffect } from 'react'
import { useCategories, useUpdateCategory, useImageUpload } from '@/hooks/use-api'
import { UpdateCategoryPayload } from '@/lib/api'
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
import { Loader2, Upload, X, Edit } from 'lucide-react'
import { toast } from 'sonner'

// واجهة الخصائص للمكون
interface EditCategoryDialogProps {
  categoryId: string | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

// الحالة الأولية الفارغة للنموذج
const initialState: UpdateCategoryPayload = {
  newName: '',
  newDescription: '',
  NewCoverPictureUrl: '',
}

export function EditCategoryDialog({ categoryId, open, onOpenChange }: EditCategoryDialogProps) {
  const [formData, setFormData] = useState<UpdateCategoryPayload>(initialState)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [isUploading, setIsUploading] = useState(false)

  const { data: categoriesData } = useCategories()
  const updateCategoryMutation = useUpdateCategory()
  const imageUploadMutation = useImageUpload()


  useEffect(() => {
    if (open && categoryId && categoriesData?.categories) {
      const currentCategory = categoriesData.categories.find(cat => cat.id === categoryId)
      if (currentCategory) {
        setFormData({
          newName: currentCategory.name || '',
          newDescription: currentCategory.description || '',
          NewCoverPictureUrl: currentCategory.coverPictureUrl || '',
        })
      }
    } else {
      setFormData(initialState)
      setErrors({})
    }
  }, [open, categoryId, categoriesData])

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { id, value } = e.target
    setFormData(prev => ({
      ...prev,
      [id]: value,
    }))

    if (errors[id]) {
      setErrors(prev => ({ ...prev, [id]: '' }))
    }
  }

  // دالة معالجة رفع صورة الغلاف
  const handleImageUpload = async (file: File) => {
    if (!file) return

    try {
      setIsUploading(true)
      const publicUrl = await imageUploadMutation.mutateAsync(file)
      setFormData(prev => ({ ...prev, NewCoverPictureUrl: publicUrl }))
      toast.success('Image uploaded successfully')
    } catch (error) {
      console.error('Image upload failed:', error)
      toast.error('Image upload failed. Please try again.')
    } finally {
      setIsUploading(false)
    }
  }

  // التحقق من صحة النموذج
  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!formData.newName.trim()) newErrors.newName = 'Category name is required'
    if (!formData.newDescription.trim()) newErrors.newDescription = 'Category description is required'
    if (!formData.NewCoverPictureUrl) newErrors.NewCoverPictureUrl = 'Cover image is required'

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  // دالة إرسال النموذج للتحديث
  const handleSubmit = async () => {
    if (!validateForm() || !categoryId) return

    try {
      // إنشاء حمولة التحديث بالبيانات الجديدة فقط
      const updateData: UpdateCategoryPayload = {
        newName: formData.newName,
        newDescription: formData.newDescription,
        NewCoverPictureUrl: formData.NewCoverPictureUrl,
      }

      await updateCategoryMutation.mutateAsync({
        id: categoryId,
        data: updateData,
      })

      toast.success('Category updated successfully!')
      onOpenChange(false)

    } catch (error: any) {
      console.error('Category update failed:', error)
      toast.error(error?.message || 'Failed to update category. Please try again.')
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="!max-w-3xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Edit className="h-5 w-5" />
            Edit Category
          </DialogTitle>
          <DialogDescription>
            Update category information. Click save when you're done.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6 py-2">
          <Card>
            <CardContent className="pt-6 space-y-4">
              <div className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="newName">Category Name *</Label>
                  <Input
                    id="newName"
                    value={formData.newName}
                    onChange={handleInputChange}
                    className={errors.newName ? 'border-red-500' : ''}
                    placeholder="Enter category name"
                  />
                  {errors.newName && <p className="text-sm text-red-500">{errors.newName}</p>}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="newDescription">Description *</Label>
                  <Textarea
                    id="newDescription"
                    value={formData.newDescription}
                    onChange={handleInputChange}
                    className={errors.newDescription ? 'border-red-500' : ''}
                    placeholder="Enter category description"
                    rows={3}
                  />
                  {errors.newDescription && <p className="text-sm text-red-500">{errors.newDescription}</p>}
                </div>
              </div>

              <div>
                <Label htmlFor="cover-upload">Cover Image *</Label>
                {formData.NewCoverPictureUrl ? (
                  <div className="relative mt-2">
                    <img
                      src={formData.NewCoverPictureUrl}
                      alt="Cover"
                      className="w-full h-auto max-h-64 object-contain rounded-lg border bg-muted/20"
                    />
                    <Button
                      type="button"
                      variant="destructive"
                      size="icon"
                      className="absolute top-2 right-2 h-6 w-6"
                      onClick={() => setFormData(prev => ({ ...prev, NewCoverPictureUrl: '' }))}
                    >
                      <X className="h-3 w-3" />
                    </Button>
                  </div>
                ) : (
                  <div className="mt-2 w-full h-48 bg-muted rounded-lg flex items-center justify-center border-2 border-dashed">
                    <input
                      type="file"
                      accept="image/*"
                      onChange={(e) => {
                        const file = e.target.files?.[0]
                        if (file) handleImageUpload(file)
                      }}
                      className="hidden"
                      id="cover-upload"
                      disabled={isUploading}
                    />
                    <label htmlFor="cover-upload" className="cursor-pointer text-center p-4">
                      {isUploading ? (
                        <Loader2 className="h-8 w-8 mx-auto animate-spin text-muted-foreground" />
                      ) : (
                        <Upload className="h-8 w-8 mx-auto mb-2 text-muted-foreground" />
                      )}
                      <p className="text-sm text-muted-foreground">
                        {isUploading ? 'Uploading...' : 'Click to upload cover image'}
                      </p>
                    </label>
                  </div>
                )}
                {errors.NewCoverPictureUrl && <p className="text-sm text-red-500 mt-2">{errors.NewCoverPictureUrl}</p>}
              </div>
            </CardContent>
          </Card>
        </div>

        <DialogFooter className="gap-2 sm:justify-end">
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={updateCategoryMutation.isPending || isUploading}
          >
            Cancel
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={updateCategoryMutation.isPending || isUploading}
          >
            {updateCategoryMutation.isPending || isUploading ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Updating...
              </>
            ) : (
              'Save Changes'
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}