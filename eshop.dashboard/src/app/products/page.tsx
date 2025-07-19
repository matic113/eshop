"use client";

import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import Image from "next/image";
import { useState } from "react";
import { useRouter } from "next/navigation";

const products = [
  {
    id: 1,
    name: "Men Grey Hoodie",
    category: "Hoodies",
    image:
      "https://images.unsplash.com/photo-1512436991641-6745cdb1723f?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 96,
    color: "Black",
    price: 49.9,
    rating: 5.0,
    votes: 32,
    inStock: true,
  },
  {
    id: 2,
    name: "Women Striped T-Shirt",
    category: "T-Shirt",
    image:
      "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 56,
    color: "White",
    price: 34.9,
    rating: 4.8,
    votes: 24,
    inStock: true,
  },
  {
    id: 3,
    name: "Women White T-Shirt",
    category: "T-Shirt",
    image:
      "https://images.unsplash.com/photo-1515378791036-0648a3ef77b2?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 78,
    color: "White",
    price: 40.9,
    rating: 5.0,
    votes: 54,
    inStock: true,
  },
  {
    id: 4,
    name: "Men White T-Shirt",
    category: "T-Shirt",
    image:
      "https://images.unsplash.com/photo-1503342217505-b0a15ec3261c?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 32,
    color: "White",
    price: 49.9,
    rating: 4.5,
    votes: 31,
    inStock: true,
  },
  {
    id: 5,
    name: "Women Red T-Shirt",
    category: "T-Shirt",
    image:
      "https://images.unsplash.com/photo-1519125323398-675f0ddb6308?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 32,
    color: "White",
    price: 34.9,
    rating: 4.9,
    votes: 22,
    inStock: true,
  },
  // Out of stock examples
  {
    id: 6,
    name: "Men Grey Hoodie",
    category: "Hoodies",
    image:
      "https://images.unsplash.com/photo-1512436991641-6745cdb1723f?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 96,
    color: "Black",
    price: 49.9,
    rating: 5.0,
    votes: 32,
    inStock: false,
  },
  {
    id: 7,
    name: "Women Striped T-Shirt",
    category: "T-Shirt",
    image:
      "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 56,
    color: "White",
    price: 34.9,
    rating: 4.8,
    votes: 24,
    inStock: false,
  },
  {
    id: 8,
    name: "Women White T-Shirt",
    category: "T-Shirt",
    image:
      "https://images.unsplash.com/photo-1515378791036-0648a3ef77b2?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 78,
    color: "White",
    price: 40.9,
    rating: 5.0,
    votes: 54,
    inStock: false,
  },
  {
    id: 9,
    name: "Men White T-Shirt",
    category: "T-Shirt",
    image:
      "https://images.unsplash.com/photo-1503342217505-b0a15ec3261c?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 32,
    color: "White",
    price: 49.9,
    rating: 4.5,
    votes: 31,
    inStock: false,
  },
  {
    id: 10,
    name: "Women Red T-Shirt",
    category: "T-Shirt",
    image:
      "https://images.unsplash.com/photo-1519125323398-675f0ddb6308?auto=format&fit=facearea&w=400&h=400&q=80",
    inventory: 32,
    color: "White",
    price: 34.9,
    rating: 4.9,
    votes: 22,
    inStock: false,
  },
];

const categories = ["All", "Hoodies", "T-Shirt"];

