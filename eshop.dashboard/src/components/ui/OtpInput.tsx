'use client';

import { colors } from '@/constants/colors';
import { ChangeEvent, KeyboardEvent, useRef, useState } from 'react';

interface OtpInputProps {
  length?: number;
  onComplete?: (otp: string) => void;
  error?: string;
}

export const OtpInput = ({ 
  length = 6, 
  onComplete,
  error 
}: OtpInputProps) => {
  const [otp, setOtp] = useState<string[]>(new Array(length).fill(''));
  const inputRefs = useRef<HTMLInputElement[]>([]);

  const focusInput = (index: number) => {
    if (inputRefs.current[index]) {
      inputRefs.current[index].focus();
    }
  };

  const handleChange = (e: ChangeEvent<HTMLInputElement>, index: number) => {
    const value = e.target.value;
    if (isNaN(Number(value))) return;

    const newOtp = [...otp];
    // Take only the last character if multiple characters are pasted
    newOtp[index] = value.slice(-1);
    setOtp(newOtp);

    // Move to next input if value is entered
    if (value && index < length - 1) {
      focusInput(index + 1);
    }

    // Check if OTP is complete
    const otpValue = newOtp.join('');
    if (otpValue.length === length && onComplete) {
      onComplete(otpValue);
    }
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>, index: number) => {
    if (e.key === 'Backspace') {
      e.preventDefault();
      if (otp[index]) {
        // Clear current input
        const newOtp = [...otp];
        newOtp[index] = '';
        setOtp(newOtp);
      } else if (index > 0) {
        // Move to previous input
        focusInput(index - 1);
      }
    } else if (e.key === 'ArrowLeft' && index > 0) {
      e.preventDefault();
      focusInput(index - 1);
    } else if (e.key === 'ArrowRight' && index < length - 1) {
      e.preventDefault();
      focusInput(index + 1);
    }
  };

  const handlePaste = (e: React.ClipboardEvent<HTMLInputElement>) => {
    e.preventDefault();
    const pastedData = e.clipboardData.getData('text/plain').slice(0, length);
    if (!/^\d+$/.test(pastedData)) return;

    const newOtp = [...otp];
    pastedData.split('').forEach((char, i) => {
      if (i < length) {
        newOtp[i] = char;
      }
    });
    setOtp(newOtp);

    if (pastedData.length === length && onComplete) {
      onComplete(pastedData);
    }
  };

  return (
    <div className="flex flex-col gap-2">
      <div className="flex gap-2 justify-center">
        {otp.map((digit, index) => (
          <input
            key={index}
            type="text"
            inputMode="numeric"
            maxLength={1}
            value={digit}
            ref={(ref) => {
              if (ref) inputRefs.current[index] = ref;
            }}
            onChange={(e) => handleChange(e, index)}
            onKeyDown={(e) => handleKeyDown(e, index)}
            onPaste={handlePaste}
            className={`
              w-12 h-12 text-center text-lg font-semibold
              border rounded-lg
              ${error ? 'border-red-500' : `border-[${colors.border}]`}
              focus:outline-none
              focus:ring-2
              ${error 
                ? 'focus:ring-red-200' 
                : `focus:ring-[${colors.primary.light}]/20`
              }
              ${error 
                ? 'focus:border-red-500' 
                : `focus:border-[${colors.primary.main}]`
              }
            `}
          />
        ))}
      </div>
      {error && (
        <span className="text-sm text-red-500 text-center">{error}</span>
      )}
    </div>
  );
}; 