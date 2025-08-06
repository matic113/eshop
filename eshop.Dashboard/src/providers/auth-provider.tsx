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
      // First, try to get user data directly
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
    staleTime: 5 * 60 * 1000, // 5 minutes
    refetchOnWindowFocus: true, // Refetch when user returns to tab
  })

  console.log('🔐 Auth Provider State:', { 
    user, 
    isLoading, 
    isAuthenticated: !!user,
    isFetched
  })

  // Logout mutation
  const logoutMutation = useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => {
      // Clear all queries when logging out
      queryClient.clear()
      router.push('/login')
    },
    onError: (error) => {
      console.error('Logout error:', error)
      // Even if logout fails, clear local state and redirect
      queryClient.clear()
      router.push('/login')
    },
  })

  const login = (userData: User) => {
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