export default function ProductsPage() {
  const [selected, setSelected] = useState<number[]>([1, 2, 4, 5]);
  const [search, setSearch] = useState("");
  const [filterCategory, setFilterCategory] = useState("All");
  const router = useRouter();

  const toggleSelect = (id: number) => {
    setSelected((prev) =>
      prev.includes(id) ? prev.filter((sid) => sid !== id) : [...prev, id]
    );
  };

  const allSelected = selected.length === products.length;
  const toggleSelectAll = () => {
    setSelected(allSelected ? [] : products.map((p) => p.id));
  };

  // Filtered products
  const filteredProducts = products.filter((product) => {
    const matchesSearch = product.name
      .toLowerCase()
      .includes(search.toLowerCase());
    const matchesCategory =
      filterCategory === "All" || product.category === filterCategory;
    return matchesSearch && matchesCategory;
  });

  return (
    <div className="w-full max-w-full mx-auto">
      <div className="bg-white rounded-xl shadow p-4 overflow-x-auto">
        {/* Filter/Search bar and table only, no Products title/toolbar here */}
        <div className="flex flex-col md:flex-row md:items-center gap-2 mb-4">
          <select
            className="border rounded-lg px-3 py-2 text-sm bg-white"
            value={filterCategory}
            onChange={(e) => setFilterCategory(e.target.value)}
          >
            {categories.map((cat) => (
              <option key={cat} value={cat}>
                {cat}
              </option>
            ))}
          </select>
          <Input
            placeholder="Search..."
            className="max-w-xs"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead>
              <tr className="border-b border-gray-200 text-gray-500">
                <th className="px-2 py-2 text-left">
                  <input
                    type="checkbox"
                    checked={allSelected}
                    onChange={toggleSelectAll}
                  />
                </th>
                <th className="px-2 py-2 text-left">Product</th>
                <th className="px-2 py-2 text-left">Inventory</th>
                <th className="px-2 py-2 text-left">Color</th>
                <th className="px-2 py-2 text-left">Price</th>
                <th className="px-2 py-2 text-left">Rating</th>
              </tr>
            </thead>
            <tbody>
              {filteredProducts.map((product) => (
                <tr
                  key={product.id}
                  className="border-b border-gray-100 hover:bg-gray-50 transition-colors"
                >
                  <td className="px-2 py-2">
                    <input
                      type="checkbox"
                      checked={selected.includes(product.id)}
                      onChange={() => toggleSelect(product.id)}
                    />
                  </td>
                  <td className="px-2 py-2">
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 rounded overflow-hidden bg-gray-100 flex-shrink-0">
                        <Image
                          src={product.image}
                          alt={product.name}
                          width={40}
                          height={40}
                          className="object-cover w-full h-full"
                        />
                      </div>
                      <div>
                        <div className="font-medium text-gray-900">
                          {product.name}
                        </div>
                        <div className="text-xs text-gray-500">
                          {product.category}
                        </div>
                      </div>
                    </div>
                  </td>
                  <td className="px-2 py-2">
                    {product.inStock ? (
                      <span>{product.inventory} in stock</span>
                    ) : (
                      <span className="bg-gray-100 text-gray-500 text-xs px-2 py-1 rounded">
                        Out of Stock
                      </span>
                    )}
                  </td>
                  <td className="px-2 py-2">{product.color}</td>
                  <td className="px-2 py-2">${product.price.toFixed(2)}</td>
                  <td className="px-2 py-2">
                    <span className="font-semibold text-gray-800">
                      {product.rating.toFixed(1)}
                    </span>
                    <span className="text-xs text-gray-500 ml-1">
                      ({product.votes} Votes)
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        {/* Pagination */}
        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-2 mt-4">
          <div className="flex items-center gap-2">
            <Button variant="text" className="p-1">
              &larr;
            </Button>
            <Button
              variant="text"
              className="p-1 bg-blue-100 text-blue-700 font-bold"
            >
              1
            </Button>
            <Button variant="text" className="p-1">
              2
            </Button>
            <Button variant="text" className="p-1">
              3
            </Button>
            <span className="px-2">...</span>
            <Button variant="text" className="p-1">
              24
            </Button>
            <Button variant="text" className="p-1">
              &rarr;
            </Button>
          </div>
          <div className="text-xs text-gray-500 text-right">
            {filteredProducts.length} Results
          </div>
        </div>
      </div>
    </div>
  );
}
