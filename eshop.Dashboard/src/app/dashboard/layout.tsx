'use client'

import { AppSidebarProvider } from '@/providers/sidebar-provider'

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <AppSidebarProvider>
      {children}
    </AppSidebarProvider>
  )
}