// import { GoogleOAuthProvider } from '@react-oauth/google';
import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import Sidebar from "../components/ui/Sidebar";
import AppBar from "../components/ui/AppBar";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "eShop Dashboard",
  description: "Admin dashboard for eShop platform",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <div className="flex h-screen">
          <Sidebar />
          <div className="flex-1 flex flex-col bg-gray-50">
            <AppBar />
            <main className="flex-1 p-8 overflow-auto">{children}</main>
          </div>
        </div>
      </body>
    </html>
  );
}
