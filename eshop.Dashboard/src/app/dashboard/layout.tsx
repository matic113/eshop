'use client'

import { AppSidebarProvider } from '@/providers/sidebar-provider'
import { AdminProtection } from '@/components/admin-protection'

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <AdminProtection>
      <AppSidebarProvider>
        {children}
      </AppSidebarProvider>
    </AdminProtection>
  )
}