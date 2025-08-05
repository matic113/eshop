'use client'

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { productsApi, ordersApi, categoriesApi, type ProductSearchParams, type Product, type PaginatedResponse } from '@/lib/api'

// Products hooks
export function useProducts(params: ProductSearchParams = {}) {
  return useQuery<PaginatedResponse<Product>>({
    queryKey: ['products', params],
    queryFn: () => productsApi.getAll(params),
    staleTime: 5 * 60 * 1000, // 5 minutes
  })
}

// Hook for basic product list (backward compatibility)
export function useProductsList() {
  return useProducts({ page: 1, pageSize: 10 })
}

export function useCreateProduct() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: productsApi.create,
    onSuccess: () => {
      // Invalidate products query to refetch data
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}

export function useUpdateProduct() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: any }) => 
      productsApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}

export function useDeleteProduct() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: productsApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}

// Orders hooks
export function useOrders() {
  return useQuery({
    queryKey: ['orders'],
    queryFn: ordersApi.getAll,
    staleTime: 2 * 60 * 1000, // 2 minutes (orders change more frequently)
  })
}

// Categories hooks
export function useCategories() {
  return useQuery({
    queryKey: ['categories'],
    queryFn: categoriesApi.getAll,
    staleTime: 10 * 60 * 1000, // 10 minutes (categories change less frequently)
  })
}

export function useCreateCategory() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: categoriesApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
    },
  })
}