'use client';

import { Button } from '@/components/ui/Button';
import { OtpInput } from '@/components/ui/OtpInput';
import { authService } from '@/services/auth';
import { useRouter } from 'next/navigation';
import { useState } from 'react';

export default function VerifyPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleVerify = async (otp: string) => {
    setError('');
    setLoading(true);

    try {
      const response = await authService.verifyOtp({
        email: localStorage.getItem('email') || '',
        otp,
      });

      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);
      router.push('/dashboard');
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Failed to verify OTP');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 bg-white p-8 rounded-xl shadow-lg">
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900">Verify your email</h2>
          <p className="mt-2 text-sm text-gray-600">
            Enter the 6-digit code sent to your email
          </p>
        </div>

        <div className="mt-8 space-y-6">
          <OtpInput
            length={6}
            onComplete={handleVerify}
            error={error}
          />

          <div className="text-center">
            <Button
              type="button"
              variant="text"
              loading={loading}
              onClick={() => {
                // TODO: Implement resend OTP
                alert('Resend OTP functionality will be implemented');
              }}
            >
              Didn't receive the code? Resend
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
} 