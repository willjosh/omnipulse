import React, { useState, useEffect } from "react";
import {
  CreateVehicleGroupCommand,
  UpdateVehicleGroupCommand,
  VehicleGroup,
} from "@/app/_hooks/vehicle-groups/vehicleGroupTypes";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";

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
  Name: string;
  Description: string;
  IsActive: boolean;
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
    Name: "",
    Description: "",
    IsActive: true,
  });

  useEffect(() => {
    if (mode === "edit" && vehicleGroup) {
      setFormData({
        Name: vehicleGroup.Name,
        Description: vehicleGroup.Description,
        IsActive: vehicleGroup.IsActive,
      });
    } else if (mode === "create") {
      setFormData({ Name: "", Description: "", IsActive: true });
    }
  }, [mode, vehicleGroup, isOpen]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.Name.trim()) return;

    if (mode === "create") {
      const createCommand: CreateVehicleGroupCommand = {
        Name: formData.Name.trim(),
        Description: formData.Description.trim(),
        IsActive: formData.IsActive,
      };
      await onSubmit(createCommand);
    } else if (mode === "edit" && vehicleGroup) {
      const updateCommand: UpdateVehicleGroupCommand = {
        id: vehicleGroup.id,
        Name: formData.Name.trim(),
        Description: formData.Description.trim(),
        IsActive: formData.IsActive,
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
    <div className="fixed inset-0 backdrop-brightness-50 bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <h2 className="text-lg font-semibold mb-4">{title}</h2>
        <form onSubmit={handleSubmit}>
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Group Name *
              </label>
              <input
                type="text"
                value={formData.Name}
                onChange={e =>
                  setFormData({ ...formData, Name: e.target.value })
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
                value={formData.Description}
                onChange={e =>
                  setFormData({ ...formData, Description: e.target.value })
                }
                rows={3}
                className="w-full px-3 py-2 border border-[var(--border)] rounded-md focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
              />
            </div>
            <div className="flex items-center">
              <input
                type="checkbox"
                id={`isActive-${mode}`}
                checked={formData.IsActive}
                onChange={e =>
                  setFormData({ ...formData, IsActive: e.target.checked })
                }
                className="mr-2"
              />
              <label
                htmlFor={`isActive-${mode}`}
                className="text-sm text-gray-700"
              >
                Active
              </label>
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
  );
};
