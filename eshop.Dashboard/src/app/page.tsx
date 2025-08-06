'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { useAuth } from '@/providers/auth-provider'
import { Loader2 } from 'lucide-react'

export default function Home() {
  const { isAuthenticated, isLoading, isFetched } = useAuth()
  const router = useRouter()

  console.log('🏠 Home Page State:', { isAuthenticated, isLoading, isFetched })

  useEffect(() => {
    console.log('🏠 Home useEffect triggered:', { isAuthenticated, isLoading, isFetched })
    // Only redirect after auth has been fetched and loading is complete
    if (!isLoading && isFetched) {
      console.log('🏠 Conditions met, redirecting...', { isAuthenticated })
      if (isAuthenticated) {
        console.log('🏠 Redirecting to dashboard')
        router.push('/dashboard')
      } else {
        console.log('🏠 Redirecting to login')
        router.push('/login')
      }
    }
  }, [isAuthenticated, isLoading, isFetched, router])

  if (isLoading || !isFetched) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background">
        <div className="text-center">
          <Loader2 className="h-8 w-8 animate-spin mx-auto mb-4 text-primary" />
          <p className="text-muted-foreground">
            {isLoading ? 'Checking authentication...' : 'Loading...'}
          </p>
        </div>
      </div>
    )
  }

  return null
}
