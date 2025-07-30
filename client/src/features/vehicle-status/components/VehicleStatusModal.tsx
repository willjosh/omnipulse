"use client";

import React, { useState, useEffect } from "react";
import {
  CreateVehicleStatusCommand,
  UpdateVehicleStatusCommand,
  VehicleStatus,
} from "../types/vehicleStatusType";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";

interface VehicleStatusModalProps {
  isOpen: boolean;
  mode: "create" | "edit";
  vehicleStatus?: VehicleStatus;
  onClose: () => void;
  onSubmit: (
    data: CreateVehicleStatusCommand | UpdateVehicleStatusCommand,
  ) => Promise<void>;
  isLoading?: boolean;
}

interface FormData {
  name: string;
  color: string;
}

interface FormErrors {
  name?: string;
  color?: string;
}

const PREDEFINED_COLORS = [
  { value: "green", label: "Green", colorClass: "bg-green-100 text-green-800" },
  {
    value: "emerald",
    label: "Emerald",
    colorClass: "bg-emerald-100 text-emerald-800",
  },
  { value: "teal", label: "Teal", colorClass: "bg-teal-100 text-teal-800" },
  { value: "cyan", label: "Cyan", colorClass: "bg-cyan-100 text-cyan-800" },
  { value: "sky", label: "Sky Blue", colorClass: "bg-sky-100 text-sky-800" },
  { value: "blue", label: "Blue", colorClass: "bg-blue-100 text-blue-800" },
  {
    value: "indigo",
    label: "Indigo",
    colorClass: "bg-indigo-100 text-indigo-800",
  },
  {
    value: "violet",
    label: "Violet",
    colorClass: "bg-violet-100 text-violet-800",
  },
  {
    value: "purple",
    label: "Purple",
    colorClass: "bg-purple-100 text-purple-800",
  },
  {
    value: "fuchsia",
    label: "Fuchsia",
    colorClass: "bg-fuchsia-100 text-fuchsia-800",
  },
  { value: "pink", label: "Pink", colorClass: "bg-pink-100 text-pink-800" },
  { value: "rose", label: "Rose", colorClass: "bg-rose-100 text-rose-800" },
  { value: "red", label: "Red", colorClass: "bg-red-100 text-red-800" },
  {
    value: "orange",
    label: "Orange",
    colorClass: "bg-orange-100 text-orange-800",
  },
  { value: "amber", label: "Amber", colorClass: "bg-amber-100 text-amber-800" },
  {
    value: "yellow",
    label: "Yellow",
    colorClass: "bg-yellow-100 text-yellow-800",
  },
  { value: "lime", label: "Lime", colorClass: "bg-lime-100 text-lime-800" },
  { value: "slate", label: "Slate", colorClass: "bg-slate-100 text-slate-800" },
  { value: "gray", label: "Gray", colorClass: "bg-gray-100 text-gray-800" },
  { value: "zinc", label: "Zinc", colorClass: "bg-zinc-100 text-zinc-800" },
  {
    value: "neutral",
    label: "Neutral",
    colorClass: "bg-neutral-100 text-neutral-800",
  },
  { value: "stone", label: "Stone", colorClass: "bg-stone-100 text-stone-800" },
];

