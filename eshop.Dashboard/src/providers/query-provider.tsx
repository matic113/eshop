'use client'

import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { useState } from 'react'

interface QueryProviderProps {
  children: React.ReactNode
}

export function QueryProvider({ children }: QueryProviderProps) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            staleTime: 60 * 1000, // 1 minute
            retry: (failureCount, error: any) => {
              // Don't retry on authentication errors
              if (error?.message?.includes('401') || error?.message?.includes('403') || 
                  error?.message?.includes('Unauthorized') || error?.message?.includes('Forbidden')) {
                return false
              }
              // Don't retry on 4xx errors (client errors)
              if (error?.message?.includes('400') || error?.message?.includes('404')) {
                return false
              }
              return failureCount < 2 // Reduce retry attempts
            },
            refetchOnWindowFocus: false, // Disable automatic refetch on window focus by default
          },
          mutations: {
            retry: (failureCount, error: any) => {
              // Don't retry mutations on client errors
              if (error?.message?.includes('4')) {
                return false
              }
              return failureCount < 1 // Only retry once for mutations
            },
          },
        },
      })
  )

  return (
    <QueryClientProvider client={queryClient}>
      {children}
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  )
}