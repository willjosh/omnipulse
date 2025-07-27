"use client";
import React, { useState, useEffect, useMemo } from "react";
import { useParams, useRouter } from "next/navigation";
import ServiceProgramHeader from "@/app/_features/service-program/components/ServiceProgramHeader";
import { TabNavigation } from "@/app/_features/shared/tabs";
import { PrimaryButton } from "@/app/_features/shared/button";
import { ChevronDown, Plus } from "lucide-react";
import { useServiceProgram } from "@/app/_hooks/service-program/useServicePrograms";
import { useServiceSchedules } from "@/app/_hooks/service-schedule/useServiceSchedules";
import {
  useServiceProgramVehicles,
  useRemoveVehicleFromServiceProgram,
} from "@/app/_hooks/service-program/useServiceProgramVehicles";
import Loading from "@/app/_features/shared/feedback/Loading";
import EditIcon from "@/app/_features/shared/icons/Edit";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import DataTable from "@/app/_features/shared/table/DataTable";
import FilterBar from "@/app/_features/shared/filter/FilterBar";
import PaginationControls from "@/app/_features/shared/table/PaginationControls";
import { ConfirmModal } from "@/app/_features/shared/modal";
import { Archive } from "@/app/_features/shared/icons";
import { useNotification } from "@/app/_features/shared/feedback/NotificationProvider";
import { ServiceScheduleWithLabels } from "@/app/_hooks/service-schedule/serviceScheduleType";
import { serviceProgramVehicleTableColumns } from "@/app/_hooks/service-program/serviceProgramVehicleTableColumns";
import { ServiceProgramVehicleWithDetails } from "@/app/_hooks/service-program/useServiceProgramVehicles";

