"use client";
import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { FormField } from "@/components/ui/Form";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import {
  useCreateTechnician,
  useUpdateTechnician,
  useTechnician,
} from "../hooks/useTechnicians";
import {
  CreateTechnicianCommand,
  UpdateTechnicianCommand,
} from "../types/technicianType";

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

  // Load existing technician data when in edit mode
  useEffect(() => {
    if (mode === "edit" && existingTechnician) {
      setFormData({
        email: existingTechnician.email,
        password: "", // password is not editable in edit mode
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

  const validateForm = (): boolean => {
    const newErrors: Partial<CreateTechnicianCommand> = {};

    if (!formData.firstName.trim()) {
      newErrors.firstName = "First name is required";
    }
    if (!formData.lastName.trim()) {
      newErrors.lastName = "Last name is required";
    }
    if (!formData.email.trim()) {
      newErrors.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = "Email format is invalid";
    }

    if (!formData.hireDate) {
      newErrors.hireDate = "Hire date is required";
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
          firstName: formData.firstName,
          lastName: formData.lastName,
          hireDate: formData.hireDate,
          isActive: formData.isActive,
        };

        await updateTechnicianMutation.mutateAsync(updateData);
      }

      router.push("/contacts");
    } catch (error) {
      console.error(
        mode === "create"
          ? "Error creating technician:"
          : "Error updating technician:",
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
          error={errors.firstName}
        >
          <input
            id="firstName"
            type="text"
            value={formData.firstName}
            onChange={e => handleInputChange("firstName", e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="Enter first name"
          />
        </FormField>

        <FormField
          label="Last Name"
          htmlFor="lastName"
          required
          error={errors.lastName}
        >
          <input
            id="lastName"
            type="text"
            value={formData.lastName}
            onChange={e => handleInputChange("lastName", e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="Enter last name"
          />
        </FormField>
      </div>

      <FormField
        label="Email Address"
        htmlFor="email"
        required
        error={errors.email}
      >
        <input
          id="email"
          type="email"
          value={formData.email}
          onChange={e => handleInputChange("email", e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          placeholder="Enter email address"
        />
      </FormField>

      <FormField
        label="Hire Date"
        htmlFor="hireDate"
        required
        error={errors.hireDate}
      >
        <input
          id="hireDate"
          type="date"
          value={formData.hireDate}
          onChange={e => handleInputChange("hireDate", e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
      </FormField>

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
