"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import DataTable from "@/app/_features/shared/DataTable";
import OptionButton from "@/app/_features/shared/button/OptionButton";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import { vehicleTableColumns } from "./VehicleTableColumns";
import { useVehicles } from "@/app/hooks/Vehicle/useVehicles";
import { Vehicle, VehicleWithLabels } from "@/app/hooks/Vehicle/vehicleType";
import TabNavigation from "@/app/_features/shared/TabNavigation";
import { vehicleTabConfig } from "./VehicleListFilters";
import PaginationControls from "@/app/_features/shared/PaginationControls";
import ConfirmModal from "@/app/_features/shared/ConfirmModal";
import FilterBar from "@/app/_features/shared/FilterBar";

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
  const { vehicles, pagination, isLoadingVehicles } = useVehicles(filters);
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    vehicle?: Vehicle[];
  }>({ isOpen: false });

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
      sortBy: sortKey,
      sortOrder:
        prev.sortBy === sortKey && prev.sortOrder === "asc" ? "desc" : "asc",
      page: 1, // Reset to first page when sorting
    }));
  };

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

  const handlePageSizeChange = (newPageSize: number) => {
    setFilters(prev => ({
      ...prev,
      limit: newPageSize,
      page: 1, // Reset to first page when changing page size
    }));
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
        {/* {/* Uncomment this when you implement filters} */}
        {/* <TabNavigation tabs={vehicleTabConfig} activeTab={filters} /> */}

        <FilterBar
          searchValue={filters.search}
          onSearchChange={handleSearch}
          searchPlaceholder="Search vehicles"
          // filters={"placeholder"}
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

      <DataTable<VehicleWithLabels>
        data={vehicles || []}
        columns={vehicleTableColumns}
        selectedItems={selectedVehicles}
        onSelectItem={handleVehicleSelect}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        showActions={true}
        loading={isLoadingVehicles}
        getItemId={vehicle => vehicle.id.toString()}
        emptyState={emptyState}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={() => console.log("confirm")}
        title="Archive Vehicle"
        message={"Are you sure you want to archive ?"}
        confirmText="Archive"
        cancelText="Cancel"
      />
    </div>
  );
};

export default VehicleList;
