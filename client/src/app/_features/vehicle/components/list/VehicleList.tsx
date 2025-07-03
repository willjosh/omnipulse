"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import {
  VehicleListItem,
  VehicleListFilters,
} from "../../types/VehicleListTypes";
import { MOCK_VEHICLES } from "../../types/VehicleListTypes";
import ConfirmModal from "@/app/_features/shared/ConfirmModal";
import DataTable from "@/app/_features/shared/DataTable";
import FilterBar from "@/app/_features/shared/FilterBar";
import TabNavigation from "@/app/_features/shared/TabNavigation";
import PaginationControls from "@/app/_features/shared/PaginationControls";
import OptionButton from "@/app/_features/shared/button/OptionButton";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import { vehicleTableColumns } from "./VehicleTableColumns";
import { vehicleFilterConfig, vehicleTabConfig } from "./VehicleListFilters";
import Details from "@/app/_features/shared/icons/Details";
import Edit from "@/app/_features/shared/icons/Edit";
import Archive from "@/app/_features/shared/icons/Archive";

const VehicleList: React.FC = () => {
  const router = useRouter();
  const [filters, setFilters] = useState<VehicleListFilters>({
    tab: "all",
    page: 1,
    limit: 10,
    sortBy: "name",
    sortOrder: "asc",
  });
  const [selectedVehicles, setSelectedVehicles] = useState<string[]>([]);
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    vehicle?: VehicleListItem;
  }>({ isOpen: false });

  const filteredVehicles = useMemo(() => {
    let result = [...MOCK_VEHICLES];

    switch (filters.tab) {
      case "assigned":
        result = result.filter(vehicle => !!vehicle.assignedOperator);
        break;
      case "unassigned":
        result = result.filter(vehicle => !vehicle.assignedOperator);
        break;
      case "archived":
        result = result.filter(vehicle => vehicle.status === "Out of Service");
        break;
      case "all":
      default:
        break;
    }

    if (filters.search) {
      const searchLower = filters.search.toLowerCase();
      const searchFields: (keyof VehicleListItem)[] = [
        "name",
        "licensePlate",
        "vin",
        "assignedOperator",
        "location",
      ];
      result = result.filter(vehicle =>
        searchFields.some(field => {
          const value = vehicle[field];
          return value?.toString().toLowerCase().includes(searchLower);
        }),
      );
    }

    if (filters.vehicleType && filters.vehicleType !== "all") {
      result = result.filter(vehicle => vehicle.type === filters.vehicleType);
    }
    if (filters.vehicleStatus && filters.vehicleStatus !== "all") {
      result = result.filter(
        vehicle => vehicle.status === filters.vehicleStatus,
      );
    }
    if (filters.location && filters.location !== "all") {
      result = result.filter(vehicle => vehicle.location === filters.location);
    }

    if (filters.sortBy) {
      result.sort((a, b) => {
        let aValue = a[filters.sortBy as keyof VehicleListItem];
        let bValue = b[filters.sortBy as keyof VehicleListItem];

        if (aValue == null && bValue == null) return 0;
        if (aValue == null) return filters.sortOrder === "asc" ? 1 : -1;
        if (bValue == null) return filters.sortOrder === "asc" ? -1 : 1;

        if (typeof aValue === "number" && typeof bValue === "number") {
          return filters.sortOrder === "asc"
            ? aValue - bValue
            : bValue - aValue;
        }

        const aString = String(aValue).toLowerCase();
        const bString = String(bValue).toLowerCase();

        if (aString < bString) return filters.sortOrder === "asc" ? -1 : 1;
        if (aString > bString) return filters.sortOrder === "asc" ? 1 : -1;
        return 0;
      });
    }

    return result;
  }, [filters]);

  const totalVehicles = filteredVehicles.length;
  const totalPages = Math.ceil(totalVehicles / (filters.limit || 10));
  const currentPage = filters.page || 1;

  const paginatedVehicles = useMemo(() => {
    const startIndex = ((filters.page || 1) - 1) * (filters.limit || 10);
    const endIndex = startIndex + (filters.limit || 10);
    return filteredVehicles.slice(startIndex, endIndex);
  }, [filteredVehicles, filters.page, filters.limit]);

  const vehicleActions = [
    {
      key: "view",
      label: "View Details",
      icon: <Details />,
      onClick: (vehicle: VehicleListItem) => {
        router.push(`/vehicles/${vehicle.id}`);
      },
    },
    {
      key: "edit",
      label: "Edit Vehicle",
      icon: <Edit />,
      onClick: (vehicle: VehicleListItem) => {
        router.push(`/vehicles/${vehicle.id}/edit`);
      },
    },
    {
      key: "archive",
      label: "Archive",
      variant: "danger" as const,
      icon: <Archive />,
      onClick: (vehicle: VehicleListItem) => {
        setConfirmModal({ isOpen: true, vehicle });
      },
    },
  ];

  const handleArchiveConfirm = () => {
    if (confirmModal.vehicle) {
      console.log("Archiving vehicle:", confirmModal.vehicle.id);
    }
  };

  const handleTabChange = (tab: string) => {
    setFilters(prev => ({ ...prev, tab: tab as any, page: 1 }));
  };

  const handleSearchChange = (searchTerm: string) => {
    setFilters(prev => ({ ...prev, search: searchTerm, page: 1 }));
  };

  const handleFilterChange = (filterKey: string, value: string) => {
    setFilters(prev => ({ ...prev, [filterKey]: value, page: 1 }));
  };

  const handleSort = (sortKey: string) => {
    setFilters(prev => ({
      ...prev,
      sortBy: sortKey,
      sortOrder:
        prev.sortBy === sortKey && prev.sortOrder === "asc" ? "desc" : "asc",
    }));
  };

  const handleRowClick = (vehicle: VehicleListItem) => {
    router.push(`/vehicles/${vehicle.id}`);
  };

  const handlePreviousPage = () => {
    if (currentPage > 1) {
      setFilters(prev => ({ ...prev, page: currentPage - 1 }));
    }
  };

  const handleNextPage = () => {
    if (currentPage < totalPages) {
      setFilters(prev => ({ ...prev, page: currentPage + 1 }));
    }
  };

  const handleSelectAll = () => {
    const currentPageVehicleIds = paginatedVehicles.map(vehicle => vehicle.id);
    const allCurrentPageSelected = currentPageVehicleIds.every(id =>
      selectedVehicles.includes(id),
    );

    if (allCurrentPageSelected) {
      setSelectedVehicles(prev =>
        prev.filter(id => !currentPageVehicleIds.includes(id)),
      );
    } else {
      setSelectedVehicles(prev => [
        ...new Set([...prev, ...currentPageVehicleIds]),
      ]);
    }
  };

  const handleVehicleSelect = (vehicleId: string) => {
    setSelectedVehicles(prev =>
      prev.includes(vehicleId)
        ? prev.filter(id => id !== vehicleId)
        : [...prev, vehicleId],
    );
  };

  const handleClearFilters = () => {
    setFilters({
      tab: "all",
      page: 1,
      limit: 10,
      sortBy: "name",
      sortOrder: "asc",
    });
  };

  const filtersWithValues = vehicleFilterConfig.map(filter => ({
    ...filter,
    value: (filters[filter.key as keyof VehicleListFilters] as string) || "",
  }));

  const emptyState = (
    <>
      <p className="text-gray-500 mb-2">No results found.</p>
      <button
        onClick={handleClearFilters}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear filters
      </button>
    </>
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

      <TabNavigation
        tabs={vehicleTabConfig}
        activeTab={filters.tab}
        onTabChange={handleTabChange}
      />

      <div className="flex items-center justify-between mb-4">
        <FilterBar
          searchValue={filters.search || ""}
          onSearchChange={handleSearchChange}
          searchPlaceholder="Search vehicles..."
          filters={filtersWithValues}
          onFilterChange={handleFilterChange}
        />

        <PaginationControls
          currentPage={currentPage}
          totalPages={totalPages}
          totalItems={totalVehicles}
          itemsPerPage={filters.limit || 10}
          onPreviousPage={handlePreviousPage}
          onNextPage={handleNextPage}
        />
      </div>

      <DataTable
        data={paginatedVehicles}
        columns={vehicleTableColumns}
        selectedItems={selectedVehicles}
        onSelectItem={handleVehicleSelect}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        onSort={handleSort}
        sortBy={filters.sortBy}
        sortOrder={filters.sortOrder}
        emptyState={emptyState}
        actions={vehicleActions}
        showActions={true}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleArchiveConfirm}
        title="Archive Vehicle"
        message={`Are you sure you want to archive ${confirmModal.vehicle?.name}? This action can be undone later.`}
        confirmText="Archive"
        cancelText="Cancel"
      />
    </div>
  );
};

export default VehicleList;
