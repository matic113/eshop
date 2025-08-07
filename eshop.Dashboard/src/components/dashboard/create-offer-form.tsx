"use client";

import { useState, useRef } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Upload, X, Smartphone, Save } from "lucide-react";
import { useImageUpload } from "@/hooks/use-api";
import { offersApi, CreateOfferRequest } from "@/lib/api";

interface OfferFormData {
  name: string;
  description: string;
  coverImage: File | null;
  coverImagePreview: string | null;
}

export function CreateOfferForm() {
  const [formData, setFormData] = useState<OfferFormData>({
    name: "",
    description: "",
    coverImage: null,
    coverImagePreview: null,
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showSuccess, setShowSuccess] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const imageUploadMutation = useImageUpload();

  const handleInputChange =
    (field: keyof Omit<OfferFormData, "coverImage" | "coverImagePreview">) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      const value = e.target.value;
      setFormData((prev) => ({ ...prev, [field]: value }));

      // Clear error for this field when user starts typing
      if (errors[field]) {
        setErrors((prev) => ({ ...prev, [field]: "" }));
      }
    };

  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Validate file type
    if (!file.type.startsWith("image/")) {
      setErrors((prev) => ({
        ...prev,
        coverImage: "Please select a valid image file",
      }));
      return;
    }

    // Validate file size (max 5MB)
    if (file.size > 5 * 1024 * 1024) {
      setErrors((prev) => ({
        ...prev,
        coverImage: "Image size must be less than 5MB",
      }));
      return;
    }

    // Create preview URL
    const previewUrl = URL.createObjectURL(file);
    setFormData((prev) => ({
      ...prev,
      coverImage: file,
      coverImagePreview: previewUrl,
    }));

    // Clear error
    if (errors.coverImage) {
      setErrors((prev) => ({ ...prev, coverImage: "" }));
    }
  };

  const removeImage = () => {
    if (formData.coverImagePreview) {
      URL.revokeObjectURL(formData.coverImagePreview);
    }
    setFormData((prev) => ({
      ...prev,
      coverImage: null,
      coverImagePreview: null,
    }));
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = "Offer name is required";
    }
    if (!formData.description.trim()) {
      newErrors.description = "Description is required";
    }
    if (!formData.coverImage) {
      newErrors.coverImage = "Cover image is required";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);
    setErrors({});

    try {
      // Upload image first
      if (!formData.coverImage) {
        throw new Error("Cover image is required");
      }

      const coverUrl = await imageUploadMutation.mutateAsync(
        formData.coverImage
      );

      // Create offer data
      const offerData: CreateOfferRequest = {
        name: formData.name.trim(),
        description: formData.description.trim(),
        offerCoverUrl: coverUrl,
      };

      console.log("Sending offer data:", offerData);

      // Send to API using the offersApi
      const result = await offersApi.create(offerData);
      console.log("API Success:", result);

      // Show success message
      setShowSuccess(true);
      setTimeout(() => setShowSuccess(false), 3000);

      // Reset form
      setFormData({
        name: "",
        description: "",
        coverImage: null,
        coverImagePreview: null,
      });

      // Clean up preview URL
      if (formData.coverImagePreview) {
        URL.revokeObjectURL(formData.coverImagePreview);
      }
    } catch (error) {
      console.error("Error saving offer:", error);
      setErrors({
        general:
          error instanceof Error
            ? error.message
            : "Failed to save offer. Please try again.",
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
      {/* Form Section */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Upload className="h-5 w-5" />
            Create Offer Card
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          {/* Name Input */}
          <div className="space-y-2">
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              value={formData.name}
              onChange={handleInputChange("name")}
              className={errors.name ? "border-red-500" : ""}
              placeholder="Enter offer name"
            />
            {errors.name && (
              <p className="text-sm text-red-500">{errors.name}</p>
            )}
          </div>

          {/* Description Input */}
          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              value={formData.description}
              onChange={handleInputChange("description")}
              className={errors.description ? "border-red-500" : ""}
              placeholder="Enter offer description"
              rows={4}
            />
            {errors.description && (
              <p className="text-sm text-red-500">{errors.description}</p>
            )}
          </div>

          {/* Cover Image Upload */}
          <div className="space-y-2">
            <Label>Cover Image</Label>
            {formData.coverImagePreview ? (
              <div className="relative inline-block">
                <img
                  src={formData.coverImagePreview}
                  alt="Cover preview"
                  className="w-full h-48 object-cover rounded-lg border"
                />
                <Button
                  type="button"
                  variant="destructive"
                  size="icon"
                  className="absolute top-2 right-2 h-8 w-8"
                  onClick={removeImage}
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            ) : (
              <div className="border-2 border-dashed border-gray-300 rounded-lg p-8 text-center hover:border-gray-400 transition-colors">
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*"
                  onChange={handleImageUpload}
                  className="hidden"
                  id="cover-upload"
                />
                <label htmlFor="cover-upload" className="cursor-pointer">
                  <Upload className="h-12 w-12 mx-auto mb-4 text-gray-400" />
                  <p className="text-sm text-gray-600 mb-2">
                    Click to upload cover image
                  </p>
                  <p className="text-xs text-gray-500">
                    PNG, JPG, GIF up to 5MB
                  </p>
                </label>
              </div>
            )}
            {errors.coverImage && (
              <p className="text-sm text-red-500">{errors.coverImage}</p>
            )}
          </div>

          {/* Action Buttons */}
          <div className="flex gap-3 pt-4">
            <Button
              type="button"
              onClick={handleSave}
              disabled={isSubmitting}
              className="w-full"
            >
              <Save className="h-4 w-4 mr-2" />
              {isSubmitting ? "Saving..." : "Save"}
            </Button>
          </div>

          {/* Success Message */}
          {showSuccess && (
            <div className="text-sm text-green-600 text-center bg-green-50 p-3 rounded-lg border border-green-200">
              ✅ Offer card added successfully!
            </div>
          )}

          {/* Error Messages */}
          {errors.general && (
            <div className="text-sm text-red-500 text-center bg-red-50 p-3 rounded-lg">
              {errors.general}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Mobile Preview Section */}
      <div className="flex flex-col items-center">
        <Card className="w-full max-w-sm">
          <CardHeader className="text-center pb-4">
            <CardTitle className="flex items-center justify-center gap-2 text-lg">
              <Smartphone className="h-5 w-5" />
              Mobile Preview
            </CardTitle>
          </CardHeader>
          <CardContent className="p-0 pb-8">
            {/* Mobile Frame */}
            <div className="relative mx-auto w-64 h-[500px] bg-gray-900 rounded-[2rem] p-2 shadow-2xl">
              {/* Screen */}
              <div className="w-full h-full bg-white rounded-[1.5rem] overflow-hidden relative">
                {/* Status Bar */}
                <div className="h-6 bg-black text-white text-xs flex items-center justify-between px-4">
                  <span>9:41</span>
                  <div className="flex items-center gap-1">
                    <div className="w-4 h-2 bg-white rounded-sm"></div>
                    <div className="w-1 h-1 bg-white rounded-full"></div>
                    <div className="w-1 h-1 bg-white rounded-full"></div>
                    <div className="w-1 h-1 bg-white rounded-full"></div>
                  </div>
                </div>

                {/* App Content */}
                <div className="h-full bg-gray-50">
                  {/* Header Section */}
                  <div className="p-4 pb-2">
                    <div className="flex items-center justify-between mb-3">
                      <div>
                        <p className="text-xs text-gray-600">Good morning...</p>
                        <p className="text-sm font-semibold text-gray-900">
                          Yousef Mahmoud
                        </p>
                      </div>
                      <div className="w-5 h-5 text-gray-600">
                        <svg fill="currentColor" viewBox="0 0 24 24">
                          <path d="M12 22c1.1 0 2-.9 2-2h-4c0 1.1.9 2 2 2zm6-6v-5c0-3.07-1.63-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.64 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2zm-2 1H8v-6c0-2.48 1.51-4.5 4-4.5s4 2.02 4 4.5v6z" />
                        </svg>
                      </div>
                    </div>
                  </div>

                  {/* Main Offer Banner */}
                  <div className="px-4 pb-4">
                    <div className="relative overflow-hidden rounded-xl shadow-lg">
                      {/* Cover Image Background */}
                      {formData.coverImagePreview ? (
                        <div className="relative h-20">
                          <img
                            src={formData.coverImagePreview}
                            alt="Offer cover"
                            className="w-full h-full object-cover"
                          />
                        </div>
                      ) : (
                        <div className="h-20 bg-gray-200 flex items-center justify-center">
                          <div className="text-center">
                            <div className="w-8 h-8 mx-auto mb-1 bg-gray-300 rounded-lg flex items-center justify-center">
                              <Upload className="h-4 w-4 text-gray-500" />
                            </div>
                            <p className="text-xs text-gray-500">No image</p>
                          </div>
                        </div>
                      )}

                      {/* Content */}
                      <div className="relative p-3">
                        <div className="text-black">
                          <h3 className="text-sm font-bold mb-1">
                            {formData.name || "Special Offer"}
                          </h3>
                          <p className="text-xs opacity-90 line-clamp-2">
                            {formData.description || "Limited Time Only"}
                          </p>
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Product Sections */}
                  <div className="px-4">
                    <h4 className="text-xs font-semibold text-gray-900 mb-3">
                      Best Prices
                    </h4>
                    <div className="grid grid-cols-2 gap-3">
                      {/* Product Card 1 */}
                      <div className="bg-white rounded-lg p-2 shadow-sm">
                        <div className="w-full h-16 bg-gray-200 rounded mb-2 flex items-center justify-center">
                          <div className="w-6 h-6 bg-gray-300 rounded"></div>
                        </div>
                        <p className="text-xs font-medium text-gray-900 mb-1">
                          Fares
                        </p>
                        <p className="text-xs text-gray-600">100.0 EGP</p>
                      </div>

                      {/* Product Card 2 */}
                      <div className="bg-white rounded-lg p-2 shadow-sm">
                        <div className="w-full h-16 bg-gray-200 rounded mb-2 flex items-center justify-center">
                          <div className="w-6 h-6 bg-gray-300 rounded"></div>
                        </div>
                        <p className="text-xs font-medium text-gray-900 mb-1">
                          Test Product
                        </p>
                        <p className="text-xs text-gray-600">100.0 EGP</p>
                      </div>
                    </div>
                  </div>

                  {/* Bottom Navigation Bar */}
                  <div className="absolute bottom-0 left-0 right-0 bg-white border-t border-gray-200">
                    <div className="flex items-center justify-around py-2">
                      <div className="w-6 h-6 text-gray-400">
                        <svg fill="currentColor" viewBox="0 0 24 24">
                          <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
                        </svg>
                      </div>
                      <div className="w-6 h-6 text-gray-400">
                        <svg fill="currentColor" viewBox="0 0 24 24">
                          <path d="M7 18c-1.1 0-1.99.9-1.99 2S5.9 22 7 22s2-.9 2-2-.9-2-2-2zM1 2v2h2l3.6 7.59-1.35 2.45c-.16.28-.25.61-.25.96 0 1.1.9 2 2 2h12v-2H7.42c-.14 0-.25-.11-.25-.25l.03-.12L8.1 13h7.45c.75 0 1.41-.41 1.75-1.03L21.7 4H5.21l-.94-2H1zm16 16c-1.1 0-1.99.9-1.99 2s.89 2 1.99 2 2-.9 2-2-.9-2-2-2z" />
                        </svg>
                      </div>
                      <div className="w-6 h-6 text-gray-400">
                        <svg fill="currentColor" viewBox="0 0 24 24">
                          <path d="M3 13h2v-2H3v2zm0 4h2v-2H3v2zm0-8h2V7H3v2zm4 4h14v-2H7v2zm0 4h14v-2H7v2zM7 7v2h14V7H7z" />
                        </svg>
                      </div>
                      <div className="w-6 h-6 bg-green-500 text-white rounded-full flex items-center justify-center">
                        <svg fill="currentColor" viewBox="0 0 24 24">
                          <path d="M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z" />
                        </svg>
                      </div>
                      <div className="w-6 h-6 text-gray-400">
                        <svg fill="currentColor" viewBox="0 0 24 24">
                          <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z" />
                        </svg>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Preview Instructions */}
        <div className="mt-4 text-center text-sm text-muted-foreground max-w-sm">
          <p>
            This is the real app interface. The offer card will appear in the
            main banner with the image and text you enter.
          </p>
        </div>
      </div>
    </div>
  );
}
