"use client";

import React from "react";
import { usePathname, useRouter } from "next/navigation";

const appBarConfig = [
  { path: "/dashboard", title: "Dashboard", actions: null },
  { path: "/orders", title: "Orders", actions: null },
  { path: "/products", title: "Products", actions: "products" },
  { path: "/categories", title: "Categories", actions: null },
  { path: "/customers", title: "Customers", actions: null },
  { path: "/reports", title: "Reports", actions: null },
  { path: "/coupons", title: "Coupons", actions: null },
  { path: "/inbox", title: "Inbox", actions: null },
];

function getAppBarData(pathname: string) {
  const found = appBarConfig.find((item) => pathname.startsWith(item.path));
  return found || { title: "", actions: null };
}

export default function AppBar() {
  const pathname = usePathname();
  const router = useRouter();
  const { title, actions } = getAppBarData(pathname);

  return (
    <header className="flex items-center justify-between h-16 px-8 bg-white border-b border-gray-200">
      <div className="text-xl font-semibold">{title}</div>
      <div className="flex items-center gap-4">
        <input
          type="text"
          placeholder="Search..."
          className="px-3 py-1.5 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        {actions === "products" && (
          <>
            <button className="bg-blue-50 border border-blue-500 text-blue-600 px-4 py-1.5 rounded font-medium hover:bg-blue-100">
              Export
            </button>
            <button
              className="bg-blue-600 text-white px-4 py-1.5 rounded font-medium hover:bg-blue-700"
              onClick={() => router.push("/products/add")}
            >
              + Add Product
            </button>
          </>
        )}
        <div className="relative">
          <span className="absolute -top-1 -right-1 bg-blue-600 text-white text-xs rounded-full px-1">
            6
          </span>
          <svg
            width="24"
            height="24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            className="text-gray-500"
          >
            <circle cx="12" cy="12" r="10" />
            <path d="M12 8v4l3 3" />
          </svg>
        </div>
        <div className="w-8 h-8 rounded-full bg-green-600 flex items-center justify-center text-white font-bold">
          R
        </div>
        <span className="text-gray-700 font-medium">Randhir kumar</span>
      </div>
    </header>
  );
}
