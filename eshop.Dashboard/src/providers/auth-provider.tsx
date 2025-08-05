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
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

interface AuthProviderProps {
  children: React.ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const router = useRouter()
  const queryClient = useQueryClient()

  // Use React Query to fetch and cache user data
  const {
    data: user,
    isLoading,
    refetch: refetchUser,
  } = useQuery({
    queryKey: ['auth', 'me'],
    queryFn: authApi.me,
    retry: false, // Don't retry on auth failures
    staleTime: 5 * 60 * 1000, // 5 minutes
    refetchOnWindowFocus: true, // Refetch when user returns to tab
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