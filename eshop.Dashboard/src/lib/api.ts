const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || ''

interface ApiResponse<T> {
  data: T
}

export interface Product {
  id: string
  productCode: string
  sellerId: string
  name: string
  description: string
  nameArabic?: string
  descriptionArabic?: string
  coverPictureUrl: string
  price: number
  stock: number
  weight: number
  color: string
  rating?: number
  reviewsCount?: number
  discountPercentage: number
  categoryIds?: string[]
  categories?: string[]
  productPictureUrls?: string[] // For backward compatibility with list endpoint
  productPictures?: string[] // For detail endpoint
  createdAt?: string
  updatedAt?: string
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface Category {
  id: string
  name: string
  description: string
  coverPictureUrl: string
}

export interface CategoriesResponse {
  categories: Category[]
}

export interface ImageUploadResponse {
  uploadUrl: string
  publicImageUrl: string
}

export interface ProductUpdateData {
  name?: string
  nameArabic?: string
  description?: string
  descriptionArabic?: string
  coverPictureUrl?: string
  price?: number
  stock?: number
  weight?: number
  color?: string
  discountPercentage?: number
  categoryIds?: string[]
  productPictures?: string[]
}

interface LoginCredentials {
  email: string
  password: string
}

interface AuthResponse {
  data: {
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
  const url = `${API_BASE_URL}${endpoint}`;

  const defaultHeaders: Record<string, string> = {};

  if (options.body) {
    defaultHeaders['Content-Type'] = 'application/json';
  }

  const config: RequestInit = {
    ...options,
    credentials: 'include',
    headers: {
      ...defaultHeaders,
      ...options.headers,
    },
  };

  const response = await fetch(url, config);

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({
      message: `HTTP ${response.status}: ${response.statusText || 'Network error'}`,
      status: response.status,
    }));

    const error = new Error(errorData.message || `HTTP error! status: ${response.status}`);
    (error as any).status = response.status;
    (error as any).errors = errorData.errors;

    throw error;
  }

  //  ⬇️⬇️  -- هذا هو التعديل الرئيسي --  ⬇️⬇️

  if (response.status === 200 && options.method === 'DELETE') {
    return { data: null as T }; // أو يمكنك إرجاع { data: true as T } إذا كان ذلك أفضل
  }

  //  ⬆️⬆️  -- نهاية التعديل الرئيسي --  ⬆️⬆️


  // الآن، فقط إذا لم تكن الاستجابة 204، قم بمحاولة قراءة الـ JSON
  const data = await response.json();


  if (data && typeof data === 'object' && 'data' in data) {
    return data;
  } else {
    return { data };
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

    if (!response || !response.data) throw new Error('Failed to get user profile')
    return response.data
  },

  // Refresh token
  refreshToken: async () => {
    const response = await apiCall('/api/auth/refresh-token', {
      method: 'POST',
      body: JSON.stringify({
        useCookies: true
      }),
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

    if (!response || !response.data) throw new Error('Failed to fetch products')
    return response.data
  },

  getById: async (id: string): Promise<Product> => {
    const response = await apiCall<Product>(`/api/products/${id}`)
    if (!response || !response.data) throw new Error('Failed to fetch product details')
    return response.data
  },

  create: async (productData: any) => {
    const response = await apiCall('/api/products', {
      method: 'POST',
      body: JSON.stringify(productData),
    })
    return response
  },

  update: async (id: string, productData: any) => {
    const response = await apiCall(`/api/products/${id}`, {
      method: 'PUT',
      body: JSON.stringify(productData),
    })
    return response
  },

  delete: async (id: string) => {
    const response = await apiCall(`/api/products/${id}`, {
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



export interface CreateCategoryPayload {
  name: string;
  Description: string;
  CoverPictureUrl: string;
}

export interface UpdateCategoryPayload {
  newName: string;
  newDescription: string;
  NewCoverPictureUrl: string;
}

// Categories API functions
export const categoriesApi = {
  getAll: async (): Promise<CategoriesResponse> => {
    const response = await apiCall<CategoriesResponse>('/api/categories/')
    if (!response || !response.data) throw new Error('Failed to fetch categories')
    return response.data
  },

  create: async (categoryData: CreateCategoryPayload) => {
    const response = await apiCall('/api/categories/', {
      method: 'POST',
      body: JSON.stringify(categoryData),
    })
    return response
  },

  delete: async (id: string) => {
    try {
      const response = await apiCall(`/api/categories/${id}`, {
        method: 'DELETE',
      })
      console.log(response);
      return response
    } catch (error) {
      console.error('Failed to delete category:', error)
      throw error
    }
  },

  update: async (id: string, categoryData: UpdateCategoryPayload) => {
    const response = await apiCall(`/api/categories/${id}`, {
      method: 'PUT',
      body: JSON.stringify(categoryData),
    })
    return response
  },
}

// Files API functions
export const filesApi = {
  getImageUploadUrl: async (): Promise<ImageUploadResponse> => {
    const response = await apiCall<ImageUploadResponse>('/api/files/image')
    if (!response || !response.data) throw new Error('Failed to get image upload URL')
    return response.data
  },

  uploadImage: async (uploadUrl: string, file: File): Promise<void> => {
    const response = await fetch(uploadUrl, {
      method: 'PUT',
      body: file,
      headers: {
        'Content-Type': file.type,
      },
    })

    if (!response.ok) {
      throw new Error('Failed to upload image')
    }
  },
}

// Offers API interfaces
export interface Offer {
  id: string
  name: string
  description: string
  coverUrl: string
  createdAt: string
}

export interface OffersResponse {
  offers: {
    items: Offer[]
  }
}

export interface CreateOfferRequest {
  name: string
  description: string
  offerCoverUrl: string
}

// Offers API functions
export const offersApi = {
  getAll: async (): Promise<OffersResponse> => {
    const response = await apiCall<OffersResponse>('/api/offers')
    if (!response || !response.data) throw new Error('Failed to fetch offers')
    return response.data
  },

  create: async (offerData: CreateOfferRequest) => {
    const response = await apiCall('/api/offers', {
      method: 'POST',
      body: JSON.stringify(offerData),
    })
    return response
  },

  delete: async (id: string) => {
    const response = await apiCall(`/api/offers/${id}`, {
      method: 'DELETE',
    })
    return response
  },
}

// Coupons API interfaces
export type CouponType = 'FixedAmount' | 'Percentage' | 'FreeShipping'

export interface Coupon {
  id: string
  couponCode: string
  couponType: CouponType
  expiresAt: string
  usagesLeft: number
  timesPerUser: number
  discountValue: number
  maxDiscount: number
}

export interface CouponsResponse {
  coupons: Coupon[]
}

export interface CreateCouponRequest {
  couponCode: string
  couponType: CouponType
  expirationDate: string // ISO string with Z
  usageTimes: number
  timesPerUser: number
  discountValue: number
  maxDiscount: number
}

// Coupons API functions
export const couponsApi = {
  getAll: async (): Promise<CouponsResponse> => {
    const response = await apiCall<CouponsResponse>('/api/coupons')
    if (!response || !response.data) throw new Error('Failed to fetch coupons')
    return response.data
  },

  create: async (couponData: CreateCouponRequest) => {
    const response = await apiCall('/api/coupons', {
      method: 'POST',
      body: JSON.stringify(couponData),
    })
    return response
  },
}