'use client'

import { ProductsDataTable } from '@/components/dashboard/products-data-table'
import { AddProductDialog } from '@/components/dashboard/add-product-dialog'
import { AppSidebar } from '@/components/app-sidebar'
import { SidebarInset, SidebarTrigger } from '@/components/ui/sidebar'
import { Separator } from '@/components/ui/separator'
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '@/components/ui/breadcrumb'
import { ThemeToggle } from '@/components/theme-toggle'
import { useAuth } from '@/providers/auth-provider'
import { CategoriesDataTable } from '@/components/dashboard/categories-data-table'
import { AddCategoryDialog } from '@/components/dashboard/add-category-dialog'

export default function CateoriesPage() {
  const { user } = useAuth()

  return (
    <>
      <AppSidebar />
      <SidebarInset>
        <header className="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-[[data-collapsible=icon]]/sidebar-wrapper:h-12">
          <div className="flex items-center gap-2 px-4">
            <SidebarTrigger className="-ml-1" />
            <Separator orientation="vertical" className="mr-2 h-4" />
            <Breadcrumb>
              <BreadcrumbList>
                <BreadcrumbItem className="hidden md:block">
                  <BreadcrumbLink href="/dashboard">
                    eShop Admin
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator className="hidden md:block" />
                <BreadcrumbItem>
                  <BreadcrumbPage>Categories</BreadcrumbPage>
                </BreadcrumbItem>
              </BreadcrumbList>
            </Breadcrumb>
          </div>
          <div className="ml-auto px-4 flex items-center gap-4">
            <p className="text-sm text-muted-foreground">
              Welcome back, {user?.fullName || user?.email}
            </p>
            <ThemeToggle />
          </div>
        </header>
        <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
          <div className="min-h-[100vh] flex-1 rounded-xl bg-muted/50 md:min-h-min">
            <div className="p-6">
              <div className="flex justify-between items-center mb-8">
                <div>
                  <h1 className="text-3xl font-bold tracking-tight">Categories</h1>
                  <p className="text-muted-foreground">
                    Manage your categories
                  </p>
                </div>
                <AddCategoryDialog />
              </div>

              <div>
                <CategoriesDataTable />
              </div>
            </div>
          </div>
        </div>
      </SidebarInset>
    </>
  )
}