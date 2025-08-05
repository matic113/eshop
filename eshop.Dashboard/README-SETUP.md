# eShop Admin Dashboard Setup

## Environment Configuration

You need to create environment files for your local and production environments.

### 1. Create `.env.local` file:

```env
# Local Development Environment Variables
NEXT_PUBLIC_APP_ENV=local

# API Configuration
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api

# Dashboard Configuration
NEXT_PUBLIC_DASHBOARD_NAME="eShop Admin Dashboard"
NEXT_PUBLIC_COMPANY_NAME="eShop"
```

### 2. Create `.env.production` file:

```env
# Production Environment Variables
NEXT_PUBLIC_APP_ENV=production

# API Configuration
NEXT_PUBLIC_API_BASE_URL=https://your-api-domain.com/api

# Dashboard Configuration
NEXT_PUBLIC_DASHBOARD_NAME="eShop Admin Dashboard"
NEXT_PUBLIC_COMPANY_NAME="eShop"
```

## Features Implemented

### ✅ Authentication System
- **Regular Login**: Email/password authentication using your backend API (`/user/login`)
- **Google OAuth**: Integration ready for Google login using your backend API (`/google-auth/google-login`)
- **Auto-redirect**: Users are automatically redirected based on authentication status
- **Token Management**: JWT tokens stored in localStorage with refresh token support

### ✅ TanStack Query Integration
- **Efficient Caching**: Smart caching and background updates
- **Error Handling**: Automatic retry logic and error management
- **DevTools**: React Query DevTools for debugging (development only)
- **API Layer**: Centralized API functions for all backend endpoints

### ✅ UI Components
- **Modern Design**: Clean, responsive interface with Tailwind CSS
- **Accessibility**: ARIA-compliant components
- **Loading States**: Proper loading indicators and error states
- **Form Validation**: Client-side validation with error feedback

### ✅ Project Structure
```
src/
├── app/
│   ├── login/page.tsx          # Login page
│   ├── dashboard/page.tsx      # Main dashboard
│   ├── layout.tsx              # Root layout with providers
│   └── page.tsx                # Home redirect logic
├── components/
│   ├── auth/
│   │   ├── login-form.tsx      # Email/password login form
│   │   └── google-login-button.tsx # Google OAuth button
│   └── ui/                     # Reusable UI components
├── lib/
│   ├── api.ts                  # API functions for backend integration
│   └── utils.ts                # Utility functions
└── providers/
    ├── auth-provider.tsx       # Authentication context
    └── query-provider.tsx      # TanStack Query setup
```

## API Integration

The dashboard is configured to work with your existing backend endpoints:

- **Authentication**: `/user/login`, `/google-auth/google-login`
- **Products**: `/products/*` endpoints
- **Orders**: `/orders/*` endpoints  
- **Categories**: `/categories/*` endpoints
- **And more**: All endpoints from your Features.md are supported

## Google OAuth Setup

The Google login button is ready for integration. To complete the setup:

1. **Backend**: Your API already handles Google OAuth tokens
2. **Frontend**: The `GoogleLoginButton` component will redirect to Google OAuth
3. **Token Flow**: Once Google provides the token, it's sent to your backend API

## Development

```bash
# Install dependencies
pnpm install

# Start development server
pnpm dev

# Build for production
pnpm build
```

## Routes

- `/` - Auto-redirects to dashboard or login based on auth status
- `/login` - Login page with email/password and Google options
- `/dashboard` - Main admin dashboard (protected route)

## Next Steps

1. Create the environment files above
2. Update the API base URLs to match your backend
3. Test the login functionality with your backend
4. Implement additional dashboard pages for products, orders, etc.

The foundation is complete and ready for expansion with your specific ecommerce features!