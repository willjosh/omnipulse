"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { EyeOpen, EyeClosed } from "@/components/ui/Icons";
import { useRegister } from "@/features/auth/hooks/useAuth";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorFields, getErrorMessage } from "@/utils/fieldErrorUtils";

export default function RegisterPage() {
  const router = useRouter();
  const notify = useNotification();
  const { mutate: register, isPending } = useRegister();

  const [form, setForm] = useState({
    email: "",
    password: "",
    confirmPassword: "",
    firstName: "",
    lastName: "",
    hireDate: "",
    isActive: true,
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [errors, setErrors] = useState<{
    email?: string;
    password?: string;
    confirmPassword?: string;
    firstName?: string;
    lastName?: string;
    hireDate?: string;
    general?: string;
  }>({});

  const validateForm = () => {
    const newErrors: typeof errors = {};

    // Required text fields
    if (!form.firstName.trim()) {
      newErrors.firstName = "First Name is required";
    }
    if (!form.lastName.trim()) {
      newErrors.lastName = "Last Name is required";
    }
    if (!form.email.trim()) {
      newErrors.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(form.email)) {
      newErrors.email = "Please enter a valid email address";
    }
    if (!form.hireDate) {
      newErrors.hireDate = "Hire Date is required";
    }

    // Password validation
    if (!form.password.trim()) {
      newErrors.password = "Password is required";
    } else if (form.password.length < 6) {
      newErrors.password = "Password must be at least 6 characters long";
    }

    // Confirm password validation
    if (!form.confirmPassword.trim()) {
      newErrors.confirmPassword = "Please confirm your password";
    } else if (form.password !== form.confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }

    return newErrors;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    // Validate form before attempting to register
    const validationErrors = validateForm();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);

      // Create a summary of missing fields
      const missingFields = Object.values(validationErrors);
      const errorMessage = `Please fix the following issues:\n• ${missingFields.join("\n• ")}`;

      notify(errorMessage, "error");
      return;
    }

    // Clear any existing errors
    setErrors({});

    register(
      {
        email: form.email,
        password: form.password,
        firstName: form.firstName,
        lastName: form.lastName,
        hireDate: form.hireDate,
        isActive: form.isActive,
      },
      {
        onSuccess: (userId: string) => {
          notify(
            `Registration successful! User ID: ${userId}. You can now log in.`,
            "success",
          );
          router.push("/login");
        },
        onError: (error: any) => {
          console.log("error", error);
          const errorMessage = getErrorMessage(
            error,
            "Registration failed. Please try again.",
          );

          const fieldErrors = getErrorFields(error, [
            "firstName",
            "lastName",
            "email",
            "hireDate",
            "password",
            "confirmPassword",
          ]);

          setErrors(fieldErrors);
          notify(errorMessage, "error");
        },
      },
    );
  };

  const handleChange = (field: string, value: any) => {
    setForm(prev => ({ ...prev, [field]: value }));
    if (errors[field as keyof typeof errors]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  return (
    <div className="w-full max-w-md">
      <div className="text-center mb-8">
        <h2 className="text-3xl font-extrabold text-gray-900">
          Register Fleet Manager Account
        </h2>
        <p className="mt-2 text-sm text-gray-600">
          Create a new Fleet Manager account to manage your fleet operations
        </p>
      </div>

      <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <form className="space-y-6" onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label
                htmlFor="firstName"
                className="block text-sm font-medium text-gray-700"
              >
                First Name
              </label>
              <div className="mt-1">
                <input
                  id="firstName"
                  name="firstName"
                  type="text"
                  required
                  value={form.firstName}
                  onChange={e => handleChange("firstName", e.target.value)}
                  className={`appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                    errors.firstName ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="First name"
                />
              </div>
            </div>

            <div>
              <label
                htmlFor="lastName"
                className="block text-sm font-medium text-gray-700"
              >
                Last Name
              </label>
              <div className="mt-1">
                <input
                  id="lastName"
                  name="lastName"
                  type="text"
                  required
                  value={form.lastName}
                  onChange={e => handleChange("lastName", e.target.value)}
                  className={`appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                    errors.lastName ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Last name"
                />
              </div>
            </div>
          </div>

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
            <label
              htmlFor="hireDate"
              className="block text-sm font-medium text-gray-700"
            >
              Hire Date
            </label>
            <div className="mt-1">
              <input
                id="hireDate"
                name="hireDate"
                type="date"
                required
                value={form.hireDate}
                onChange={e => handleChange("hireDate", e.target.value)}
                className={`appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                  errors.hireDate ? "border-red-300" : "border-gray-300"
                }`}
              />
            </div>
          </div>

          <div>
            <label
              htmlFor="password"
              className="block text-sm font-medium text-gray-700"
            >
              Password
            </label>
            <div className="mt-1 relative">
              <input
                id="password"
                name="password"
                type={showPassword ? "text" : "password"}
                autoComplete="new-password"
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

          <div>
            <label
              htmlFor="confirmPassword"
              className="block text-sm font-medium text-gray-700"
            >
              Confirm Password
            </label>
            <div className="mt-1 relative">
              <input
                id="confirmPassword"
                name="confirmPassword"
                type={showConfirmPassword ? "text" : "password"}
                autoComplete="new-password"
                required
                value={form.confirmPassword}
                onChange={e => handleChange("confirmPassword", e.target.value)}
                className={`appearance-none block w-full px-3 py-2 pr-10 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                  errors.confirmPassword ? "border-red-300" : "border-gray-300"
                }`}
                placeholder="Confirm your password"
              />
              <button
                type="button"
                className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600 focus:outline-none focus:text-gray-600 transition-colors duration-200"
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
              >
                {showConfirmPassword ? <EyeClosed /> : <EyeOpen />}
              </button>
            </div>
          </div>

          <div className="flex items-center">
            <input
              id="isActive"
              name="isActive"
              type="checkbox"
              checked={form.isActive}
              onChange={e => handleChange("isActive", e.target.checked)}
              className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            />
            <label
              htmlFor="isActive"
              className="ml-2 block text-sm text-gray-900"
            >
              Account is active
            </label>
          </div>

          <div className="pt-2">
            <PrimaryButton
              type="submit"
              className="w-full py-3 text-base font-medium"
              disabled={isPending}
            >
              {isPending
                ? "Creating account..."
                : "Create Fleet Manager Account"}
            </PrimaryButton>
          </div>

          <div className="mt-6 pt-6 border-t border-gray-200">
            <div className="text-center">
              <p className="text-sm text-gray-600">
                Already have an account?{" "}
                <button
                  type="button"
                  onClick={() => router.push("/login")}
                  className="font-medium text-blue-600 hover:text-blue-500 hover:underline focus:outline-none transition-colors duration-200"
                >
                  Sign in
                </button>
              </p>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
