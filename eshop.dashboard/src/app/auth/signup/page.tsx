"use client";

import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { colors } from "@/constants/colors";
import { authService } from "@/services/auth";
import { SignupData } from "@/types/auth";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useState } from "react";

export default function SignupPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [data, setData] = useState<SignupData>({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    password: "",
  });

  const [confirmPassword, setConfirmPassword] = useState("");
  const [errors, setErrors] = useState<
    Partial<SignupData & { confirmPassword: string }>
  >({});

  const validateForm = () => {
    const newErrors: Partial<SignupData & { confirmPassword: string }> = {};

    if (!data.firstName.trim()) {
      newErrors.firstName = "First name is required";
    }

    if (!data.lastName.trim()) {
      newErrors.lastName = "Last name is required";
    }

    if (!data.email.trim()) {
      newErrors.email = "Email is required";
    } else if (!/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(data.email)) {
      newErrors.email = "Invalid email address";
    }

    if (!data.phone.trim()) {
      newErrors.phone = "Phone number is required";
    } else if (!/^\+?[1-9]\d{1,14}$/.test(data.phone)) {
      newErrors.phone = "Invalid phone number";
    }

    if (!data.password) {
      newErrors.password = "Password is required";
    } else if (data.password.length < 8) {
      newErrors.password = "Password must be at least 8 characters";
    }

    if (data.password !== confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");

    if (!validateForm()) {
      return;
    }

    setLoading(true);

    try {
      const response = await authService.signup(data);
      localStorage.setItem("accessToken", response.accessToken);
      localStorage.setItem("refreshToken", response.refreshToken);
      router.push("/auth/verify");
    } catch (error) {
      setError(error instanceof Error ? error.message : "Failed to sign up");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 bg-white p-8 rounded-xl shadow-lg">
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900">
            Create an account
          </h2>
          <p className="mt-2 text-sm text-gray-600">
            Already have an account?{" "}
            <Link
              href="/auth/login"
              className="text-blue-600 hover:text-blue-500"
            >
              Sign in
            </Link>
          </p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-4">
            <Input
              label="First name"
              required
              placeholder="Enter your first name"
              value={data.firstName}
              onChange={(e) => setData({ ...data, firstName: e.target.value })}
              error={errors.firstName}
            />

            <Input
              label="Last name"
              required
              placeholder="Enter your last name"
              value={data.lastName}
              onChange={(e) => setData({ ...data, lastName: e.target.value })}
              error={errors.lastName}
            />
          </div>

          <Input
            label="Email address"
            type="email"
            required
            placeholder="Enter your email address"
            value={data.email}
            onChange={(e) => setData({ ...data, email: e.target.value })}
            error={errors.email}
          />

          <Input
            label="Phone number"
            type="tel"
            required
            placeholder="Enter your phone number (e.g., +1234567890)"
            value={data.phone}
            onChange={(e) => setData({ ...data, phone: e.target.value })}
            error={errors.phone}
          />

          <Input
            label="Password"
            type="password"
            required
            placeholder="Create a strong password (min. 8 characters)"
            value={data.password}
            onChange={(e) => setData({ ...data, password: e.target.value })}
            error={errors.password}
          />

          <Input
            label="Confirm password"
            type="password"
            required
            placeholder="Re-enter your password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            error={errors.confirmPassword}
          />

          {error && (
            <div className="text-sm text-red-500 text-center">{error}</div>
          )}

          <Button
            type="submit"
            fullWidth
            loading={loading}
            style={{
              backgroundColor: colors.primary.main,
              color: colors.primary.contrastText,
            }}
          >
            Create account
          </Button>
        </form>
      </div>
    </div>
  );
}
