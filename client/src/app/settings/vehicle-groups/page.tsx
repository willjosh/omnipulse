"use client";

import React, { useState, useMemo } from "react";
import { useVehicleGroups } from "@/app/_hooks/vehicle-groups/useVehicleGroups";
import {
  CreateVehicleGroupCommand,
  VehicleGroup,
} from "@/app/_hooks/vehicle-groups/vehicleGroupTypes";
import { Plus, Layers } from "lucide-react";
import { Loading } from "@/app/_features/shared/feedback";
import { DataTable } from "@/app/_features/shared/table";
import { ConfirmModal } from "@/app/_features/shared/modal";
import { Archive } from "@/app/_features/shared/icons";
import { vehicleGroupTableColumns } from "@/app/_features/vehicle-groups/VehicleGroupTableColumns";

const VehicleGroupsPage = () => {
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [selectedGroups, setSelectedGroups] = useState<string[]>([]);
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    vehicleGroup?: VehicleGroup;
  }>({ isOpen: false });

  const {
    vehicleGroups,
    isLoading,
    createVehicleGroupMutation,
    deleteVehicleGroupMutation,
  } = useVehicleGroups();

  const [formData, setFormData] = useState({
    Name: "",
    Description: "",
    IsActive: true,
  });

  const handleCreateGroup = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.Name.trim()) return;

    const createCommand: CreateVehicleGroupCommand = {
      Name: formData.Name.trim(),
      Description: formData.Description.trim(),
      IsActive: formData.IsActive,
    };

    try {
      await createVehicleGroupMutation.mutateAsync(createCommand);
      setShowCreateModal(false);
      setFormData({ Name: "", Description: "", IsActive: true });
    } catch (error) {
      console.error("Error creating vehicle group:", error);
    }
  };

  const handleCreateClick = () => {
    setShowCreateModal(true);
    setFormData({ Name: "", Description: "", IsActive: true });
  };

  const handleDeleteVehicleGroup = async () => {
    if (!confirmModal.vehicleGroup) return;

    try {
      await deleteVehicleGroupMutation.mutateAsync(
        confirmModal.vehicleGroup.id,
      );
      setConfirmModal({ isOpen: false });
    } catch (error) {
      console.error("Error deleting vehicle group:", error);
    }
  };

  const vehicleGroupActions = useMemo(
    () => [
      {
        key: "DELETE",
        label: "Delete",
        variant: "danger" as const,
        icon: <Archive />,
        onClick: (vehicleGroup: VehicleGroup) => {
          setConfirmModal({ isOpen: true, vehicleGroup });
        },
      },
    ],
    [],
  );

  const handleSelectAll = () => {
    if (!vehicleGroups) return;

    const allGroupIds = vehicleGroups.map(group => group.id.toString());
    const allSelected = allGroupIds.every(id => selectedGroups.includes(id));

    if (allSelected) {
      setSelectedGroups([]);
    } else {
      setSelectedGroups(allGroupIds);
    }
  };

  const handleGroupSelect = (groupId: string) => {
    setSelectedGroups(prev =>
      prev.includes(groupId)
        ? prev.filter(id => id !== groupId)
        : [...prev, groupId],
    );
  };

  if (isLoading) {
    return <Loading />;
  }

  return (
    <div className="flex-1 p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <Layers className="w-8 h-8 text-[var(--primary-color)]" />
          <h1 className="text-2xl font-bold text-gray-900">Vehicle Groups</h1>
        </div>
        <button
          onClick={handleCreateClick}
          className="flex items-center gap-2 bg-[var(--primary-color)] text-white px-4 py-2 rounded-md hover:bg-blue-700 transition-colors"
        >
          <Plus size={16} />
          Create Group
        </button>
      </div>

      {/* Vehicle Groups Table */}
      <DataTable<VehicleGroup>
        data={vehicleGroups || []}
        columns={vehicleGroupTableColumns}
        selectedItems={selectedGroups}
        onSelectItem={handleGroupSelect}
        onSelectAll={handleSelectAll}
        actions={vehicleGroupActions}
        showActions={true}
        loading={isLoading}
        fixedLayout={false}
        getItemId={group =>
          group.id ? group.id.toString() : `temp-${Date.now()}`
        }
        emptyState={
          <div className="text-center py-8">
            <Layers className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500">No vehicle groups found</p>
          </div>
        }
      />

      {/* Create Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 backdrop-brightness-50 bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h2 className="text-lg font-semibold mb-4">Create Vehicle Group</h2>
            <form onSubmit={handleCreateGroup}>
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
                    id="isActive"
                    checked={formData.IsActive}
                    onChange={e =>
                      setFormData({ ...formData, IsActive: e.target.checked })
                    }
                    className="mr-2"
                  />
                  <label htmlFor="isActive" className="text-sm text-gray-700">
                    Active
                  </label>
                </div>
              </div>
              <div className="flex justify-end gap-3 mt-6">
                <button
                  type="button"
                  onClick={() => setShowCreateModal(false)}
                  className="px-4 py-2 text-gray-700 border border-[var(--border)] rounded-md hover:bg-gray-50 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={createVehicleGroupMutation.isPending}
                  className="px-4 py-2 bg-[var(--primary-color)] text-white rounded-md hover:bg-blue-700 transition-colors disabled:opacity-50"
                >
                  {createVehicleGroupMutation.isPending
                    ? "Creating..."
                    : "Create"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleDeleteVehicleGroup}
        title="Delete Vehicle Group"
        message={`Are you sure you want to delete "${confirmModal.vehicleGroup?.Name}"? This action cannot be undone.`}
        confirmText="Delete"
        cancelText="Cancel"
      />
    </div>
  );
};

export default VehicleGroupsPage;
