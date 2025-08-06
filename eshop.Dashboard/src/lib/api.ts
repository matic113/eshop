const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000'

export interface Product {
  id: string
  productCode: string
  name: string
  description: string
  arabicName: string
  arabicDescription: string
  coverPictureUrl: string
  price: number
  stock: number
  weight: number
  color: string
  rating: number
  reviewsCount: number
  discountPercentage: number
  sellerId: string
  categories: string[]
}

export interface PaginatedResponse<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

interface ApiResponse<T = any> {
  data: T
  message?: string
  errors?: Record<string, string[]>
}

interface LoginCredentials {
  email: string
  password: string
}

interface GoogleLoginData {
  token: string
}

interface AuthResponse {
  token: string
  refreshToken: string
  user: {
    userId: string
    email: string
    fullName: string
    profilePicture?: string
  }
}

// Generic API call function
async function apiCall<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<ApiResponse<T>> {
  const url = `${API_BASE_URL}${endpoint}`
  
  const defaultHeaders: Record<string, string> = {}
  
  // Only add Content-Type header if we're sending a body
  if (options.body) {
    defaultHeaders['Content-Type'] = 'application/json'
  }

  // HTTP-only cookies will be automatically included by the browser

  const config: RequestInit = {
    ...options,
    credentials: 'include', // Include cookies in requests
    headers: {
      ...defaultHeaders,
      ...options.headers,
    },
  }

  const response = await fetch(url, config)
  
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ 
      message: `HTTP ${response.status}: ${response.statusText || 'Network error'}`,
      status: response.status 
    }))
    
    // Create a more descriptive error
    const error = new Error(errorData.message || `HTTP error! status: ${response.status}`)
    ;(error as any).status = response.status
    ;(error as any).errors = errorData.errors
    
    throw error
  }

  const data = await response.json()
  
  // Check if the response is already wrapped in a data property
  // If not, wrap it to match our ApiResponse interface
  if (data && typeof data === 'object' && 'data' in data) {
    return data
  } else {
    return { data }
  }
}

// Authentication API functions
export const authApi = {
  // Regular email/password login
  login: async (credentials: LoginCredentials): Promise<{
    userId: string;
    email: string;
    fullName: string;
    profilePicture?: string;
  }> => {
    const response = await apiCall<{
      userId: string;
      email: string;
      fullName: string;
      profilePicture?: string;
    }>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials),
    })
    if (!response || !response.data) throw new Error('Login failed')
    return response.data
  },

  // Google OAuth login - redirect to server endpoint
  googleLogin: async (returnPath: string = '/dashboard'): Promise<void> => {
    const fullReturnUrl = `${window.location.origin}${returnPath}`
    const url = `${API_BASE_URL}/api/auth/google/login?returnUrl=${encodeURIComponent(fullReturnUrl)}`
    window.location.href = url
  },

  // Get current user profile
  me: async (): Promise<{
    userId: string;
    email: string;
    fullName: string;
    profilePicture?: string;
  }> => {
    const response = await apiCall<{
      userId: string;
      email: string;
      fullName: string;
      profilePicture?: string;
    }>('/api/auth/me')
    console.log('Auth me response:', response)
    if (!response || !response.data) throw new Error('Failed to get user profile')
    return response.data
  },

  // Refresh token
  refreshToken: async () => {
    const response = await apiCall('/api/auth/refresh-token', {
      method: 'POST',
    })
    return response
  },

  // Logout
  logout: async (): Promise<void> => {
    await apiCall('/api/auth/logout', {
      method: 'POST',
    })
  },
}

// Product search parameters interface
export interface ProductSearchParams {
  searchTerm?: string | null
  category?: string | null
  minPrice?: number | null
  maxPrice?: number | null
  isInStock?: boolean | null
  sortBy?: string | null
  sortOrder?: 'asc' | 'desc' | null
  page?: number
  pageSize?: number
}

// Products API functions
export const productsApi = {
  getAll: async (params: ProductSearchParams = {}): Promise<PaginatedResponse<Product>> => {
    // Build query string with only non-null values
    const queryString = new URLSearchParams()
    
    // Add default values if not provided
    const page = params.page || 1
    const pageSize = params.pageSize || 10
    queryString.append('page', page.toString())
    queryString.append('pageSize', pageSize.toString())
    
    // Only add other parameters if they have actual values
    if (params.searchTerm && params.searchTerm.trim() !== '') {
      queryString.append('searchTerm', params.searchTerm)
    }
    if (params.category && params.category.trim() !== '') {
      queryString.append('category', params.category)
    }
    if (params.minPrice !== null && params.minPrice !== undefined && params.minPrice >= 0) {
      queryString.append('minPrice', params.minPrice.toString())
    }
    if (params.maxPrice !== null && params.maxPrice !== undefined && params.maxPrice >= 0) {
      queryString.append('maxPrice', params.maxPrice.toString())
    }
    if (params.isInStock !== null && params.isInStock !== undefined) {
      queryString.append('isInStock', params.isInStock.toString())
    }
    if (params.sortBy && params.sortBy.trim() !== '') {
      queryString.append('sortBy', params.sortBy)
    }
    if (params.sortOrder && (params.sortOrder === 'asc' || params.sortOrder === 'desc')) {
      queryString.append('sortOrder', params.sortOrder)
    }

    const response = await apiCall<PaginatedResponse<Product>>(`/api/products?${queryString.toString()}`)
    console.log('Products API response:', response)
    if (!response || !response.data) throw new Error('Failed to fetch products')
    return response.data
  },

  create: async (productData: any) => {
    const response = await apiCall('/products/create', {
      method: 'POST',
      body: JSON.stringify(productData),
    })
    return response
  },

  update: async (id: string, productData: any) => {
    const response = await apiCall(`/products/update/${id}`, {
      method: 'PUT',
      body: JSON.stringify(productData),
    })
    return response
  },

  delete: async (id: string) => {
    const response = await apiCall(`/products/delete/${id}`, {
      method: 'DELETE',
    })
    return response
  },
}

// Orders API functions
export const ordersApi = {
  getAll: async () => {
    const response = await apiCall('/orders/get-all')
    return response
  },
}

// Categories API functions
export const categoriesApi = {
  getAll: async () => {
    const response = await apiCall('/categories/list-all-categories')
    return response
  },

  create: async (categoryData: any) => {
    const response = await apiCall('/categories/create', {
      method: 'POST',
      body: JSON.stringify(categoryData),
    })
    return response
  },
}