"use client";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { useRouter } from "next/navigation";
import { useState } from "react";

const testImage =
  "https://images.unsplash.com/photo-1512436991641-6745cdb1723f?auto=format&fit=facearea&w=400&h=400&q=80";

const categories = ["Women", "Men", "T-Shirt", "Hoodie", "Dress"];
const initialTags = ["T-Shirt", "Men Clothes", "Summer Collection"];
const sizes = ["S", "M", "L", "XL", "XXL"];
const countries = ["USA", "UK", "Germany", "France", "Egypt"];

export default function AddProductPage() {
  const router = useRouter();
  const [selectedCategories, setSelectedCategories] = useState<string[]>([]);
  const [tags, setTags] = useState<string[]>(initialTags);
  const [tagInput, setTagInput] = useState("");
  const [addTax, setAddTax] = useState(false);
  const [hasOptions, setHasOptions] = useState(true);
  const [optionValue, setOptionValue] = useState<string[]>(sizes);
  const [isDigital, setIsDigital] = useState(false);

  const handleCategoryChange = (cat: string) => {
    setSelectedCategories((prev) =>
      prev.includes(cat) ? prev.filter((c) => c !== cat) : [...prev, cat]
    );
  };

  const handleTagAdd = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" && tagInput.trim()) {
      setTags((prev) => [...prev, tagInput.trim()]);
      setTagInput("");
    }
  };

  const handleTagRemove = (tag: string) => {
    setTags((prev) => prev.filter((t) => t !== tag));
  };

  return (
    <div className="p-4 md:p-8 bg-[#F7F8FA] min-h-screen">
      {/* Breadcrumb */}
      <div className="flex items-center gap-2 mb-2 text-sm text-gray-500">
        <button
          onClick={() => router.push("/products")}
          className="hover:underline"
        >
          Products
        </button>
        <span>{">"}</span>
        <span className="text-gray-800 font-semibold">Add Product</span>
      </div>
      <div className="flex items-center gap-2 mb-6">
        <Button variant="text" className="p-0" onClick={() => router.back()}>
          &larr; Back
        </Button>
        <h1 className="text-xl font-bold">Add Product</h1>
        <div className="flex-1" />
      </div>
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left/Main Form */}
        <div className="lg:col-span-2 flex flex-col gap-6">
          {/* Information */}
          <div className="bg-white rounded-xl p-6 flex flex-col gap-4">
            <div className="font-semibold text-gray-800 mb-2">Information</div>
            <Input label="Product Name" placeholder="Summer T-Shirt" />
            <textarea
              className="border rounded-lg p-2 min-h-[60px] text-sm"
              placeholder="Product description"
            />
          </div>
          {/* Images */}
          <div className="bg-white rounded-xl p-6 flex flex-col gap-4">
            <div className="font-semibold text-gray-800 mb-2">Images</div>
            <div className="border-2 border-dashed border-gray-200 rounded-lg flex flex-col items-center justify-center p-6 min-h-[120px]">
              <img
                src={testImage}
                alt="Product"
                className="w-24 h-24 object-cover rounded mb-2"
              />
              <Button variant="outlined" className="mb-1">
                Add File
              </Button>
              <span className="text-xs text-gray-400">
                Or drag and drop files
              </span>
            </div>
          </div>
          {/* Price */}
          <div className="bg-white rounded-xl p-6 flex flex-col gap-4">
            <div className="font-semibold text-gray-800 mb-2">Price</div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input label="Product Price" placeholder="Enter price" />
              <Input label="Discount Price" placeholder="Price at Discount" />
            </div>
            <label className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                checked={addTax}
                onChange={() => setAddTax((v) => !v)}
              />
              Add tax for this product
            </label>
          </div>
          {/* Options */}
          <div className="bg-white rounded-xl p-6 flex flex-col gap-4">
            <div className="font-semibold text-gray-800 mb-2">
              Different Options
            </div>
            <label className="flex items-center gap-2 text-sm mb-2">
              <input
                type="checkbox"
                checked={hasOptions}
                onChange={() => setHasOptions((v) => !v)}
              />
              This product has multiple options
            </label>
            {hasOptions && (
              <div className="flex flex-col gap-2">
                <div className="flex gap-2 items-center">
                  <Input
                    label="Size"
                    placeholder="Size"
                    className="max-w-[120px]"
                  />
                  <div className="flex gap-1 flex-wrap">
                    {optionValue.map((size) => (
                      <span
                        key={size}
                        className="px-2 py-1 bg-gray-100 rounded text-xs font-medium"
                      >
                        {size}
                      </span>
                    ))}
                  </div>
                </div>
                <Button variant="text" className="text-blue-600 w-fit">
                  Add More
                </Button>
              </div>
            )}
          </div>
          {/* Shipping */}
          <div className="bg-white rounded-xl p-6 flex flex-col gap-4">
            <div className="font-semibold text-gray-800 mb-2">Shipping</div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input label="Weight" placeholder="Enter Weight" />
              <select className="border rounded-lg p-2 text-sm">
                <option>Select Country</option>
                {countries.map((c) => (
                  <option key={c}>{c}</option>
                ))}
              </select>
            </div>
            <label className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                checked={isDigital}
                onChange={() => setIsDigital((v) => !v)}
              />
              This is digital item
            </label>
          </div>
        </div>
        {/* Right Sidebar */}
        <div className="flex flex-col gap-6">
          {/* Categories */}
          <div className="bg-white rounded-xl p-6 flex flex-col gap-2">
            <div className="font-semibold text-gray-800 mb-2">Categories</div>
            {categories.map((cat) => (
              <label key={cat} className="flex items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  checked={selectedCategories.includes(cat)}
                  onChange={() => handleCategoryChange(cat)}
                />
                {cat}
              </label>
            ))}
            <Button variant="text" className="text-blue-600 w-fit mt-2">
              Create New
            </Button>
          </div>
          {/* Tags */}
          <div className="bg-white rounded-xl p-6 flex flex-col gap-2">
            <div className="font-semibold text-gray-800 mb-2">Tags</div>
            <Input
              placeholder="Enter tag name"
              value={tagInput}
              onChange={(e) => setTagInput(e.target.value)}
              onKeyDown={handleTagAdd}
            />
            <div className="flex flex-wrap gap-2 mt-2">
              {tags.map((tag) => (
                <span
                  key={tag}
                  className="bg-blue-100 text-blue-700 px-2 py-1 rounded text-xs flex items-center gap-1"
                >
                  {tag}
                  <button
                    onClick={() => handleTagRemove(tag)}
                    className="ml-1 text-blue-500 hover:text-blue-700"
                  >
                    ×
                  </button>
                </span>
              ))}
            </div>
          </div>
          {/* SEO Settings */}
          <div className="bg-white rounded-xl p-6 flex flex-col gap-2">
            <div className="font-semibold text-gray-800 mb-2">SEO Settings</div>
            <Input label="Title" placeholder="Title" />
            <textarea
              className="border rounded-lg p-2 min-h-[40px] text-sm"
              placeholder="Description"
            />
          </div>
        </div>
      </div>
      <div className="flex justify-end gap-2 mt-8">
        <Button variant="outlined" onClick={() => router.push("/products")}>
          Cancel
        </Button>
        <Button>Save</Button>
      </div>
    </div>
  );
}