export default function ServiceProgramDetailsPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id ? Number(params.id) : undefined;

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const {
    data: serviceProgram,
    isPending: isLoadingProgram,
    isError: isProgramError,
  } = useServiceProgram(id!);
  const {
    serviceSchedules,
    pagination,
    isPending: isLoadingSchedules,
  } = useServiceSchedules({
    PageNumber: page,
    PageSize: pageSize,
    Search: search,
  });

  // Service Program Vehicles
  const {
    serviceProgramVehicles,
    pagination: vehiclesPagination,
    isPending: isLoadingVehicles,
  } = useServiceProgramVehicles(id!, {
    PageNumber: page,
    PageSize: pageSize,
    Search: search,
  });

  // Remove vehicle mutation
  const removeVehicleMutation = useRemoveVehicleFromServiceProgram();
  const notify = useNotification();

  // Filter service schedules to only show those belonging to this service program
  const filteredServiceSchedules = serviceSchedules.filter(
    schedule => schedule.serviceProgramID === id,
  );

  const [activeTab, setActiveTab] = useState("schedules");
  const [isAddDropdownOpen, setIsAddDropdownOpen] = useState(false);
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    vehicle?: ServiceProgramVehicleWithDetails;
  }>({ isOpen: false });

  const tabs = [
    { key: "schedules", label: "Service Schedules" },
    { key: "vehicles", label: "Vehicles" },
  ];

  // Reset page to 1 when search changes
  useEffect(() => {
    setPage(1);
  }, [search]);

  // Define vehicle actions before early returns to maintain hook order
  const vehicleActions = useMemo(
    () => (spVehicle: ServiceProgramVehicleWithDetails) => [
      {
        key: "remove",
        label: "Delete",
        variant: "danger" as const,
        icon: <Archive />,
        onClick: (vehicle: ServiceProgramVehicleWithDetails) => {
          setConfirmModal({ isOpen: true, vehicle });
        },
      },
    ],
    [],
  );

  if (isLoadingProgram) {
    return <Loading />;
  }

  if (isProgramError || !serviceProgram) {
    return (
      <EmptyState
        title="Service Program not found"
        message="The service program you are looking for does not exist or could not be loaded."
      />
    );
  }

  const breadcrumbs = [
    { label: "Service Programs", href: "/service-programs" },
  ];

  const handleEdit = () => {
    router.push(`/service-programs/${id}/edit`);
  };

  const handleAddServiceSchedule = () => {
    router.push(`/service-schedules/new?serviceProgramId=${id}`);
    setIsAddDropdownOpen(false);
  };

  const handleAddVehicle = () => {
    router.push(`/service-programs/${id}/vehicles/add`);
    setIsAddDropdownOpen(false);
  };

  const handleSelectItem = (itemId: string) => {
    setSelectedItems(prev =>
      prev.includes(itemId)
        ? prev.filter(id => id !== itemId)
        : [...prev, itemId],
    );
  };

  const handleSelectAll = () => {
    if (selectedItems.length === filteredServiceSchedules.length) {
      setSelectedItems([]);
    } else {
      setSelectedItems(
        filteredServiceSchedules.map(schedule => schedule.id.toString()),
      );
    }
  };

  const handleRowClick = (schedule: ServiceScheduleWithLabels) => {
    router.push(`/service-schedules/${schedule.id}`);
  };

  const handleVehicleRowClick = (
    spVehicle: ServiceProgramVehicleWithDetails,
  ) => {
    router.push(`/vehicles/${spVehicle.vehicle.id}`);
  };

  const handleRemoveVehicle = async () => {
    if (!confirmModal.vehicle) return;

    try {
      await removeVehicleMutation.mutateAsync({
        serviceProgramID: id!,
        vehicleID: confirmModal.vehicle.vehicleID,
      });
      setConfirmModal({ isOpen: false });
      notify(
        `Successfully removed "${confirmModal.vehicle.vehicle.name}" from the service program`,
        "success",
      );
    } catch (error) {
      console.error("Error removing vehicle from service program:", error);
      notify(
        "Failed to remove vehicle from service program. Please try again.",
        "error",
      );
    }
  };

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(1); // Reset to first page when changing page size
  };

  const serviceScheduleColumns = [
    { key: "name", header: "Name", width: "200px" },
    {
      key: "frequency",
      header: "Frequency",
      width: "150px",
      render: (schedule: ServiceScheduleWithLabels) => {
        if (schedule.timeIntervalValue && schedule.timeIntervalUnitLabel) {
          return `${schedule.timeIntervalValue} ${
            schedule.timeIntervalValue === 1
              ? schedule.timeIntervalUnitLabel.replace(/s$/, "")
              : schedule.timeIntervalUnitLabel
          }`;
        } else if (schedule.mileageInterval) {
          return `${schedule.mileageInterval} km`;
        }
        return "-";
      },
    },
    {
      key: "buffer",
      header: "Buffer",
      width: "150px",
      render: (schedule: ServiceScheduleWithLabels) => {
        if (schedule.timeBufferValue && schedule.timeBufferUnitLabel) {
          return `${schedule.timeBufferValue} ${
            schedule.timeBufferValue === 1
              ? schedule.timeBufferUnitLabel.replace(/s$/, "")
              : schedule.timeBufferUnitLabel
          }`;
        } else if (schedule.mileageBuffer) {
          return `${schedule.mileageBuffer} km`;
        }
        return "-";
      },
    },
    {
      key: "firstService",
      header: "First Service",
      width: "150px",
      render: (schedule: ServiceScheduleWithLabels) => {
        if (
          schedule.firstServiceTimeValue &&
          schedule.firstServiceTimeUnitLabel
        ) {
          return `${schedule.firstServiceTimeValue} ${
            schedule.firstServiceTimeValue === 1
              ? schedule.firstServiceTimeUnitLabel.replace(/s$/, "")
              : schedule.firstServiceTimeUnitLabel
          }`;
        } else if (schedule.firstServiceMileage) {
          return `${schedule.firstServiceMileage} km`;
        }
        return "-";
      },
    },
    {
      key: "isActive",
      header: "Active",
      width: "100px",
      render: (schedule: ServiceScheduleWithLabels) =>
        schedule.isActive ? "Yes" : "No",
    },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      <ServiceProgramHeader
        title={serviceProgram.name}
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <PrimaryButton onClick={handleEdit}>
              <EditIcon /> Edit
            </PrimaryButton>
            <div className="relative ml-2">
              <PrimaryButton
                onClick={() => setIsAddDropdownOpen(!isAddDropdownOpen)}
                className="flex items-center"
              >
                <Plus size={14} />
                Add
                <ChevronDown size={16} />
              </PrimaryButton>
              {isAddDropdownOpen && (
                <>
                  <div className="absolute right-0 mt-1 w-48 bg-white rounded-md shadow-lg border border-gray-200 z-20">
                    <div className="py-1">
                      <button
                        onClick={handleAddServiceSchedule}
                        className="flex items-center w-full text-left px-3 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                      >
                        Add Service Schedule
                      </button>
                      <button
                        onClick={handleAddVehicle}
                        className="flex items-center w-full text-left px-3 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                      >
                        Add Vehicles
                      </button>
                    </div>
                  </div>
                  <div
                    className="fixed inset-0 z-10"
                    onClick={() => setIsAddDropdownOpen(false)}
                  />
                </>
              )}
            </div>
          </>
        }
      />
      <div className="px-6 mt-4 mb-8">
        {/* Service Program Description */}
        {serviceProgram.description &&
          serviceProgram.description.trim() !== "" && (
            <div className="mb-6 p-4 bg-white rounded-lg shadow">
              <h3 className="text-sm font-medium text-gray-900 mb-2">
                Description
              </h3>
              <p className="text-sm text-gray-600">
                {serviceProgram.description}
              </p>
            </div>
          )}

        <TabNavigation
          tabs={tabs}
          activeTab={activeTab}
          onTabChange={setActiveTab}
        />

        {activeTab === "schedules" && (
          <>
            <div className="mb-4 flex items-center justify-between">
              <FilterBar
                searchValue={search}
                onSearchChange={setSearch}
                searchPlaceholder="Search..."
                onFilterChange={() => {}}
              />
              {pagination && (
                <PaginationControls
                  currentPage={pagination.pageNumber}
                  totalPages={pagination.totalPages}
                  totalItems={pagination.totalCount}
                  itemsPerPage={pagination.pageSize}
                  onPreviousPage={() => handlePageChange(page - 1)}
                  onNextPage={() => handlePageChange(page + 1)}
                  onPageChange={handlePageChange}
                  onPageSizeChange={handlePageSizeChange}
                  className="ml-4"
                />
              )}
            </div>

            {!isLoadingSchedules && filteredServiceSchedules.length === 0 ? (
              <div className="bg-white rounded-lg shadow w-full">
                <EmptyState
                  title="No Service Schedules"
                  message={
                    search
                      ? "No service schedules found matching your search criteria."
                      : "No service schedules have been assigned to this service program yet."
                  }
                />
              </div>
            ) : (
              <div className="bg-white rounded-lg shadow w-full">
                <div className="w-full">
                  <DataTable
                    data={filteredServiceSchedules}
                    columns={serviceScheduleColumns}
                    selectedItems={selectedItems}
                    onSelectItem={handleSelectItem}
                    onSelectAll={handleSelectAll}
                    onRowClick={handleRowClick}
                    loading={isLoadingSchedules}
                    emptyState="No service schedules found"
                    getItemId={(item: ServiceScheduleWithLabels) =>
                      item.id.toString()
                    }
                    showActions={false}
                    fixedLayout={false}
                  />
                </div>
              </div>
            )}
          </>
        )}

        {activeTab === "vehicles" && (
          <>
            <div className="mb-4 flex items-center justify-between">
              <FilterBar
                searchValue={search}
                onSearchChange={setSearch}
                searchPlaceholder="Search vehicles..."
                onFilterChange={() => {}}
              />
              {vehiclesPagination && (
                <PaginationControls
                  currentPage={vehiclesPagination.pageNumber}
                  totalPages={vehiclesPagination.totalPages}
                  totalItems={vehiclesPagination.totalCount}
                  itemsPerPage={vehiclesPagination.pageSize}
                  onPreviousPage={() => handlePageChange(page - 1)}
                  onNextPage={() => handlePageChange(page + 1)}
                  onPageChange={handlePageChange}
                  onPageSizeChange={handlePageSizeChange}
                  className="ml-4"
                />
              )}
            </div>

            {!isLoadingVehicles && serviceProgramVehicles.length === 0 ? (
              <div className="bg-white rounded-lg shadow w-full">
                <EmptyState
                  title="No Vehicles Assigned"
                  message={
                    search
                      ? "No vehicles found matching your search criteria."
                      : "No vehicles have been assigned to this service program yet."
                  }
                />
              </div>
            ) : (
              <div className="bg-white rounded-lg shadow w-full">
                <div className="w-full">
                  <DataTable
                    data={serviceProgramVehicles}
                    columns={serviceProgramVehicleTableColumns}
                    selectedItems={selectedItems}
                    onSelectItem={handleSelectItem}
                    onSelectAll={handleSelectAll}
                    onRowClick={handleVehicleRowClick}
                    loading={isLoadingVehicles}
                    emptyState="No vehicles found"
                    getItemId={(item: ServiceProgramVehicleWithDetails) =>
                      item.vehicleID.toString()
                    }
                    showActions={true}
                    actions={vehicleActions}
                    fixedLayout={false}
                  />
                </div>
              </div>
            )}
          </>
        )}
      </div>

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() =>
          !removeVehicleMutation.isPending && setConfirmModal({ isOpen: false })
        }
        onConfirm={handleRemoveVehicle}
        title="Remove Vehicle from Program"
        message={
          confirmModal.vehicle
            ? `Are you sure you want to remove "${confirmModal.vehicle.vehicle.name}" from this service program?`
            : ""
        }
        confirmText={removeVehicleMutation.isPending ? "Removing..." : "Remove"}
        cancelText="Cancel"
      />
    </div>
  );
}
