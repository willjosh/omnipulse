"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { OptionButton, PrimaryButton } from "@/components/ui/Button";
import { ConfirmModal } from "@/components/ui/Modal";
import { FilterBar } from "@/components/ui/Filter";
import { Archive, Edit, Details } from "@/components/ui/Icons";
import { fuelPurchaseTableColumns } from "../config/FuelPurchaseTableColumns";
import {
  useFuelPurchases,
  useDeleteFuelPurchase,
} from "../hooks/useFuelPurchases";
import {
  FuelPurchase,
  FuelPurchaseWithLabels,
} from "../types/fuelPurchaseType";
import {
  FuelPurchaseActionType,
  FUEL_PURCHASE_ACTION_CONFIG,
} from "../config/fuelPurchaseActions";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { Plus } from "lucide-react";

const FuelPurchaseList: React.FC = () => {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("purchaseDate");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc");

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

  const { fuelPurchases, pagination, isPending, isError, error } =
    useFuelPurchases(filters);
  const { mutateAsync: deleteFuelPurchase, isPending: isDeleting } =
    useDeleteFuelPurchase();
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    fuelPurchase?: FuelPurchaseWithLabels;
  }>({ isOpen: false });

  const handleDeleteFuelPurchase = async () => {
    if (!confirmModal.fuelPurchase) return;
    try {
      await deleteFuelPurchase(confirmModal.fuelPurchase.id);
      setConfirmModal({ isOpen: false });
    } catch (error) {
      console.error("Error deleting fuel purchase:", error);
    }
  };

  const fuelPurchaseActions = useMemo(
    () => [
      {
        key: FuelPurchaseActionType.VIEW,
        label: FUEL_PURCHASE_ACTION_CONFIG[FuelPurchaseActionType.VIEW].label,
        icon: <Details />,
        onClick: (fuelPurchase: FuelPurchaseWithLabels) => {
          router.push(`/fuel-purchases/${fuelPurchase.id}`);
        },
      },
      {
        key: FuelPurchaseActionType.EDIT,
        label: FUEL_PURCHASE_ACTION_CONFIG[FuelPurchaseActionType.EDIT].label,
        icon: <Edit />,
        onClick: (fuelPurchase: FuelPurchaseWithLabels) => {
          router.push(`/fuel-purchases/${fuelPurchase.id}/edit`);
        },
      },
      {
        key: FuelPurchaseActionType.DELETE,
        label: FUEL_PURCHASE_ACTION_CONFIG[FuelPurchaseActionType.DELETE].label,
        variant:
          FUEL_PURCHASE_ACTION_CONFIG[FuelPurchaseActionType.DELETE].variant,
        icon: <Archive />,
        onClick: (fuelPurchase: FuelPurchaseWithLabels) => {
          setConfirmModal({ isOpen: true, fuelPurchase });
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

  const handleRowClick = (fuelPurchase: FuelPurchase) => {
    router.push(`/fuel-purchases/${fuelPurchase.id}`);
  };

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No fuel purchases found.</p>
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
          Error loading fuel purchases: {error?.message || "Unknown error"}
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">Fuel Purchases</h1>
        <div className="flex items-center gap-3">
          <OptionButton />
          <PrimaryButton onClick={() => router.push("/fuel-purchases/new")}>
            <Plus size={16} />
            Add Fuel Purchase
          </PrimaryButton>
        </div>
      </div>

      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search fuel purchases..."
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

      <DataTable<FuelPurchaseWithLabels>
        data={fuelPurchases || []}
        columns={fuelPurchaseTableColumns}
        onRowClick={handleRowClick}
        actions={fuelPurchaseActions}
        showActions={true}
        fixedLayout={false}
        loading={isPending || isDeleting}
        getItemId={fuelPurchase => fuelPurchase.id.toString()}
        emptyState={emptyState}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleDeleteFuelPurchase}
        title="Delete Fuel Purchase"
        message={`Are you sure you want to delete fuel purchase ${confirmModal.fuelPurchase?.receiptNumber}?`}
        confirmText="Delete"
        cancelText="Cancel"
      />
    </div>
  );
};

export default FuelPurchaseList;
