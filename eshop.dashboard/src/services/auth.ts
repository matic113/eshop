import { AuthResponse, GoogleAuthResponse, LoginCredentials, OtpVerification, SignupData } from '@/types/auth';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

export const authService = {
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/api/user/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(credentials),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to login');
    }

    return response.json();
  },

  async signup(data: SignupData): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/api/user/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to signup');
    }

    return response.json();
  },

  async verifyOtp(data: OtpVerification): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/api/user/verify-email`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to verify OTP');
    }

    return response.json();
  },

  async loginWithGoogle(token: string): Promise<GoogleAuthResponse> {
    const response = await fetch(`${API_URL}/api/google-auth/callback`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ token }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to login with Google');
    }

    return response.json();
  },

  async refreshToken(token: string): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/api/user/refresh-token`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ token }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to refresh token');
    }

    return response.json();
  },
}; 