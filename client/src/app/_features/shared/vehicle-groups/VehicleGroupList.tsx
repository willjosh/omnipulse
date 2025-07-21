"use client";

import React, { useState, useMemo } from "react";
import { useVehicleGroups } from "@/app/_hooks/vehicle-groups/useVehicleGroups";
import {
  CreateVehicleGroupCommand,
  UpdateVehicleGroupCommand,
  VehicleGroup,
} from "@/app/_hooks/vehicle-groups/vehicleGroupTypes";
import { Plus, Layers } from "lucide-react";
import { Loading } from "@/app/_features/shared/feedback";
import { DataTable } from "@/app/_features/shared/table";
import { ConfirmModal } from "@/app/_features/shared/modal";
import { Archive, Edit } from "@/app/_features/shared/icons";
import { PrimaryButton } from "@/app/_features/shared/button";
import { vehicleGroupTableColumns } from "@/app/_features/vehicle-groups/VehicleGroupTableColumns";
import { VehicleGroupModal } from "./VehicleGroupModal";

export const VehicleGroupList: React.FC = () => {
  const [modalState, setModalState] = useState<{
    isOpen: boolean;
    mode: "create" | "edit";
    vehicleGroup?: VehicleGroup;
  }>({ isOpen: false, mode: "create" });

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
    updateVehicleGroupMutation,
  } = useVehicleGroups();

  const handleCreateClick = () => {
    setModalState({ isOpen: true, mode: "create" });
  };

  const handleEditClick = (vehicleGroup: VehicleGroup) => {
    setModalState({ isOpen: true, mode: "edit", vehicleGroup });
  };

  const handleModalSubmit = async (
    data: CreateVehicleGroupCommand | UpdateVehicleGroupCommand,
  ) => {
    try {
      if (modalState.mode === "create") {
        await createVehicleGroupMutation.mutateAsync(
          data as CreateVehicleGroupCommand,
        );
      } else {
        await updateVehicleGroupMutation.mutateAsync(
          data as UpdateVehicleGroupCommand,
        );
      }
      setModalState({ isOpen: false, mode: "create" });
    } catch (error) {
      console.error(
        `Error ${modalState.mode === "create" ? "creating" : "updating"} vehicle group:`,
        error,
      );
    }
  };

  const handleModalClose = () => {
    setModalState({ isOpen: false, mode: "create" });
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
        key: "EDIT",
        label: "Edit",
        variant: "default" as const,
        icon: <Edit />,
        onClick: (vehicleGroup: VehicleGroup) => {
          handleEditClick(vehicleGroup);
        },
      },
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

  const isModalLoading =
    modalState.mode === "create"
      ? createVehicleGroupMutation.isPending
      : updateVehicleGroupMutation.isPending;

  return (
    <div className="flex-1 p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <Layers className="w-8 h-8 text-[var(--primary-color)]" />
          <h1 className="text-2xl font-bold text-gray-900">Vehicle Groups</h1>
        </div>
        <PrimaryButton onClick={handleCreateClick}>
          <Plus size={12} />
          Create Group
        </PrimaryButton>
      </div>

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

      <VehicleGroupModal
        isOpen={modalState.isOpen}
        mode={modalState.mode}
        vehicleGroup={modalState.vehicleGroup}
        onClose={handleModalClose}
        onSubmit={handleModalSubmit}
        isLoading={isModalLoading}
      />

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
