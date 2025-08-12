import React, { useState, useEffect } from "react";
import {
  CreateVehicleGroupCommand,
  UpdateVehicleGroupCommand,
  VehicleGroup,
} from "@/features/vehicle-group/types/vehicleGroupType";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import ModalPortal from "@/components/ui/Modal/ModalPortal";

interface VehicleGroupModalProps {
  isOpen: boolean;
  mode: "create" | "edit";
  vehicleGroup?: VehicleGroup;
  onClose: () => void;
  onSubmit: (
    data: CreateVehicleGroupCommand | UpdateVehicleGroupCommand,
  ) => Promise<void>;
  isLoading?: boolean;
}

interface FormData {
  name: string;
  description: string;
  isActive: boolean;
}

export const VehicleGroupModal: React.FC<VehicleGroupModalProps> = ({
  isOpen,
  mode,
  vehicleGroup,
  onClose,
  onSubmit,
  isLoading = false,
}) => {
  const [formData, setFormData] = useState<FormData>({
    name: "",
    description: "",
    isActive: true,
  });

  useEffect(() => {
    if (mode === "edit" && vehicleGroup) {
      setFormData({
        name: vehicleGroup.name,
        description: vehicleGroup.description,
        isActive: vehicleGroup.isActive,
      });
    } else if (mode === "create") {
      setFormData({ name: "", description: "", isActive: true });
    }
  }, [mode, vehicleGroup, isOpen]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.name.trim()) return;

    if (mode === "create") {
      const createCommand: CreateVehicleGroupCommand = {
        name: formData.name.trim(),
        description: formData.description.trim(),
        isActive: formData.isActive,
      };
      await onSubmit(createCommand);
    } else if (mode === "edit" && vehicleGroup) {
      const updateCommand: UpdateVehicleGroupCommand = {
        vehicleGroupId: vehicleGroup.id,
        name: formData.name.trim(),
        description: formData.description.trim(),
        isActive: formData.isActive,
      };
      await onSubmit(updateCommand);
    }
  };

  if (!isOpen) return null;

  const title =
    mode === "create" ? "Create Vehicle Group" : "Edit Vehicle Group";
  const submitText = mode === "create" ? "Create" : "Update";
  const loadingText = mode === "create" ? "Creating..." : "Updating...";

  return (
    <ModalPortal isOpen={isOpen}>
      <div className="fixed inset-0 backdrop-brightness-50 bg-opacity-50 flex items-center justify-center z-[100]">
        <div className="bg-white rounded-lg p-6 w-full max-w-md">
          <h2 className="text-lg font-semibold mb-4">{title}</h2>
          <form onSubmit={handleSubmit}>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Group Name{" "}
                  <span
                    className={`${!formData.name.trim() ? "text-red-500" : "text-gray-500"}`}
                  >
                    *
                  </span>
                </label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={e =>
                    setFormData({ ...formData, name: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-[var(--border)] rounded-md focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Description
                </label>
                <textarea
                  value={formData.description}
                  onChange={e =>
                    setFormData({ ...formData, description: e.target.value })
                  }
                  rows={3}
                  className="w-full px-3 py-2 border border-[var(--border)] rounded-md focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
                />
              </div>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <SecondaryButton onClick={onClose}>Cancel</SecondaryButton>
              <PrimaryButton type="submit" disabled={isLoading}>
                {isLoading ? loadingText : submitText}
              </PrimaryButton>
            </div>
          </form>
        </div>
      </div>
    </ModalPortal>
  );
};
