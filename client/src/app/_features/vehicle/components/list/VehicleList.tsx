"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import DataTable from "@/app/_features/shared/DataTable";
import OptionButton from "@/app/_features/shared/button/OptionButton";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import { vehicleTableColumns } from "./VehicleTableColumns";
import { useVehicles } from "@/app/_hooks/Vehicle/useVehicles";
import { Vehicle, VehicleWithLabels } from "@/app/_hooks/Vehicle/vehicleType";
// import TabNavigation from "@/app/_features/shared/TabNavigation";
// import { vehicleFilterConfig, vehicleTabConfig } from "./VehicleListFilters";
import PaginationControls from "@/app/_features/shared/PaginationControls";
import ConfirmModal from "@/app/_features/shared/ConfirmModal";
import FilterBar from "@/app/_features/shared/FilterBar";
import { Archive, Edit, Details } from "@/app/_features/shared/icons";
import {
  VehicleActionType,
  VEHICLE_ACTION_CONFIG,
} from "../../config/vehicleActions";

const VehicleList: React.FC = () => {
  const router = useRouter();
  const [selectedVehicles, setSelectedVehicles] = useState<string[]>([]);
  const [filters, setFilters] = useState({
    page: 1,
    pageSize: 10,
    sortBy: "Name",
    sortOrder: "asc" as "asc" | "desc",
    search: "",
  });
  const { vehicles, pagination, isLoadingVehicles, deactivateVehicleMutation } =
    useVehicles(filters);
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    vehicle?: VehicleWithLabels;
  }>({ isOpen: false });

  const handleArchiveVehicle = async () => {
    if (!confirmModal.vehicle) return;

    try {
      const vehicleId = confirmModal.vehicle.id.toString();
      await deactivateVehicleMutation.mutateAsync(vehicleId);

      setConfirmModal({ isOpen: false });
    } catch (error) {
      console.error("Error archiving vehicle:", error);
    }
  };

  const vehicleActions = useMemo(
    () => [
      {
        key: VehicleActionType.VIEW,
        label: VEHICLE_ACTION_CONFIG[VehicleActionType.VIEW].label,
        icon: <Details />,
        onClick: (vehicle: VehicleWithLabels) => {
          router.push(`/vehicles/${vehicle.id}`);
        },
      },
      {
        key: VehicleActionType.EDIT,
        label: VEHICLE_ACTION_CONFIG[VehicleActionType.EDIT].label,
        icon: <Edit />,
        onClick: (vehicle: VehicleWithLabels) => {
          router.push(`/vehicles/${vehicle.id}/edit`);
        },
      },
      {
        key: VehicleActionType.ARCHIVE,
        label: VEHICLE_ACTION_CONFIG[VehicleActionType.ARCHIVE].label,
        variant: VEHICLE_ACTION_CONFIG[VehicleActionType.ARCHIVE].variant,
        icon: <Archive />,
        onClick: (vehicle: VehicleWithLabels) => {
          setConfirmModal({ isOpen: true, vehicle });
        },
      },
    ],
    [router],
  );

  // Handle selection functions
  const handleSelectAll = () => {
    if (!vehicles) return;

    const allVehicleIds = vehicles.map(vehicle => vehicle.id.toString());
    const allSelected = allVehicleIds.every(id =>
      selectedVehicles.includes(id),
    );

    if (allSelected) {
      setSelectedVehicles([]);
    } else {
      setSelectedVehicles(allVehicleIds);
    }
  };

  const handleVehicleSelect = (vehicleId: string) => {
    setSelectedVehicles(prev =>
      prev.includes(vehicleId)
        ? prev.filter(id => id !== vehicleId)
        : [...prev, vehicleId],
    );
  };

  // const handleSort = (sortKey: string) => {
  //   setFilters(prev => ({
  //     ...prev,
  //     sortBy: sortKey,
  //     sortOrder:
  //       prev.sortBy === sortKey && prev.sortOrder === "asc" ? "desc" : "asc",
  //     page: 1, // Reset to first page when sorting
  //   }));
  // };

  const handleSearch = (searchTerm: string) => {
    setFilters(prev => ({
      ...prev,
      search: searchTerm,
      page: 1, // Reset to first page when searching
    }));
  };

  const handlePageChange = (newPage: number) => {
    setFilters(prev => ({ ...prev, page: newPage }));
  };

  // const handlePageSizeChange = (newPageSize: number) => {
  //   setFilters(prev => ({
  //     ...prev,
  //     limit: newPageSize,
  //     page: 1, // Reset to first page when changing page size
  //   }));
  // };

  const handleRowClick = (vehicle: Vehicle) => {
    router.push(`/vehicles/${vehicle.id}`);
  };

  const emptyState = (
    <div className="text-center">
      <p className="text-gray-500 mb-2">No vehicles found.</p>
      <button
        onClick={() => console.log("Clear filters")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear filters
      </button>
    </div>
  );

  return (
    <div className="px-6 w-[1260px] min-h-screen mx-auto">
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-semibold text-gray-900">Vehicles</h1>
        <div className="flex items-center gap-3">
          <OptionButton />
          <PrimaryButton onClick={() => router.push("/vehicles/create")}>
            <span>+</span>
            Add Vehicles
          </PrimaryButton>
        </div>
      </div>

      <div className="flex items-center justify-between mb-4">
        {/* Filter tabs not yet implemented */}
        {/* <TabNavigation tabs={vehicleTabConfig} activeTab={filters} /> */}

        <FilterBar
          searchValue={filters.search}
          onSearchChange={handleSearch}
          searchPlaceholder="Search vehicles"
          filters={[]}
          onFilterChange={() => console.log("Filter change")}
        />

        <PaginationControls
          currentPage={filters.page}
          totalPages={pagination?.totalPages || 0}
          totalItems={pagination?.totalCount || 0}
          itemsPerPage={filters.pageSize}
          onNextPage={() => handlePageChange(filters.page + 1)}
          onPreviousPage={() => handlePageChange(filters.page - 1)}
        />
      </div>

      <DataTable
        data={vehicles || []}
        columns={vehicleTableColumns}
        selectedItems={selectedVehicles}
        onSelectItem={handleVehicleSelect}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        actions={vehicleActions}
        showActions={true}
        loading={isLoadingVehicles}
        emptyState={emptyState}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleArchiveVehicle}
        title="Archive Vehicle"
        message={`Are you sure you want archive ${confirmModal.vehicle?.Name}?`}
        confirmText="Archive"
        cancelText="Cancel"
      />
    </div>
  );
};

export default VehicleList;
