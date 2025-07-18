'use client';

import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { colors } from '@/constants/colors';
import { authService } from '@/services/auth';
import { LoginCredentials } from '@/types/auth';
// import { useGoogleLogin } from '@react-oauth/google';
import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { FormEvent, useState } from 'react';

export default function LoginPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [credentials, setCredentials] = useState<LoginCredentials>({
    email: '',
    password: '',
  });

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const response = await authService.login(credentials);
      // Store tokens
      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);
      
      if (!response.user.isVerified) {
        router.push('/auth/verify');
      } else {
        router.push('/dashboard');
      }
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Failed to login');
    } finally {
      setLoading(false);
    }
  };

  // const googleLogin = useGoogleLogin({
  //   onSuccess: async (response) => {
  //     setError('');
  //     setLoading(true);

  //     try {
  //       const authResponse = await authService.loginWithGoogle(response.access_token);
  //       localStorage.setItem('accessToken', authResponse.token);
  //       router.push('/dashboard');
  //     } catch (error) {
  //       setError(error instanceof Error ? error.message : 'Failed to login with Google');
  //     } finally {
  //       setLoading(false);
  //     }
  //   },
  //   onError: () => {
  //     setError('Failed to login with Google');
  //   },
  // });

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 bg-white p-8 rounded-xl shadow-lg">
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900">Welcome back</h2>
          <p className="mt-2 text-sm text-gray-600">
            Don't have an account?{' '}
            <Link href="/auth/signup" className="text-blue-600 hover:text-blue-500">
              Sign up
            </Link>
          </p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          <div className="space-y-4">
            <Input
              label="Email address"
              type="email"
              required
              placeholder="Enter your email address"
              value={credentials.email}
              onChange={(e) => setCredentials({ ...credentials, email: e.target.value })}
            />

            <Input
              label="Password"
              type="password"
              required
              placeholder="Enter your password"
              value={credentials.password}
              onChange={(e) => setCredentials({ ...credentials, password: e.target.value })}
            />
          </div>

          {error && (
            <div className="text-sm text-red-500 text-center">{error}</div>
          )}

          <div className="space-y-4">
            <Button
              type="submit"
              fullWidth
              loading={loading}
              style={{
                backgroundColor: colors.primary.main,
                color: colors.primary.contrastText, 
              }}
            >
              Sign in
            </Button>

            <div className="relative">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-gray-300" />
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-2 bg-white text-gray-500">Or continue with</span>
              </div>
            </div>

            <Button
              type="button"
              variant="outlined"
              fullWidth
              onClick={() => {
                console.log('Google login');
                // googleLogin();
              }}
              disabled={loading}
            >
              <Image
                src="/google.svg"
                alt="Google"
                width={20}
                height={20}
                className="mr-2"
              />
              Google
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
} 