"use client";
import React, { useState, useMemo, useEffect } from "react";
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
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { Plus } from "lucide-react";

const VehicleList: React.FC = () => {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("name");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

  useEffect(() => {
    setPage(1);
  }, [search, sortBy, sortOrder]);

  const filters = useMemo(
    () => ({
      PageNumber: page,
      PageSize: pageSize,
      SortBy: sortBy,
      SortDescending: sortOrder === "desc",
      Search: search,
    }),
    [page, pageSize, sortBy, sortOrder, search],
  );

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

  const handleSort = (sortKey: string) => {
    if (sortBy === sortKey) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(sortKey);
      setSortOrder("asc");
    }
    setPage(1);
  };

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(1);
  };

  const handleRowClick = (vehicle: Vehicle) => {
    router.push(`/vehicles/${vehicle.id}`);
  };

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No vehicles found.</p>
      <button
        onClick={() => setSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear search
      </button>
    </div>
  );

  if (isError) {
    return (
      <div className="p-6 w-full max-w-none">
        <div className="text-center py-10 text-red-600">
          Error loading vehicles: {error?.message || "Unknown error"}
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">Vehicles</h1>
        <div className="flex items-center gap-3">
          <OptionButton />
          <PrimaryButton onClick={() => router.push("/vehicles/create")}>
            <Plus size={16} />
            Add Vehicles
          </PrimaryButton>
        </div>
      </div>

      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search"
          onFilterChange={() => {}}
        />

        <PaginationControls
          currentPage={page}
          totalPages={pagination?.totalPages || 0}
          totalItems={pagination?.totalCount || 0}
          itemsPerPage={pageSize}
          onNextPage={() => handlePageChange(page + 1)}
          onPreviousPage={() => handlePageChange(page - 1)}
          onPageChange={setPage}
          onPageSizeChange={handlePageSizeChange}
        />
      </div>

      <DataTable<VehicleWithLabels>
        data={vehicles || []}
        columns={vehicleTableColumns}
        onRowClick={handleRowClick}
        actions={vehicleActions}
        showActions={true}
        fixedLayout={false}
        loading={isPending || isDeactivating}
        getItemId={vehicle => vehicle.id.toString()}
        emptyState={emptyState}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
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