const VehicleStatusModal: React.FC<VehicleStatusModalProps> = ({
  isOpen,
  mode,
  vehicleStatus,
  onClose,
  onSubmit,
  isLoading = false,
}) => {
  const [formData, setFormData] = useState<FormData>({ name: "", color: "" });
  const [errors, setErrors] = useState<FormErrors>({});
  const [showValidation, setShowValidation] = useState(false);

  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = "hidden";
      // Initialize form based on mode
      if (mode === "edit" && vehicleStatus) {
        setFormData({ name: vehicleStatus.name, color: vehicleStatus.color });
      } else if (mode === "create") {
        setFormData({ name: "", color: "" });
      }
      setErrors({});
      setShowValidation(false);
    } else {
      document.body.style.overflow = "unset";
    }

    return () => {
      document.body.style.overflow = "unset";
    };
  }, [isOpen, mode, vehicleStatus]);

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    if (!formData.name.trim()) {
      newErrors.name = "Status name is required";
    } else if (formData.name.trim().length < 2) {
      newErrors.name = "Status name must be at least 2 characters";
    }

    if (!formData.color) {
      newErrors.color = "Please select a color";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setShowValidation(true);

    if (!validateForm()) {
      return;
    }

    try {
      if (mode === "create") {
        const command: CreateVehicleStatusCommand = {
          name: formData.name.trim(),
          color: formData.color,
        };
        await onSubmit(command);
      } else if (mode === "edit" && vehicleStatus) {
        const command: UpdateVehicleStatusCommand = {
          id: vehicleStatus.id,
          name: formData.name.trim(),
          color: formData.color,
        };
        await onSubmit(command);
      }
      onClose();
    } catch (error) {
      console.error("Error saving vehicle status:", error);
      const action = mode === "create" ? "create" : "update";
      setErrors({
        name: `Failed to ${action} vehicle status. Please try again.`,
      });
    }
  };

  const handleInputChange = (field: keyof FormData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const getFieldError = (field: keyof FormErrors) => {
    return showValidation ? errors[field] : undefined;
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="fixed inset-0 backdrop-brightness-50" onClick={onClose} />
      <div className="relative bg-white rounded-lg p-6 max-w-md mx-4 shadow-xl border border-gray-200 w-full">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">
          {mode === "create"
            ? "Create New Vehicle Status"
            : "Edit Vehicle Status"}
        </h3>

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Name Field */}
          <div>
            <label
              htmlFor="statusName"
              className="block text-sm font-medium text-gray-700 mb-2"
            >
              Status Name *
            </label>
            <input
              type="text"
              id="statusName"
              value={formData.name}
              onChange={e => handleInputChange("name", e.target.value)}
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                getFieldError("name") ? "border-red-500" : "border-gray-300"
              }`}
              placeholder="Enter status name"
              maxLength={50}
            />
            {getFieldError("name") && (
              <p className="mt-1 text-sm text-red-600">
                {getFieldError("name")}
              </p>
            )}
          </div>

          {/* Color Field */}
          <div>
            <label
              htmlFor="statusColor"
              className="block text-sm font-medium text-gray-700 mb-2"
            >
              Color *
            </label>
            <select
              id="statusColor"
              value={formData.color}
              onChange={e => handleInputChange("color", e.target.value)}
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                getFieldError("color") ? "border-red-500" : "border-gray-300"
              }`}
            >
              <option value="">Select a color</option>
              {PREDEFINED_COLORS.map(color => (
                <option key={color.value} value={color.value}>
                  {color.label} ({color.value})
                </option>
              ))}
            </select>
            {getFieldError("color") && (
              <p className="mt-1 text-sm text-red-600">
                {getFieldError("color")}
              </p>
            )}
          </div>

          {/* Color Preview */}
          {formData.color && (
            <div className="mt-3">
              <p className="text-sm font-medium text-gray-700 mb-2">Preview:</p>
              <span
                className={`inline-flex px-3 py-1 text-sm font-medium rounded-full ${
                  PREDEFINED_COLORS.find(c => c.value === formData.color)
                    ?.colorClass || "bg-gray-100 text-gray-800"
                }`}
              >
                {formData.name || "Status Name"}
              </span>
            </div>
          )}

          {/* Action Buttons */}
          <div className="flex justify-end gap-3 mt-6 pt-4 border-t border-gray-200">
            <SecondaryButton
              type="button"
              onClick={onClose}
              disabled={isLoading}
            >
              Cancel
            </SecondaryButton>
            <PrimaryButton type="submit" disabled={isLoading}>
              {isLoading
                ? mode === "create"
                  ? "Creating..."
                  : "Updating..."
                : mode === "create"
                  ? "Create Status"
                  : "Update Status"}
            </PrimaryButton>
          </div>
        </form>
      </div>
    </div>
  );
};

export default VehicleStatusModal;
