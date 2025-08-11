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

// Cookie utilities for caching
const ADMIN_STATUS_COOKIE = 'admin_status'
const ADMIN_STATUS_EXPIRY_COOKIE = 'admin_status_expiry'
const PROFILE_PICTURE_CACHE_COOKIE = 'cached_profile_picture'
const PROFILE_PICTURE_EXPIRY_COOKIE = 'cached_profile_picture_expiry'
const USER_INFO_CACHE_COOKIE = 'cached_user_info'
const USER_INFO_EXPIRY_COOKIE = 'cached_user_info_expiry'
const CURRENT_USER_ID_COOKIE = 'current_user_id'

export const cookieUtils = {
  set: (name: string, value: string, expirationMinutes: number) => {
    const expiresAt = new Date(Date.now() + expirationMinutes * 60 * 1000)
    document.cookie = `${name}=${encodeURIComponent(value)}; expires=${expiresAt.toUTCString()}; path=/; SameSite=Strict`
  },

  get: (name: string): string | null => {
    const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'))
    return match ? decodeURIComponent(match[2]) : null
  },

  remove: (name: string) => {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/`
  },

  isExpired: (expiryKey: string): boolean => {
    const expiry = cookieUtils.get(expiryKey)
    if (!expiry) return true
    return new Date() > new Date(expiry)
  },

  // Clear all authentication-related cookies
  clearAllAuthCookies: () => {
    cookieUtils.remove(ADMIN_STATUS_COOKIE)
    cookieUtils.remove(ADMIN_STATUS_EXPIRY_COOKIE)
    cookieUtils.remove(PROFILE_PICTURE_CACHE_COOKIE)
    cookieUtils.remove(PROFILE_PICTURE_EXPIRY_COOKIE)
    cookieUtils.remove(USER_INFO_CACHE_COOKIE)
    cookieUtils.remove(USER_INFO_EXPIRY_COOKIE)
    cookieUtils.remove(CURRENT_USER_ID_COOKIE)
  },

  // Check if the cached data belongs to a different user
  isDataForDifferentUser: (userId: string): boolean => {
    const currentUserId = cookieUtils.get(CURRENT_USER_ID_COOKIE)
    return currentUserId !== null && currentUserId !== userId
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

  // Get current user profile with caching
  me: async (): Promise<{
    userId: string;
    email: string;
    fullName: string;
    profilePicture?: string;
  }> => {
    // First, fetch fresh data to get the current user ID
    const response = await apiCall<{
      userId: string;
      email: string;
      fullName: string;
      profilePicture?: string;
    }>('/api/auth/me')

    if (!response || !response.data) throw new Error('Failed to get user profile')
    
    const currentUserId = response.data.userId
    
    // If cached data is for a different user, clear all cache
    if (cookieUtils.isDataForDifferentUser(currentUserId)) {
      console.log('🔄 Different user detected, clearing all cached data')
      cookieUtils.clearAllAuthCookies()
    }
    
    // Check if we have valid cached data for THIS user
    if (!cookieUtils.isExpired(USER_INFO_EXPIRY_COOKIE)) {
      const cachedUserInfo = cookieUtils.get(USER_INFO_CACHE_COOKIE)
      if (cachedUserInfo) {
        try {
          const userData = JSON.parse(cachedUserInfo)
          // Verify the cached data is for the current user
          if (userData.userId === currentUserId) {
            // Return cached data with profile picture optimization
            return {
              ...userData,
              profilePicture: authApi.getCachedProfilePicture(userData.profilePicture)
            }
          }
        } catch (error) {
          // If parsing fails, continue to cache fresh data
          console.warn('Failed to parse cached user info:', error)
        }
      }
    }
    
    // Cache the fresh user info for 1 hour
    const userDataToCache = {
      userId: response.data.userId,
      email: response.data.email,
      fullName: response.data.fullName,
      profilePicture: response.data.profilePicture
    }
    
    cookieUtils.set(USER_INFO_CACHE_COOKIE, JSON.stringify(userDataToCache), 60)
    cookieUtils.set(USER_INFO_EXPIRY_COOKIE, new Date(Date.now() + 60 * 60 * 1000).toISOString(), 60)
    cookieUtils.set(CURRENT_USER_ID_COOKIE, currentUserId, 60)
    
    // Return data with profile picture caching
    return {
      ...response.data,
      profilePicture: authApi.getCachedProfilePicture(response.data.profilePicture)
    }
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
    // Clear ALL authentication-related data on logout
    cookieUtils.clearAllAuthCookies()
  },

  // Check if user is admin with caching
  checkAdmin: async (): Promise<boolean> => {
    // First get current user to ensure we're checking admin for the right user
    const currentUser = await authApi.me()
    const currentUserId = currentUser.userId
    
    // If cached data is for a different user, clear admin cache
    if (cookieUtils.isDataForDifferentUser(currentUserId)) {
      cookieUtils.remove(ADMIN_STATUS_COOKIE)
      cookieUtils.remove(ADMIN_STATUS_EXPIRY_COOKIE)
    }
    
    // Check cache first
    if (!cookieUtils.isExpired(ADMIN_STATUS_EXPIRY_COOKIE)) {
      const cachedStatus = cookieUtils.get(ADMIN_STATUS_COOKIE)
      if (cachedStatus !== null) {
        return cachedStatus === 'true'
      }
    }

    try {
      // Make API call to check admin status
      await apiCall('/api/auth/admin')
      
      // Cache the positive result for 1 hour
      cookieUtils.set(ADMIN_STATUS_COOKIE, 'true', 60)
      cookieUtils.set(ADMIN_STATUS_EXPIRY_COOKIE, new Date(Date.now() + 60 * 60 * 1000).toISOString(), 60)
      
      return true
    } catch (error) {
      // Cache the negative result for 1 hour
      cookieUtils.set(ADMIN_STATUS_COOKIE, 'false', 60)
      cookieUtils.set(ADMIN_STATUS_EXPIRY_COOKIE, new Date(Date.now() + 60 * 60 * 1000).toISOString(), 60)
      
      return false
    }
  },

  // Get cached profile picture or return original
  getCachedProfilePicture: (originalUrl?: string): string | undefined => {
    if (!originalUrl) return originalUrl

    // Check if we have a cached version that's not expired
    if (!cookieUtils.isExpired(PROFILE_PICTURE_EXPIRY_COOKIE)) {
      const cached = cookieUtils.get(PROFILE_PICTURE_CACHE_COOKIE)
      if (cached && cached === originalUrl) {
        // Return the same URL but we know it's "cached" in the sense that we're not making new requests
        return originalUrl
      }
    }

    // Cache the profile picture URL for 24 hours
    cookieUtils.set(PROFILE_PICTURE_CACHE_COOKIE, originalUrl, 24 * 60)
    cookieUtils.set(PROFILE_PICTURE_EXPIRY_COOKIE, new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(), 24 * 60)
    
    return originalUrl
  },

  // Clear user info cache (useful for forced refresh)
  clearUserCache: () => {
    cookieUtils.clearAllAuthCookies()
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

// Orders domain types
export type OrderStatus =
  | 'Pending'
  | 'Completed'
  | 'Failed'
  | 'Processing'
  | 'Shipped'
  | 'Delivered'
  | 'Cancelled'

export type OrderPeriod = '1d' | '3d' | '7d' | '30d'

export interface OrderCustomerInfo {
  customerId: string
  fullName: string
  email: string
  phoneNumber: string
}

export interface OrderShippingDetails {
  addressId: string
  state: string
  city: string
  street: string
  apartment: string
  notes?: string
  phoneNumber: string
}

export interface Order {
  orderId: string
  orderCode: string
  createdAt: string
  updatedAt: string
  status: OrderStatus
  totalPrice: number
  shippingPrice: number
  discountAmout?: number
  couponCode?: string | null
  paymentMethod: string
  customerInfo: OrderCustomerInfo
  shippingDetails: OrderShippingDetails
}

export interface OrdersAdminParams {
  period?: OrderPeriod
  page?: number
  pageSize?: number
}

export interface OrdersAdminResponse {
  period: OrderPeriod
  orders: {
    items: Order[]
    page: number
    pageSize: number
    totalCount: number
    hasNextPage: boolean
    hasPreviousPage: boolean
  }
}

export interface UpdateOrderStatusPayload {
  orderId: string
  status: OrderStatus
  notes?: string
}

// Orders API functions
export const ordersApi = {
  getAdmin: async (
    params: OrdersAdminParams = {}
  ): Promise<OrdersAdminResponse> => {
    const query = new URLSearchParams()
    const page = params.page || 1
    const pageSize = params.pageSize || 20
    const period = params.period || '1d'
    query.append('page', String(page))
    query.append('pageSize', String(pageSize))
    query.append('period', period)

    const response = await apiCall<OrdersAdminResponse>(
      `/api/orders/admin?${query.toString()}`
    )
    if (!response || !response.data) throw new Error('Failed to fetch orders')
    return response.data
  },

  updateStatus: async (payload: UpdateOrderStatusPayload) => {
    const response = await apiCall(`/api/orders/admin/order-status`, {
      method: 'PUT',
      body: JSON.stringify(payload),
    })
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