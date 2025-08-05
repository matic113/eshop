'use client'

import { useAuth } from '@/providers/auth-provider'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { ProductsOverview } from '@/components/dashboard/products-overview'
import { ProductsDataTable } from '@/components/dashboard/products-data-table'
import { AppSidebar } from '@/components/app-sidebar'
import { SidebarInset, SidebarTrigger } from '@/components/ui/sidebar'
import { Separator } from '@/components/ui/separator'
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '@/components/ui/breadcrumb'
import { ThemeToggle } from '@/components/theme-toggle'

export default function DashboardPage() {
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
                  <BreadcrumbPage>Dashboard</BreadcrumbPage>
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
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <ProductsOverview />

                      <Card>
                        <CardHeader>
                          <CardTitle>Orders</CardTitle>
                          <CardDescription>View and manage orders</CardDescription>
                        </CardHeader>
                        <CardContent>
                    <p className="text-sm text-muted-foreground">
                      Track orders, update statuses, and manage fulfillment.
                    </p>
                        </CardContent>
                      </Card>

                      <Card>
                  <CardHeader>
                    <CardTitle>Categories</CardTitle>
                    <CardDescription>Organize product categories</CardDescription>
                  </CardHeader>
                  <CardContent>
              <p className="text-sm text-muted-foreground">
                Create and manage product categories and hierarchies.
              </p>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle>Reviews</CardTitle>
                    <CardDescription>Monitor customer feedback</CardDescription>
                  </CardHeader>
                  <CardContent>
              <p className="text-sm text-muted-foreground">
                View and moderate product reviews and ratings.
              </p>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle>Offers</CardTitle>
                    <CardDescription>Manage promotional campaigns</CardDescription>
                  </CardHeader>
                  <CardContent>
              <p className="text-sm text-muted-foreground">
                Create and manage promotional offers and discounts.
              </p>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle>Notifications</CardTitle>
                    <CardDescription>Send user notifications</CardDescription>
                  </CardHeader>
                  <CardContent>
              <p className="text-sm text-muted-foreground">
                Send targeted and broadcast notifications to users.
              </p>
                  </CardContent>
                </Card>
              </div>

              {/* Products Data Table */}
              <div className="mt-8">
                <ProductsDataTable />
              </div>
            </div>
          </div>
        </div>
      </SidebarInset>
    </>
  )
}