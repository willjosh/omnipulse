"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { OptionButton, PrimaryButton } from "@/components/ui/Button";
import { ConfirmModal } from "@/components/ui/Modal";
import { FilterBar } from "@/components/ui/Filter";
import { Archive, Edit, Details } from "@/components/ui/Icons";
import { vehicleTableColumns } from "../config/VehicleTableColumns";
import {
  useVehicles,
  useDeactivateVehicle,
} from "@/features/vehicle/hooks/useVehicles";
import {
  Vehicle,
  VehicleWithLabels,
} from "@/features/vehicle/types/vehicleType";
import {
  VehicleActionType,
  VEHICLE_ACTION_CONFIG,
} from "../config/vehicleActions";

const VehicleList: React.FC = () => {
  const router = useRouter();
  const [selectedVehicles, setSelectedVehicles] = useState<string[]>([]);
  const [filters, setFilters] = useState({
    PageNumber: 1,
    PageSize: 10,
    SortBy: "name",
    SortDescending: false,
    Search: "",
  });
  const { vehicles, pagination, isPending, isError, error } =
    useVehicles(filters);
  const { mutateAsync: deactivateVehicle, isPending: isDeactivating } =
    useDeactivateVehicle();
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    vehicle?: VehicleWithLabels;
  }>({ isOpen: false });

  const handleArchiveVehicle = async () => {
    if (!confirmModal.vehicle) return;
    try {
      const vehicleId = confirmModal.vehicle.id.toString();
      await deactivateVehicle(vehicleId);
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

  const handleSort = (sortKey: string) => {
    setFilters(prev => ({
      ...prev,
      SortBy: sortKey,
      SortDescending: prev.SortBy === sortKey ? !prev.SortDescending : false,
      PageNumber: 1,
    }));
  };

  const handleSearch = (searchTerm: string) => {
    setFilters(prev => ({ ...prev, Search: searchTerm, PageNumber: 1 }));
  };

  const handlePageChange = (newPage: number) => {
    setFilters(prev => ({ ...prev, PageNumber: newPage }));
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setFilters(prev => ({ ...prev, PageSize: newPageSize, PageNumber: 1 }));
  };

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

  if (isPending || isDeactivating) {
    return <div className="text-center py-10">Loading vehicles...</div>;
  }

  if (isError) {
    return (
      <div className="text-center py-10 text-red-600">
        Error loading vehicles: {error?.message || "Unknown error"}
      </div>
    );
  }

  return (
    <div className="p-6 w-[1260px] min-h-screen mx-auto">
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
          searchValue={filters.Search}
          onSearchChange={handleSearch}
          searchPlaceholder="Search vehicles"
          // filters={"placeholder"}
          onFilterChange={() => console.log("Filter change")}
        />

        <PaginationControls
          currentPage={filters.PageNumber}
          totalPages={pagination?.totalPages || 0}
          totalItems={pagination?.totalCount || 0}
          itemsPerPage={filters.PageSize}
          onNextPage={() => handlePageChange(filters.PageNumber + 1)}
          onPreviousPage={() => handlePageChange(filters.PageNumber - 1)}
        />
      </div>

      <DataTable<VehicleWithLabels>
        data={vehicles || []}
        columns={vehicleTableColumns}
        selectedItems={selectedVehicles}
        onSelectItem={handleVehicleSelect}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        actions={vehicleActions}
        showActions={true}
        fixedLayout={false}
        loading={isPending || isDeactivating}
        getItemId={vehicle => vehicle.id.toString()}
        emptyState={emptyState}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleArchiveVehicle}
        title="Archive Vehicle"
        message={`Are you sure you want archive ${confirmModal.vehicle?.name}?`}
        confirmText="Archive"
        cancelText="Cancel"
      />
    </div>
  );
};

export default VehicleList;
