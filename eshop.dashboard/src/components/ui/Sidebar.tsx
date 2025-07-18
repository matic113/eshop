"use client";
import React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";

const navItems = [
  { label: "Dashboard", href: "/dashboard" },
  { label: "Orders", href: "/orders" },
  { label: "Products", href: "/products" },
  { label: "Categories", href: "/categories" },
  { label: "Customers", href: "/customers" },
  { label: "Reports", href: "/reports" },
  { label: "Coupons", href: "/coupons" },
  { label: "Inbox", href: "/inbox" },
];

const otherInfo = [
  { label: "Knowledge Base", href: "#" },
  { label: "Product Updates", href: "#" },
];

const settings = [
  { label: "Personal Settings", href: "#" },
  { label: "Global Settings", href: "#" },
];

export default function Sidebar() {
  const pathname = usePathname();
  return (
    <aside className="flex flex-col h-full w-64 bg-[#181C2A] text-white p-4 justify-between">
      <div>
        <div className="flex items-center gap-2 mb-8">
          <span className="text-2xl font-bold text-yellow-400">fastcart</span>
        </div>
        <nav className="flex flex-col gap-2">
          {navItems.map((item) => {
            const isActive = pathname.startsWith(item.href);
            return (
              <Link
                key={item.label}
                href={item.href}
                className={`px-3 py-2 rounded flex items-center gap-2 transition-colors ${
                  isActive ? "bg-[#23263A] font-bold" : "hover:bg-[#23263A]"
                }`}
              >
                <span>{item.label}</span>
              </Link>
            );
          })}
        </nav>
        <div className="mt-8">
          <div className="text-xs text-gray-400 mb-2">Other Information</div>
          <nav className="flex flex-col gap-2">
            {otherInfo.map((item) => (
              <Link
                key={item.label}
                href={item.href}
                className="px-3 py-2 rounded hover:bg-[#23263A] flex items-center gap-2"
              >
                <span>{item.label}</span>
              </Link>
            ))}
          </nav>
        </div>
        <div className="mt-8">
          <div className="text-xs text-gray-400 mb-2">Settings</div>
          <nav className="flex flex-col gap-2">
            {settings.map((item) => (
              <Link
                key={item.label}
                href={item.href}
                className="px-3 py-2 rounded hover:bg-[#23263A] flex items-center gap-2"
              >
                <span>{item.label}</span>
              </Link>
            ))}
          </nav>
        </div>
      </div>
    </aside>
  );
}
