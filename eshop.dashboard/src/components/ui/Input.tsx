'use client';

import { colors } from '@/constants/colors';
import { forwardRef, InputHTMLAttributes } from 'react';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  fullWidth?: boolean;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, className = '', fullWidth = true, ...props }, ref) => {
    return (
      <div className={`flex flex-col gap-1 ${fullWidth ? 'w-full' : ''}`}>
        {label && (
          <label className="text-sm font-medium text-gray-700">{label}</label>
        )}
        <input
          ref={ref}
          className={`
            px-4 py-2 rounded-lg border text-base
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
            placeholder:text-gray-400
            placeholder:text-xs
            disabled:bg-gray-50
            disabled:text-gray-500
            disabled:cursor-not-allowed
            ${className}
          `}
          {...props}
        />
        {error && (
          <span className="text-sm text-red-500">{error}</span>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input'; 