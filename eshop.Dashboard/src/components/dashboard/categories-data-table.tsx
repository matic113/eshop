'use client'

import { useState } from 'react'
import { useProducts, useDeleteProduct, useCategories, useDeleteCategory } from '@/hooks/use-api'
import { type ProductSearchParams, type Product, Category } from '@/lib/api'
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
import { Loader2, Edit, Package, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import { EditCategoryDialog } from './edit-category-dialog'


export function CategoriesDataTable() {
  const { data: categories, isLoading, error, refetch } = useCategories()

  const [editDialogOpen, setEditDialogOpen] = useState(false)
  const [editCategoryId, setEditCategoryId] = useState<string | null>(null)

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [categoryToDelete, setCategoryToDelete] = useState<Category | null>(null)

  const deleteCategory = useDeleteCategory()

  const handleEditClick = (category: Category, event: React.MouseEvent) => {
    event.stopPropagation()
    setEditCategoryId(category.id)
    setEditDialogOpen(true)
  }

  const handleDeleteClick = (category: Category, event: React.MouseEvent) => {
    event.stopPropagation()
    setCategoryToDelete(category)
    setDeleteDialogOpen(true)
  }

  const handleDeleteConfirm = () => {
    if (!categoryToDelete || deleteCategory.isPending) {
      return; // توقف هنا
    }

    // إذا وصلنا إلى هنا، فهذا هو التشغيل الأول فقط
    deleteCategory.mutate(categoryToDelete.id, {
      onSuccess: () => {
        // هذه الدالة هي التي ستعمل الآن بشكل مضمون
        toast.success(`Category "${categoryToDelete.name}" has been deleted successfully`);
        setDeleteDialogOpen(false);
        setCategoryToDelete(null);
        refetch();
      },
      onError: (error) => {
        // لن نصل إلى هنا أبدًا بسبب خطأ 404 الناتج عن الاستدعاء المزدوج
        toast.error(`Failed to delete category: ${error.message}`);
      }
    });
  };




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
            Manage All Categories
          </CardTitle>
          <CardDescription>
            Browse, search, and manage your category catalog
          </CardDescription>

        </CardHeader>

        <CardContent className="space-y-6">
          {/* Search Filters */}
          <Label htmlFor="searchTerm">Search Category</Label>


          {/* Data Table */}
          {isLoading ? (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <span className="ml-2">Loading Categories...</span>
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
          ) : categories?.categories ? (
            <div className="space-y-4">
              {/* Pagination Info */}
              <div className="flex items-center justify-between">
                <p className="text-sm text-muted-foreground">
                  Showing {categories.categories.length} categories
                </p>

              </div>

              {/* Table */}
              <div className="rounded-md border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="w-[72px]">Image</TableHead>
                      <TableHead>Category Name</TableHead>
                      <TableHead className="text-center">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {categories.categories.map((category: Category) => {

                      return (
                        <TableRow
                          key={category.id}
                          className="hover:bg-muted/50 transition-colors"
                        >
                          <TableCell className="py-2 align-middle">
                            <HoverCard>
                              <HoverCardTrigger asChild>
                                <div className="cursor-pointer">
                                  <Avatar className="h-10 w-10 md:h-12 md:w-12">
                                    <AvatarImage
                                      src={category.coverPictureUrl}
                                      alt={category.name}
                                      className="object-cover"
                                    />
                                    <AvatarFallback>
                                      <Package className="h-5 w-5" />
                                    </AvatarFallback>
                                  </Avatar>
                                </div>
                              </HoverCardTrigger>
                              <HoverCardContent className="w-80 p-0">
                                <div className="relative">
                                  {category.coverPictureUrl ? (
                                    <img
                                      src={category.coverPictureUrl}
                                      alt={category.name}
                                      className="w-full h-64 object-cover rounded-md"
                                    />
                                  ) : (
                                    <div className="w-full h-64 bg-muted rounded-md flex items-center justify-center">
                                      <Package className="h-16 w-16 text-muted-foreground" />
                                    </div>
                                  )}
                                  <div className="p-3">
                                    <h4 className="text-sm font-semibold">{category.name}</h4>
                                  </div>
                                </div>
                              </HoverCardContent>
                            </HoverCard>
                          </TableCell>

                          <TableCell className="py-2 align-middle">
                            <div className="space-y-0.5">
                              <p className="font-medium leading-none">{category.name}</p>
                              <p className="text-sm text-muted-foreground truncate" title={category.description}>
                                {truncateText(category.description, 100)}
                              </p>
                            </div>
                          </TableCell>


                          <TableCell className="py-2 text-center align-middle">
                            <div className="flex items-center justify-center gap-2">
                              <Button
                                size="sm"
                                variant="outline"
                                className="h-8 px-3 text-xs hover:cursor-pointer"
                                onClick={(e) => handleEditClick(category, e)}
                              >
                                <Edit className="h-3 w-3 mr-1" />
                                Edit
                              </Button>
                              <Button
                                size="sm"
                                variant="outline"
                                className="h-8 px-3 text-xs text-red-600 hover:cursor-pointer hover:text-red-700 hover:bg-red-50"
                                onClick={(e) => handleDeleteClick(category, e)}
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
                No Categries found. Try adjusting your search criteria.
              </p>
            </div>
          )}
        </CardContent>
      </Card >

      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete the category
              <strong> "{categoryToDelete?.name}"</strong> from your store.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deleteCategory.isPending}>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={(e) => {
                e.preventDefault();
                handleDeleteConfirm();
              }}
              className="bg-red-600 hover:bg-red-700"
              disabled={deleteCategory.isPending}
            >
              {deleteCategory.isPending ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Deleting...
                </>
              ) : (
                'Delete Category'
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
      {/* Edit Product Dialog */}
      <EditCategoryDialog
        categoryId={editCategoryId}
        open={editDialogOpen}
        onOpenChange={(open) => {
          setEditDialogOpen(open)
          if (!open) {
            setEditCategoryId(null)
          }
        }}
      />

    </>
  )
}