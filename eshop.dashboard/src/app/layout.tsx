// import { GoogleOAuthProvider } from '@react-oauth/google';
import type { Metadata } from 'next';
import { Inter } from 'next/font/google';
import './globals.css';

const inter = Inter({ subsets: ['latin'] });

export const metadata: Metadata = {
  title: 'eShop Dashboard',
  description: 'Admin dashboard for eShop platform',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        {/* <GoogleOAuthProvider clientId={process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID || ''}> */}
          {children}
        {/* </GoogleOAuthProvider> */}
      </body>
    </html>
  );
}
