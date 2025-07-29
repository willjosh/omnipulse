"use client";

import React, { useState, useMemo } from "react";
import {
  useVehicleGroups,
  useCreateVehicleGroup,
  useUpdateVehicleGroup,
  useDeleteVehicleGroup,
} from "@/features/vehicle-group/hooks/useVehicleGroups";
import {
  CreateVehicleGroupCommand,
  UpdateVehicleGroupCommand,
  VehicleGroup,
} from "@/features/vehicle-group/types/vehicleGroupType";
import { Plus, Layers } from "lucide-react";
import { Loading } from "@/components/ui/Feedback";
import { DataTable } from "@/components/ui/Table";
import { ConfirmModal } from "@/components/ui/Modal";
import { Archive, Edit } from "@/components/ui/Icons";
import { PrimaryButton } from "@/components/ui/Button";
import { vehicleGroupTableColumns } from "@/features/vehicle-group/config/VehicleGroupTableColumns";
import { VehicleGroupModal } from "@/features/vehicle-group/components/VehicleGroupModal";

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

  const { vehicleGroups, pagination, isPending, isError, isSuccess, error } =
    useVehicleGroups();
  const createVehicleGroupMutation = useCreateVehicleGroup();
  const updateVehicleGroupMutation = useUpdateVehicleGroup();
  const deleteVehicleGroupMutation = useDeleteVehicleGroup();

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

  if (isPending) {
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
        loading={isPending}
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
        message={`Are you sure you want to delete "${confirmModal.vehicleGroup?.name}"? This action cannot be undone.`}
        confirmText="Delete"
        cancelText="Cancel"
      />
    </div>
  );
};
