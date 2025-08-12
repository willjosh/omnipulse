"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { PrimaryButton } from "@/components/ui/Button";
import { EyeOpen, EyeClosed } from "@/components/ui/Icons";
import { useLogin } from "@/features/auth/hooks/useAuth";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorFields, getErrorMessage } from "@/utils/fieldErrorUtils";

export default function LoginPage() {
  const router = useRouter();
  const notify = useNotification();
  const { mutate: login, isPending } = useLogin();

  const [form, setForm] = useState({ email: "", password: "" });
  const [showPassword, setShowPassword] = useState(false);
  const [errors, setErrors] = useState<{
    email?: string;
    password?: string;
    general?: string;
  }>({});

  const validateForm = () => {
    const newErrors: typeof errors = {};

    if (!form.email.trim()) {
      newErrors.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(form.email)) {
      newErrors.email = "Please enter a valid email address";
    }

    if (!form.password.trim()) {
      newErrors.password = "Password is required";
    }

    return newErrors;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const validationErrors = validateForm();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);

      const missingFields = Object.values(validationErrors);
      const errorMessage = `Please fix the following issues:\n• ${missingFields.join("\n• ")}`;

      notify(errorMessage, "error");
      return;
    }

    setErrors({});

    login(
      { email: form.email, password: form.password },
      {
        onSuccess: () => {
          notify("Login successful!", "success");
          router.push("/");
        },
        onError: (error: any) => {
          console.error("Login failed:", error);

          const errorMessage = getErrorMessage(
            error,
            "Login failed. Please check your credentials.",
          );
          const fieldErrors = getErrorFields(error, ["email", "password"]);

          setErrors(fieldErrors);
          notify(errorMessage, "error");
        },
      },
    );
  };

  const handleChange = (field: string, value: string) => {
    setForm(prev => ({ ...prev, [field]: value }));
    if (errors[field as keyof typeof errors]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  return (
    <div className="w-full max-w-md">
      <div className="text-center mb-8">
        <h2 className="text-3xl font-extrabold text-gray-900">
          Sign in to your account
        </h2>
      </div>

      <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <form className="space-y-6" onSubmit={handleSubmit}>
          <div>
            <label
              htmlFor="email"
              className="block text-sm font-medium text-gray-700"
            >
              Email address
            </label>
            <div className="mt-1">
              <input
                id="email"
                name="email"
                type="email"
                autoComplete="email"
                required
                value={form.email}
                onChange={e => handleChange("email", e.target.value)}
                className={`appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                  errors.email ? "border-red-300" : "border-gray-300"
                }`}
                placeholder="Enter your email"
              />
            </div>
          </div>

          <div>
            <div className="flex items-center justify-between">
              <label
                htmlFor="password"
                className="block text-sm font-medium text-gray-700"
              >
                Password
              </label>
            </div>
            <div className="mt-1 relative">
              <input
                id="password"
                name="password"
                type={showPassword ? "text" : "password"}
                autoComplete="current-password"
                required
                value={form.password}
                onChange={e => handleChange("password", e.target.value)}
                className={`appearance-none block w-full px-3 py-2 pr-10 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                  errors.password ? "border-red-300" : "border-gray-300"
                }`}
                placeholder="Enter your password"
              />
              <button
                type="button"
                className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600 focus:outline-none focus:text-gray-600 transition-colors duration-200"
                onClick={() => setShowPassword(!showPassword)}
              >
                {showPassword ? <EyeClosed /> : <EyeOpen />}
              </button>
            </div>
          </div>

          <div className="pt-2">
            <PrimaryButton
              type="submit"
              className="w-full py-3 text-base font-medium"
              disabled={isPending}
            >
              {isPending ? "Signing in..." : "Sign in"}
            </PrimaryButton>
          </div>

          <div className="mt-6 pt-6 border-t border-gray-200">
            <div className="text-center">
              <p className="text-sm text-gray-600">
                Need a Fleet Manager account?{" "}
                <button
                  type="button"
                  onClick={() => router.push("/register")}
                  className="font-medium text-blue-600 hover:text-blue-500 hover:underline focus:outline-none transition-colors duration-200"
                >
                  Register here
                </button>
              </p>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
