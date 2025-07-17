'use client';

import { colors } from '@/constants/colors';
import { ButtonHTMLAttributes, ReactNode } from 'react';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children: ReactNode;
  variant?: 'contained' | 'outlined' | 'text';
  fullWidth?: boolean;
  loading?: boolean;
}

export const Button = ({
  children,
  variant = 'contained',
  className = '',
  fullWidth = false,
  loading = false,
  disabled,
  ...props
}: ButtonProps) => {
  const baseStyles = 'px-6 py-2 rounded-lg font-medium transition-all duration-200 flex items-center justify-center gap-2';
  
  const variantStyles = {
    contained: `
      bg-[${colors.primary.main}]
      text-white
      hover:bg-[${colors.primary.dark}]
      disabled:bg-[${colors.primary.light}]/50
      disabled:cursor-not-allowed
    `,
    outlined: `
      border-2
      border-[${colors.primary.main}]
      text-[${colors.primary.main}]
      hover:bg-[${colors.primary.main}]/5
      disabled:border-[${colors.primary.light}]/50
      disabled:text-[${colors.primary.light}]/50
      disabled:cursor-not-allowed
    `,
    text: `
      text-[${colors.primary.main}]
      hover:bg-[${colors.primary.main}]/5
      disabled:text-[${colors.primary.light}]/50
      disabled:cursor-not-allowed
    `
  };

  return (
    <button
      className={`
        ${baseStyles}
        ${variantStyles[variant]}
        ${fullWidth ? 'w-full' : ''}
        ${className}
      `}
      disabled={disabled || loading}
      {...props}
    >
      {loading ? (
        <>
          <svg
            className="animate-spin h-5 w-5"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
          >
            <circle
              className="opacity-25"
              cx="12"
              cy="12"
              r="10"
              stroke="currentColor"
              strokeWidth="4"
            />
            <path
              className="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
            />
          </svg>
          Loading...
        </>
      ) : (
        children
      )}
    </button>
  );
}; 