'use client'

import { createContext, useContext, useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { authApi } from '@/lib/api'

interface User {
  userId: string
  email: string
  fullName: string
  profilePicture?: string
  isAdmin?: boolean
}

type LoginResponse = User

interface AuthContextType {
  user: User | null
  isLoading: boolean
  login: (user: LoginResponse) => void
  logout: () => void
  refetchUser: () => void
  isAuthenticated: boolean
  isFetched: boolean
  isAdmin: boolean
  isAdminLoading: boolean
  checkAdminStatus: () => void
  clearUserCache: () => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

interface AuthProviderProps {
  children: React.ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const router = useRouter()
  const queryClient = useQueryClient()

  // Enhanced authentication function that tries refresh if initial auth fails
  const attemptAuthentication = async (): Promise<User | null> => {
    try {
      // First, try to get user data directly (now with built-in caching)
      const userData = await authApi.me()
      return userData
    } catch (error) {
      try {
        // If direct auth fails, try to refresh the token
        await authApi.refreshToken()
        // After successful refresh, try to get user data again
        const userData = await authApi.me()
        return userData
      } catch (refreshError) {
        // Both attempts failed, user is not authenticated
        return null
      }
    }
  }

  // Use React Query to fetch and cache user data
  const {
    data: user,
    isLoading,
    refetch: refetchUser,
    isFetched,
  } = useQuery({
    queryKey: ['auth', 'me'],
    queryFn: attemptAuthentication,
    retry: false, // Don't retry on auth failures - we handle it manually
    staleTime: 60 * 60 * 1000, // 1 hour - matches our cookie cache
    refetchOnWindowFocus: false, // Don't refetch on focus - we have cookie cache
  })

  // Admin status checking with React Query
  const {
    data: isAdmin = false,
    isLoading: isAdminLoading,
    refetch: refetchAdminStatus,
  } = useQuery({
    queryKey: ['auth', 'admin'],
    queryFn: authApi.checkAdmin,
    enabled: !!user, // Only check admin status if user is authenticated
    retry: false,
    staleTime: 60 * 60 * 1000, // 1 hour - matches our cookie cache
    refetchOnWindowFocus: false, // Don't refetch on focus - we have cookie cache
  })

  console.log('🔐 Auth Provider State:', {
    user,
    isLoading,
    isAuthenticated: !!user,
    isFetched,
    isAdmin,
    isAdminLoading
  })

  // Logout mutation
  const logoutMutation = useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => {
      // Clear all queries when logging out (including admin status)
      queryClient.clear()
      // Also specifically clear admin-related queries
      queryClient.removeQueries({ queryKey: ['auth', 'admin'] })
      queryClient.removeQueries({ queryKey: ['auth', 'me'] })
      router.push('/login')
    },
    onError: (error) => {
      console.error('Logout error:', error)
      // Even if logout fails, clear local state and redirect
      queryClient.clear()
      queryClient.removeQueries({ queryKey: ['auth', 'admin'] })
      queryClient.removeQueries({ queryKey: ['auth', 'me'] })
      router.push('/login')
    },
  })

  const login = (userData: User) => {
    // Clear ALL cached auth data before setting new user data
    authApi.clearUserCache()
    // Clear React Query cache to force fresh data
    queryClient.clear()
    // Update the query cache with new user data
    queryClient.setQueryData(['auth', 'me'], userData)
    router.push('/dashboard')
  }

  const logout = () => {
    logoutMutation.mutate()
  }

  const value: AuthContextType = {
    user: user as User || null,
    isLoading,
    login,
    logout,
    refetchUser: () => {
      refetchUser()
    },
    isAuthenticated: !!user,
    isFetched,
    isAdmin,
    isAdminLoading,
    checkAdminStatus: () => {
      refetchAdminStatus()
    },
    clearUserCache: () => {
      authApi.clearUserCache()
      // Also refetch user data after clearing cache
      refetchUser()
    },
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}