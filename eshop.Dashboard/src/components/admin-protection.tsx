'use client'

import { useAuth } from '@/providers/auth-provider'
import { useRouter, usePathname } from 'next/navigation'
import { useEffect, useState } from 'react'
import { LoadingSpinner } from '@/components/ui/loading-spinner'

interface AdminProtectionProps {
  children: React.ReactNode
  fallback?: React.ReactNode
}

export function AdminProtection({ children, fallback }: AdminProtectionProps) {
  const { isAuthenticated, isAdmin, isLoading, isAdminLoading, isFetched } = useAuth()
  const router = useRouter()
  const pathname = usePathname()
  const [hasRedirected, setHasRedirected] = useState(false)

  useEffect(() => {
    // Only process redirects once to avoid loops
    if (hasRedirected) return

    // Wait for auth to be loaded
    if (!isLoading && isFetched) {
      if (!isAuthenticated) {
        // User is not authenticated, redirect to login
        setHasRedirected(true)
        router.replace('/login')
        return
      }

      // Wait for admin check to complete
      if (!isAdminLoading) {
        if (!isAdmin) {
          // User is authenticated but not admin - show unauthorized page
          if (!fallback) {
            setHasRedirected(true)
            router.replace('/unauthorized')
            return
          }
        }
      }
    }
  }, [isAuthenticated, isAdmin, isLoading, isAdminLoading, isFetched, router, fallback, hasRedirected])

  // Reset redirect flag when pathname changes (user navigated manually)
  useEffect(() => {
    setHasRedirected(false)
  }, [pathname])

  // Show loading while checking authentication and admin status
  if (isLoading || !isFetched || (isAuthenticated && isAdminLoading)) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <LoadingSpinner className="w-8 h-8" />
        <span className="ml-2 text-muted-foreground">Checking permissions...</span>
      </div>
    )
  }

  // User is not authenticated - show loading while redirect happens
  if (!isAuthenticated) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <LoadingSpinner className="w-8 h-8" />
        <span className="ml-2 text-muted-foreground">Redirecting to login...</span>
      </div>
    )
  }

  // User is authenticated but not admin
  if (!isAdmin) {
    if (fallback) {
      return <>{fallback}</>
    }
    // Show loading while redirect happens
    return (
      <div className="flex items-center justify-center min-h-screen">
        <LoadingSpinner className="w-8 h-8" />
        <span className="ml-2 text-muted-foreground">Access denied. Redirecting...</span>
      </div>
    )
  }

  // User is admin, show protected content
  return <>{children}</>
}

// This fallback component is no longer used as we redirect to /unauthorized page
// Keeping for potential future use
export function UnauthorizedFallback() {
  const router = useRouter()

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-6">
      <div className="text-center space-y-4">
        <h1 className="text-4xl font-bold text-destructive">Access Denied</h1>
        <p className="text-lg text-muted-foreground max-w-md">
          You don't have permission to access this area. Admin privileges are required.
        </p>
        <div className="flex gap-2 justify-center">
          <button
            onClick={() => router.replace('/login')}
            className="inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 bg-primary text-primary-foreground hover:bg-primary/90 h-10 px-4 py-2"
          >
            Login with Admin Account
          </button>
          <button
            onClick={() => router.replace('/')}
            className="inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 border border-input bg-background hover:bg-accent hover:text-accent-foreground h-10 px-4 py-2"
          >
            Go Home
          </button>
        </div>
      </div>
    </div>
  )
}
