"use client";
import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { FormField } from "@/app/_features/shared/form";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";
import {
  useCreateTechnician,
  useUpdateTechnician,
  useTechnician,
} from "@/app/_hooks/technician/useTechnicians";
import {
  CreateTechnicianCommand,
  UpdateTechnicianCommand,
} from "@/app/_hooks/technician/technicianType";

interface TechnicianFormProps {
  mode: "create" | "edit";
  technicianId?: string;
}

const TechnicianForm: React.FC<TechnicianFormProps> = ({
  mode,
  technicianId,
}) => {
  const router = useRouter();
  const createTechnicianMutation = useCreateTechnician();
  const updateTechnicianMutation = useUpdateTechnician();

  const { data: existingTechnician, isLoading: isLoadingTechnician } =
    useTechnician(technicianId || "");

  const [formData, setFormData] = useState<CreateTechnicianCommand>({
    Email: "",
    FirstName: "",
    LastName: "",
    HireDate: new Date().toISOString().split("T")[0],
    IsActive: true,
  });

  const [errors, setErrors] = useState<Partial<CreateTechnicianCommand>>({});

  // Load existing technician data when in edit mode
  useEffect(() => {
    if (mode === "edit" && existingTechnician) {
      setFormData({
        Email: existingTechnician.Email,
        FirstName: existingTechnician.FirstName,
        LastName: existingTechnician.LastName,
        HireDate: existingTechnician.HireDate.split("T")[0],
        IsActive: existingTechnician.IsActive,
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

  const validateForm = (): boolean => {
    const newErrors: Partial<CreateTechnicianCommand> = {};

    if (!formData.FirstName.trim()) {
      newErrors.FirstName = "First name is required";
    }
    if (!formData.LastName.trim()) {
      newErrors.LastName = "Last name is required";
    }
    if (!formData.Email.trim()) {
      newErrors.Email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(formData.Email)) {
      newErrors.Email = "Email format is invalid";
    }

    if (!formData.HireDate) {
      newErrors.HireDate = "Hire date is required";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      if (mode === "create") {
        await createTechnicianMutation.mutateAsync(formData);
      } else {
        const updateData: UpdateTechnicianCommand = {
          id: technicianId!,
          FirstName: formData.FirstName,
          LastName: formData.LastName,
          Email: formData.Email,
          HireDate: formData.HireDate,
          IsActive: formData.IsActive,
        };

        await updateTechnicianMutation.mutateAsync(updateData);
      }

      router.push("/contacts");
    } catch (error) {
      console.error(
        `Error ${mode === "create" ? "creating" : "updating"} technician:`,
        error,
      );
    }
  };

  const handleCancel = () => {
    router.push("/contacts");
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-2 gap-4">
        <FormField
          label="First Name"
          htmlFor="firstName"
          required
          error={errors.FirstName}
        >
          <input
            id="firstName"
            type="text"
            value={formData.FirstName}
            onChange={e => handleInputChange("FirstName", e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="Enter first name"
          />
        </FormField>

        <FormField
          label="Last Name"
          htmlFor="lastName"
          required
          error={errors.LastName}
        >
          <input
            id="lastName"
            type="text"
            value={formData.LastName}
            onChange={e => handleInputChange("LastName", e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="Enter last name"
          />
        </FormField>
      </div>

      <FormField
        label="Email Address"
        htmlFor="email"
        required
        error={errors.Email}
      >
        <input
          id="email"
          type="email"
          value={formData.Email}
          onChange={e => handleInputChange("Email", e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          placeholder="Enter email address"
        />
      </FormField>

      <FormField
        label="Hire Date"
        htmlFor="hireDate"
        required
        error={errors.HireDate}
      >
        <input
          id="hireDate"
          type="date"
          value={formData.HireDate}
          onChange={e => handleInputChange("HireDate", e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
      </FormField>

      <FormField label="Status" htmlFor="isActive">
        <div className="flex items-center space-x-3">
          <label className="flex items-center">
            <input
              type="radio"
              name="status"
              checked={formData.IsActive === true}
              onChange={() => handleInputChange("IsActive", true)}
              className="mr-2 text-blue-600"
            />
            <span className="text-sm text-gray-700">Active</span>
          </label>
          <label className="flex items-center">
            <input
              type="radio"
              name="status"
              checked={formData.IsActive === false}
              onChange={() => handleInputChange("IsActive", false)}
              className="mr-2 text-blue-600"
            />
            <span className="text-sm text-gray-700">Inactive</span>
          </label>
        </div>
      </FormField>

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
