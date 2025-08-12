"use client";
import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { FormField } from "@/components/ui/Form";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import {
  useCreateTechnician,
  useUpdateTechnician,
  useTechnician,
} from "../hooks/useTechnicians";
import {
  CreateTechnicianCommand,
  UpdateTechnicianCommand,
} from "../types/technicianType";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

interface TechnicianFormProps {
  mode: "create" | "edit";
  technicianId?: string;
}

const TechnicianForm: React.FC<TechnicianFormProps> = ({
  mode,
  technicianId,
}) => {
  const router = useRouter();
  const notify = useNotification();
  const createTechnicianMutation = useCreateTechnician();
  const updateTechnicianMutation = useUpdateTechnician();

  const { technician: existingTechnician, isPending: isLoadingTechnician } =
    useTechnician(technicianId || "");

  const [formData, setFormData] = useState<CreateTechnicianCommand>({
    email: "",
    password: "",
    firstName: "",
    lastName: "",
    hireDate: new Date().toISOString().split("T")[0],
    isActive: true,
  });

  const [errors, setErrors] = useState<Partial<CreateTechnicianCommand>>({});

  useEffect(() => {
    setErrors({});
    if (mode === "edit" && existingTechnician) {
      setFormData({
        email: existingTechnician.email,
        password: "",
        firstName: existingTechnician.firstName,
        lastName: existingTechnician.lastName,
        hireDate: existingTechnician.hireDate.split("T")[0],
        isActive: existingTechnician.isActive,
      });
    }
  }, [mode, existingTechnician]);

  const handleInputChange = (
    field: keyof CreateTechnicianCommand,
    value: string | boolean,
  ) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.firstName.trim()) {
      newErrors.firstName = "First Name is required";
    }
    if (!formData.lastName.trim()) {
      newErrors.lastName = "Last Name is required";
    }

    if (mode === "create") {
      if (!formData.email.trim()) {
        newErrors.email = "Email is required";
      } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
        newErrors.email = "Please enter a valid email address";
      }
    }

    if (!formData.hireDate) {
      newErrors.hireDate = "Hire Date is required";
    }

    if (mode === "create") {
      if (!formData.password.trim()) {
        newErrors.password = "Password is required";
      } else if (formData.password.length < 6) {
        newErrors.password = "Password must be at least 6 characters long";
      }
    }

    return newErrors;
  };

  const handleSubmit = async (e: React.FormEvent) => {
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

    try {
      if (mode === "create") {
        await createTechnicianMutation.mutateAsync(formData);
        notify("Technician created successfully!", "success");
      } else {
        const updateData: UpdateTechnicianCommand = {
          id: technicianId!,
          firstName: formData.firstName,
          lastName: formData.lastName,
          hireDate: formData.hireDate,
          isActive: formData.isActive,
        };

        await updateTechnicianMutation.mutateAsync(updateData);
        notify("Technician updated successfully!", "success");
      }

      router.push("/technician");
    } catch (error: any) {
      let errorMessage =
        mode === "create"
          ? getErrorMessage(
              error,
              "Failed to create technician. Please check your input and try again.",
            )
          : getErrorMessage(
              error,
              "Failed to update technician. Please check your input and try again.",
            );

      const fieldErrors = getErrorFields(error, [
        "firstName",
        "lastName",
        "email",
        "hireDate",
        "password",
      ]);
      setErrors(fieldErrors);
      notify(errorMessage, "error");
    }
  };

  const handleCancel = () => {
    setErrors({});
    router.push("/technician");
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-2 gap-4">
        <FormField label="First Name" htmlFor="firstName" required>
          <input
            id="firstName"
            type="text"
            value={formData.firstName}
            onChange={e => handleInputChange("firstName", e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
              errors.firstName ? "border-red-300" : "border-gray-300"
            }`}
            placeholder="Enter first name"
          />
        </FormField>

        <FormField label="Last Name" htmlFor="lastName" required>
          <input
            id="lastName"
            type="text"
            value={formData.lastName}
            onChange={e => handleInputChange("lastName", e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
              errors.lastName ? "border-red-300" : "border-gray-300"
            }`}
            placeholder="Enter last name"
          />
        </FormField>
      </div>

      {mode === "create" && (
        <FormField label="Email Address" htmlFor="email" required>
          <input
            id="email"
            type="email"
            value={formData.email}
            onChange={e => handleInputChange("email", e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
              errors.email ? "border-red-300" : "border-gray-300"
            }`}
            placeholder="Enter email address"
          />
        </FormField>
      )}

      {mode === "edit" && (
        <FormField label="Email Address" htmlFor="email">
          <div className="w-full px-3 py-2 border border-gray-200 rounded-md bg-gray-50">
            <span className="text-gray-600">{formData.email}</span>
          </div>
          <p className="text-xs text-gray-500 mt-1">
            Email cannot be changed after account creation
          </p>
        </FormField>
      )}

      {mode === "create" && (
        <FormField label="Password" htmlFor="password" required>
          <input
            id="password"
            type="password"
            value={formData.password}
            onChange={e => handleInputChange("password", e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
              errors.password ? "border-red-300" : "border-gray-300"
            }`}
            placeholder="Enter password (minimum 6 characters)"
          />
        </FormField>
      )}

      <FormField label="Hire Date" htmlFor="hireDate" required>
        <input
          id="hireDate"
          type="date"
          value={formData.hireDate}
          onChange={e => handleInputChange("hireDate", e.target.value)}
          className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
            errors.hireDate ? "border-red-300" : "border-gray-300"
          }`}
        />
      </FormField>

      {mode === "edit" && (
        <FormField label="Status" htmlFor="isActive">
          <div className="flex items-center space-x-3">
            <label className="flex items-center">
              <input
                type="radio"
                name="status"
                checked={formData.isActive === true}
                onChange={() => handleInputChange("isActive", true)}
                className="mr-2 text-blue-600"
              />
              <span className="text-sm text-gray-700">Active</span>
            </label>
            <label className="flex items-center">
              <input
                type="radio"
                name="status"
                checked={formData.isActive === false}
                onChange={() => handleInputChange("isActive", false)}
                className="mr-2 text-blue-600"
              />
              <span className="text-sm text-gray-700">Inactive</span>
            </label>
          </div>
        </FormField>
      )}

      <div className="flex justify-end space-x-3 pt-6 border-t border-gray-200">
        <SecondaryButton onClick={handleCancel} type="button">
          Cancel
        </SecondaryButton>
        <PrimaryButton
          type="submit"
          disabled={
            mode === "create"
              ? createTechnicianMutation.isPending
              : updateTechnicianMutation.isPending
          }
        >
          {mode === "create"
            ? createTechnicianMutation.isPending
              ? "Creating..."
              : "Create Technician"
            : updateTechnicianMutation.isPending
              ? "Updating..."
              : "Update Technician"}
        </PrimaryButton>
      </div>
    </form>
  );
};

export default TechnicianForm;
