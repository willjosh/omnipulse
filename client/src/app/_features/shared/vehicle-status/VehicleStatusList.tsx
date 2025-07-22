"use client";

import React, { useState, useMemo } from "react";
import { useVehicleStatuses } from "@/app/_hooks/vehicle-status/useVehicleStatus";
import { VehicleStatus } from "@/app/_hooks/vehicle-status/vehicleStatusTypes";
import { Settings } from "lucide-react";
import { Edit } from "@/app/_features/shared/icons";
import { Loading } from "@/app/_features/shared/feedback";
import { DataTable } from "@/app/_features/shared/table";
import { vehicleStatusTableColumns } from "@/app/_features/vehicle-status/VehicleStatusTableColumns";
import { PrimaryButton } from "@/app/_features/shared/button";
import VehicleStatusModal from "./VehicleStatusModal";

export const VehicleStatusList: React.FC = () => {
  const [selectedStatuses, setSelectedStatuses] = useState<string[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<"create" | "edit">("create");
  const [editingStatus, setEditingStatus] = useState<
    VehicleStatus | undefined
  >();

  const {
    vehicleStatuses,
    isLoading,
    createVehicleStatusMutation,
    updateVehicleStatusMutation,
  } = useVehicleStatuses();

  const handleSelectAll = () => {
    if (!vehicleStatuses) return;

    const allStatusIds = vehicleStatuses.map(status => status.id.toString());
    const allSelected = allStatusIds.every(id => selectedStatuses.includes(id));

    if (allSelected) {
      setSelectedStatuses([]);
    } else {
      setSelectedStatuses(allStatusIds);
    }
  };

  const handleStatusSelect = (statusId: string) => {
    setSelectedStatuses(prev =>
      prev.includes(statusId)
        ? prev.filter(id => id !== statusId)
        : [...prev, statusId],
    );
  };

  const handleCreateStatus = () => {
    setModalMode("create");
    setEditingStatus(undefined);
    setIsModalOpen(true);
  };

  const handleEditStatus = (status: VehicleStatus) => {
    setModalMode("edit");
    setEditingStatus(status);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingStatus(undefined);
  };

  const handleSubmit = async (data: any) => {
    if (modalMode === "create") {
      await createVehicleStatusMutation.mutateAsync(data);
    } else {
      await updateVehicleStatusMutation.mutateAsync(data);
    }
  };

  const isSubmitting =
    createVehicleStatusMutation.isPending ||
    updateVehicleStatusMutation.isPending;

  const vehicleStatusActions = useMemo(
    () => [
      {
        key: "EDIT",
        label: "Edit",
        variant: "default" as const,
        icon: <Edit />,
        onClick: (vehicleStatus: VehicleStatus) => {
          handleEditStatus(vehicleStatus);
        },
      },
    ],
    [],
  );

  if (isLoading) {
    return <Loading />;
  }

  return (
    <div className="flex-1 p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <Settings className="w-8 h-8 text-[var(--primary-color)]" />
          <h1 className="text-2xl font-bold text-gray-900">Vehicle Status</h1>
        </div>
        <PrimaryButton onClick={handleCreateStatus}>
          <span>+</span>
          Create Status
        </PrimaryButton>
      </div>

      <DataTable<VehicleStatus>
        data={vehicleStatuses || []}
        columns={vehicleStatusTableColumns}
        selectedItems={selectedStatuses}
        onSelectItem={handleStatusSelect}
        onSelectAll={handleSelectAll}
        actions={vehicleStatusActions}
        showActions={true}
        loading={isLoading}
        fixedLayout={false}
        getItemId={status =>
          status.id ? status.id.toString() : `temp-${Date.now()}`
        }
        emptyState={
          <div className="text-center py-8">
            <Settings className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500">No vehicle statuses found</p>
          </div>
        }
      />

      {/* Vehicle Status Modal */}
      <VehicleStatusModal
        isOpen={isModalOpen}
        mode={modalMode}
        vehicleStatus={editingStatus}
        onClose={handleCloseModal}
        onSubmit={handleSubmit}
        isLoading={isSubmitting}
      />
    </div>
  );
};
