'use client'

import { SidebarProvider } from '@/components/ui/sidebar'

interface AppSidebarProviderProps {
  children: React.ReactNode
}

export function AppSidebarProvider({ children }: AppSidebarProviderProps) {
  return (
    <SidebarProvider>
      {children}
    </SidebarProvider>
  )
}