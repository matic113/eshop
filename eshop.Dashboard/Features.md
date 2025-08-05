# Backend Features Documentation

This document outlines all the features available in the eShop backend API, organized by functional categories. Each feature is described with its capabilities, endpoints, and data models to facilitate dashboard implementation.

## 🔐 Authentication & User Management

### User Authentication
**Location:** `eshop.API/Features/User/`

**Core Capabilities:**
- **User Registration** (`Register.cs`): Complete user account creation with email verification
- **User Login** (`Login.cs`): Secure authentication with JWT tokens and password validation
- **Password Management**: 
  - Forgot Password (`ForgotPassword.cs`): Initiate password reset via email
  - Reset Password (`ResetPassword.cs`): Complete password reset with validation
  - Change Password (`ChangePassword.cs`): Update password for authenticated users
- **Email Verification** (`VerifyEmail.cs`): Confirm user email addresses
- **OTP Management**:
  - Validate OTP (`ValidateOtp.cs`): Verify one-time passwords
  - Resend OTP (`ResendOtp.cs`): Request new OTP codes
- **Token Management**:
  - Refresh Token (`RefreshToken.cs`): Renew access tokens
  - Get User Profile (`Me.cs`): Retrieve authenticated user information

**Dashboard Requirements:**
- User management interface with registration/login forms
- Password reset workflow UI
- Email verification status tracking
- OTP verification interface
- Token management and session monitoring

### Google Authentication
**Location:** `eshop.API/Features/GoogleAuth/`

**Core Capabilities:**
- **Google OAuth Login** (`GoogleLogin.cs`): OAuth2 integration for web applications
- **Mobile Google Login** (`MobileLogin.cs`): Google authentication for mobile apps
- **Authentication Callback** (`Callback.cs`): Handle OAuth callback processing

**Dashboard Requirements:**
- Social login integration buttons
- OAuth flow management interface
- Mobile authentication support

## 🛍️ Product Management

### Product Operations
**Location:** `eshop.API/Features/Products/`

**Core Capabilities:**
- **Product Creation** (`Create.cs`): Add new products with comprehensive details
  - Product information (name, description, price, weight)
  - Category assignment
  - Seller association
  - Stock management
  - Image URL handling
- **Product Listing** (`ListAll.cs`): Retrieve all products with filtering and pagination
- **Product Details** (`Get.cs`): Fetch individual product information
- **Product Updates** (`Update.cs`): Modify existing product details
- **Product Deletion** (`Delete.cs`): Remove products from catalog

**Data Models:**
- Product ID, Name, Description
- Price, Weight, Stock quantity
- Category association
- Seller information
- Image URLs
- Creation/modification timestamps

**Dashboard Requirements:**
- Product catalog management interface
- CRUD operations for products
- Image upload and management
- Stock level monitoring
- Category assignment interface
- Seller-product relationship management

### Category Management
**Location:** `eshop.API/Features/Categories/`

**Core Capabilities:**
- **Category Creation** (`Create.cs`): Add new product categories
- **Category Listing** (`ListAllCategories.cs`): Retrieve all available categories
- **Category Details** (`Get.cs`): Fetch individual category information
- **Category Updates** (`Update.cs`): Modify category details
- **Category Deletion** (`Delete.cs`): Remove categories

**Dashboard Requirements:**
- Category hierarchy management
- Category CRUD interface
- Product-category relationship visualization

## 🛒 Shopping Cart & Orders

### Shopping Cart Management
**Location:** `eshop.API/Features/Carts/`

**Core Capabilities:**
- **Cart Retrieval** (`Get.cs`): Get user's current cart with all items
- **Add to Cart** (`CreateItem.cs`): Add products to shopping cart
- **Update Cart Items** (`UpdateItem.cs`): Modify item quantities
- **Remove from Cart** (`DeleteItem.cs`): Remove items from cart
- **Quantity Management** (`DecrementItem.cs`): Decrease item quantities

**Data Models:**
- Cart ID and user association
- Cart items with product details
- Quantity and pricing information
- Total cart value calculation

**Dashboard Requirements:**
- Shopping cart management interface
- Real-time cart updates
- Item quantity controls
- Cart abandonment tracking

### Order Processing
**Location:** `eshop.API/Features/Orders/`

**Core Capabilities:**
- **Order Checkout** (`Checkout.cs`): Process cart items into orders
  - Shipping address selection
  - Payment method choice (Cash on Delivery, Paymob)
  - Order validation and creation
