'use client'

import * as React from 'react'
import {
  BarChart3,
  Box,
  Database,
  FileText,
  FolderOpen,
  HelpCircle,
  LayoutDashboard,
  Package,
  Search,
  Settings,
  ShoppingBag,
  Star,
  Store,
  Tag,
  Users,
} from 'lucide-react'

import { NavDocuments } from '@/components/nav-documents'
import { NavMain } from '@/components/nav-main'
import { NavSecondary } from '@/components/nav-secondary'
import { NavUser } from '@/components/nav-user'
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from '@/components/ui/sidebar'
import { useAuth } from '@/providers/auth-provider'

const data = {
  navMain: [
    {
      title: 'Dashboard',
      url: '/dashboard',
      icon: LayoutDashboard,
    },
    {
      title: 'Products',
      url: '/dashboard/products',
      icon: Package,
      // items: [
      //   {
      //     title: 'All Products',
      //     url: '/dashboard/products',
      //   },
      //   {
      //     title: 'Add Product',
      //     url: '/dashboard/products/add',
      //   },
      //   {
      //     title: 'Categories',
      //     url: '/dashboard/categories',
      //   },
      // ],
    },
    {
      title: 'Categories',
      url: '/dashboard/categories',
      icon: FolderOpen,
    },
    {
      title: 'Orders',
      url: '/dashboard/orders',
      icon: ShoppingBag,
      items: [
        // {
        //   title: 'All Orders',
        //   url: '/dashboard/orders',
        // },
        // {
        //   title: 'Pending',
        //   url: '/dashboard/orders/pending',
        // },
        // {
        //   title: 'Completed',
        //   url: '/dashboard/orders/completed',
        // },
      ],
    },
    {
      title: 'Analytics',
      url: '/dashboard/analytics',
      icon: BarChart3,
    },
    {
      title: 'Customers',
      url: '/dashboard/customers',
      icon: Users,
    },
  ],
  navManagement: [
    {
      name: 'Inventory',
      url: '/dashboard/inventory',
      icon: Box,
    },
    {
      name: 'Reviews',
      url: '/dashboard/reviews',
      icon: Star,
    },
    {
      name: 'Offers',
      url: '/dashboard/offers',
      icon: Tag,
    },
    {
      name: 'Reports',
      url: '/dashboard/reports',
      icon: FileText,
    },
  ],
  navSecondary: [
    {
      title: 'Settings',
      url: '/dashboard/settings',
      icon: Settings,
    },
    {
      title: 'Search',
      url: '/dashboard/search',
      icon: Search,
    },
    {
      title: 'Help & Support',
      url: '/dashboard/help',
      icon: HelpCircle,
    },
  ],
}

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const { user, logout } = useAuth()

  const userData = user ? {
    name: user.fullName,
    email: user.email,
    avatar: user.profilePicture || '',
  } : {
    name: 'Admin User',
    email: 'admin@eshop.com',
    avatar: '',
  }

  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton
              asChild
              className="data-[slot=sidebar-menu-button]:!p-1.5"
            >
              <a href="/dashboard">
                <Store className="!size-5" />
                <span className="text-base font-semibold">eShop Admin</span>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={data.navMain} />
        <NavDocuments items={data.navManagement} />
        <NavSecondary items={data.navSecondary} className="mt-auto" />
      </SidebarContent>
      <SidebarFooter>
        <NavUser user={userData} onLogout={logout} />
      </SidebarFooter>
    </Sidebar>
  )
}