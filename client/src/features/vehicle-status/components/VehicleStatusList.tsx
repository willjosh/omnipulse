"use client";

import React, { useState, useMemo } from "react";
import {
  useVehicleStatuses,
  useCreateVehicleStatus,
  useUpdateVehicleStatus,
  useDeleteVehicleStatus,
} from "../hooks/useVehicleStatus";
import { VehicleStatus } from "../types/vehicleStatusType";
import { Settings } from "lucide-react";
import { Edit, Archive } from "@/components/ui/Icons";
import { Loading } from "@/components/ui/Feedback";
import { DataTable } from "@/components/ui/Table";
import { vehicleStatusTableColumns } from "@/features/vehicle-status/components/VehicleStatusTableColumns";
import { PrimaryButton } from "@/components/ui/Button";
import VehicleStatusModal from "@/features/vehicle-status/components/VehicleStatusModal";
import { ConfirmModal } from "@/components/ui/Modal";

export const VehicleStatusList: React.FC = () => {
  const [selectedStatuses, setSelectedStatuses] = useState<string[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<"create" | "edit">("create");
  const [editingStatus, setEditingStatus] = useState<
    VehicleStatus | undefined
  >();
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    vehicleStatus?: VehicleStatus;
  }>({ isOpen: false });

  const { vehicleStatuses, isPending, isError, error } = useVehicleStatuses();

  const createVehicleStatusMutation = useCreateVehicleStatus();
  const updateVehicleStatusMutation = useUpdateVehicleStatus();
  const deleteVehicleStatusMutation = useDeleteVehicleStatus();

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

  const handleDeleteVehicleStatus = async () => {
    if (!confirmModal.vehicleStatus) return;

    try {
      await deleteVehicleStatusMutation.mutateAsync(
        confirmModal.vehicleStatus.id,
      );
      setConfirmModal({ isOpen: false });
    } catch (error) {
      console.error("Error deleting vehicle status:", error);
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
      {
        key: "ARCHIVE",
        label: "Archive",
        variant: "danger" as const,
        icon: <Archive />,
        onClick: (vehicleStatus: VehicleStatus) => {
          setConfirmModal({ isOpen: true, vehicleStatus });
        },
      },
    ],
    [],
  );

  if (isPending) {
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
        loading={isPending}
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

      {/* Archive Confirmation Modal */}
      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleDeleteVehicleStatus}
        title="Archive Vehicle Status"
        message={`Are you sure you want to archive "${confirmModal.vehicleStatus?.name}"? This action cannot be undone.`}
        confirmText="Archive"
        cancelText="Cancel"
      />
    </div>
  );
};