- **Order History** (`GetHistory.cs`): Retrieve user's past orders
- **Order Listing** (`GetAll.cs`): Administrative order management

**Payment Integration:**
- Cash on Delivery support
- Paymob payment gateway integration
- Unified checkout URL generation

**Dashboard Requirements:**
- Order management interface
- Payment status tracking
- Shipping address management
- Order history visualization
- Payment method configuration

## 📍 Address Management

### User Addresses
**Location:** `eshop.API/Features/Addresses/`

**Core Capabilities:**
- **Address Creation** (`Create.cs`): Add new shipping addresses
  - State, city, street, apartment details
  - Phone number validation
  - Delivery notes
- **Address Listing** (`ListUserAddresses.cs`): Get all user addresses
- **Address Details** (`Get.cs`): Retrieve specific address information
- **Address Updates** (`Update.cs`): Modify existing addresses
- **Address Deletion** (`Delete.cs`): Remove addresses

**Validation Rules:**
- Egyptian phone number format validation
- Required fields for complete addresses
- Character limits for each field

**Dashboard Requirements:**
- Address book management
- Address validation interface
- Default address selection
- Delivery area coverage mapping

## ⭐ Reviews & Ratings

### Product Reviews
**Location:** `eshop.API/Features/Reviews/`

**Core Capabilities:**
- **Review Creation** (`Create.cs`): Add product reviews
  - Star rating (1-5 scale)
  - Optional comment (max 500 characters)
  - User and product association
- **Review Retrieval** (`GetProductReviews.cs`): Get all reviews for a product

**Dashboard Requirements:**
- Review management interface
- Rating analytics and statistics
- Review moderation tools
- Product rating aggregation

## 🎯 Offers & Promotions

### Promotional Offers
**Location:** `eshop.API/Features/Offers/`

**Core Capabilities:**
- **Offer Creation** (`Create.cs`): Create promotional offers
  - Offer name and description
  - Cover image management
  - Offer validation
- **Offer Listing** (`ListAll.cs`): Retrieve all active offers

**Dashboard Requirements:**
- Promotional campaign management
- Offer creation and editing interface
- Campaign performance tracking
- Image upload for offer covers

## 🔔 Notification System

### User Notifications
**Location:** `eshop.API/Features/Notifications/`

**Core Capabilities:**
- **Individual Notifications** (`NotifiyUser.cs`): Send notifications to specific users
- **Broadcast Notifications** (`NotifyAll.cs`): Send notifications to all users
- **Notification Retrieval** (`GetUserNotifications.cs`): Get user's notifications
- **Mark as Read** (`MarkAsRead.cs`): Update notification read status

**Notification Features:**
- Text message support (max 250 characters)
- Read/unread status tracking
- User targeting capabilities

**Dashboard Requirements:**
- Notification management interface
- Bulk notification sending
- Notification analytics
- Read status monitoring

## 📁 File Management

### Image Upload
**Location:** `eshop.API/Features/Files/`

**Core Capabilities:**
- **Image Upload** (`ImageUpload.cs`): Generate secure upload URLs
  - Integration with R2 object storage
  - Public image URL generation
  - Secure upload endpoint creation

**Dashboard Requirements:**
- File upload interface
- Image gallery management
- Storage quota monitoring
- Image optimization tools

## 🔗 Webhook Integration

### Payment Webhooks
**Location:** `eshop.API/Features/Webhooks/`

**Core Capabilities:**
- **Paymob Integration** (`Paymob.cs`): Handle payment gateway webhooks
  - Payment status updates
  - Transaction verification
  - Order completion processing

**Dashboard Requirements:**
- Webhook monitoring interface
- Payment status tracking
- Failed webhook retry mechanism
- Integration health monitoring

## 🎛️ Dashboard Implementation Considerations

### API Architecture
- Built with FastEndpoints framework
- Clean Architecture pattern implementation
- JWT-based authentication
- Comprehensive validation using FluentValidation
- Repository pattern for data access

### Key Data Relationships
- Users ↔ Products (Seller relationship)
- Users ↔ Orders ↔ Addresses
- Products ↔ Categories
- Products ↔ Reviews ↔ Users
- Users ↔ Carts ↔ Products
- Orders ↔ Notifications

### Security Considerations
- JWT token authentication required for most endpoints
- Role-based access control implementation
- Input validation on all endpoints
- Secure file upload with presigned URLs

### Performance Features
- Pagination support for large datasets
- Efficient querying with repository pattern
- Optimized cart and order processing

This documentation provides the foundation for implementing a comprehensive dashboard that can manage all aspects of the eShop backend functionality.