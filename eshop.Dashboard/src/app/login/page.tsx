import { LoginForm } from '@/components/auth/login-form'
import { GoogleLoginButton } from '@/components/auth/google-login-button'
import { ThemeToggle } from '@/components/theme-toggle'

export default function LoginPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-background py-12 px-4 sm:px-6 lg:px-8">
      {/* Theme toggle in top-right corner */}
      <div className="absolute top-4 right-4">
        <ThemeToggle />
      </div>
      <div className="max-w-md w-full space-y-6">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-foreground">
            {process.env.NEXT_PUBLIC_DASHBOARD_NAME || 'Admin Dashboard'}
          </h1>
          <p className="mt-2 text-sm text-muted-foreground">
            Welcome to {process.env.NEXT_PUBLIC_COMPANY_NAME || 'eShop'} administration
          </p>
        </div>
        
        <LoginForm />
        
        <div className="relative">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-gray-200 dark:border-gray-700" />
          </div>
          <div className="relative flex justify-center text-sm">
            <span className="px-2 bg-background text-muted-foreground">Or continue with</span>
          </div>
        </div>
        
        <GoogleLoginButton />
      </div>
    </div>
  )
}