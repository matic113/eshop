'use client'

import { useRouter } from 'next/navigation'
import { useAuth } from '@/providers/auth-provider'

export default function UnauthorizedPage() {
  const router = useRouter()
  const { user, logout } = useAuth()

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-6">
      <div className="text-center space-y-6 max-w-md">
        <div className="space-y-2">
          <h1 className="text-4xl font-bold text-destructive">Access Denied</h1>
          <p className="text-lg text-muted-foreground">
            You don't have permission to access this area. Admin privileges are required.
          </p>
        </div>
        
        {user && (
          <div className="bg-muted/50 rounded-lg p-4 space-y-2">
            <p className="text-sm text-muted-foreground">
              Currently logged in as:
            </p>
            <p className="font-medium">{user.fullName || user.email}</p>
          </div>
        )}

        <div className="space-y-3">
          <p className="text-sm text-muted-foreground">
            You need to log in with an admin account to access the dashboard.
          </p>
          
          <div className="flex flex-col gap-3">
            <button
              onClick={() => {
                logout()
                router.push('/login')
              }}
              className="inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 bg-primary text-primary-foreground hover:bg-primary/90 h-10 px-4 py-2"
            >
              Login with Admin Account
            </button>
            
            <button
              onClick={() => router.push('/')}
              className="inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 border border-input bg-background hover:bg-accent hover:text-accent-foreground h-10 px-4 py-2"
            >
              Go to Home
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
