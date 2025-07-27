"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { PrimaryButton } from "../_features/shared/button";
import { Plus, Trash2 } from "lucide-react";
import { FilterBar } from "../_features/shared/filter";
import PaginationControls from "../_features/shared/table/PaginationControls";
import { DataTable } from "../_features/shared/table";
import { ConfirmModal } from "../_features/shared/modal";
import {
  useServicePrograms,
  useDeleteServiceProgram,
} from "../_hooks/service-program/useServicePrograms";
import { ServiceProgram } from "../_hooks/service-program/serviceProgramType";
import { useNotification } from "../_features/shared/feedback/NotificationProvider";

export default function ServiceProgramsPage() {
  const router = useRouter();
  const notify = useNotification();
  const { mutate: deleteServiceProgram, isPending: isDeleting } =
    useDeleteServiceProgram();

  // Search state
  const [search, setSearch] = useState("");

  // Confirmation modal state
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    serviceProgram: ServiceProgram | null;
  }>({ isOpen: false, serviceProgram: null });

  // Pagination state
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  // Reset page to 1 when search changes
  useEffect(() => {
    setPage(1);
  }, [search]);

  // Compose filter object for data fetching (include search)
  const filter = useMemo(
    () => ({ PageNumber: page, PageSize: pageSize, Search: search }),
    [page, pageSize, search],
  );

  // Fetch data
  const { servicePrograms, pagination, isPending } = useServicePrograms(filter);

  // Table columns
  const columns = [
    { key: "name", header: "Name", sortable: true, width: "200px" },
    {
      key: "description",
      header: "Description",
      render: (item: ServiceProgram) => item.description || "-",
      width: "260px",
    },
    {
      key: "isActive",
      header: "Active",
      render: (item: ServiceProgram) => (item.isActive ? "Yes" : "No"),
      width: "80px",
    },
    {
      key: "serviceScheduleCount",
      header: "# Schedules",
      render: (item: ServiceProgram) => item.serviceScheduleCount,
      width: "120px",
    },
    {
      key: "assignedVehicleCount",
      header: "# Vehicles",
      render: (item: ServiceProgram) => item.assignedVehicleCount,
      width: "120px",
    },
    {
      key: "createdAt",
      header: "Created",
      render: (item: ServiceProgram) =>
        new Date(item.createdAt).toLocaleDateString(),
      width: "120px",
    },
    {
      key: "updatedAt",
      header: "Updated",
      render: (item: ServiceProgram) =>
        new Date(item.updatedAt).toLocaleDateString(),
      width: "120px",
    },
  ];

  // Row click handler
  const handleRowClick = (row: ServiceProgram) => {
    router.push(`/service-programs/${row.id}`);
  };

  // Delete handler
  const handleDeleteServiceProgram = async () => {
    if (!confirmModal.serviceProgram) return;

    deleteServiceProgram(confirmModal.serviceProgram.id, {
      onSuccess: () => {
        notify("Service program deleted successfully!", "success");
        setConfirmModal({ isOpen: false, serviceProgram: null });
      },
      onError: error => {
        console.error("Failed to delete service program:", error);
        notify("Failed to delete service program", "error");
        setConfirmModal({ isOpen: false, serviceProgram: null });
      },
    });
  };

  // Actions for the table
  const serviceProgramActions = useMemo(
    () => [
      {
        key: "delete",
        label: "Delete",
        icon: <Trash2 size={16} />,
        variant: "danger" as const,
        onClick: (serviceProgram: ServiceProgram) => {
          setConfirmModal({ isOpen: true, serviceProgram });
        },
      },
    ],
    [],
  );

  // Empty state
  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No service programs found.</p>
      <button
        onClick={() => setSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear search
      </button>
    </div>
  );

  // Selection state (not functional yet)
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const handleSelectItem = (id: string) => {
    setSelectedItems(items =>
      items.includes(id)
        ? items.filter(itemId => itemId !== id)
        : [...items, id],
    );
  };
  const handleSelectAll = () => {
    if (servicePrograms.length === 0) return;
    if (selectedItems.length === servicePrograms.length) {
      setSelectedItems([]);
    } else {
      setSelectedItems(servicePrograms.map(item => item.id.toString()));
    }
  };

  return (
    <div className="p-6 w-[1260px] min-h-screen mx-auto">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Service Programs
        </h1>
        <PrimaryButton onClick={() => router.push("/service-programs/new")}>
          <Plus size={18} className="mr-2" />
          Add Service Program
        </PrimaryButton>
      </div>
      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search service programs..."
          onFilterChange={() => {}}
        />
        {pagination && (
          <PaginationControls
            currentPage={pagination.pageNumber}
            totalPages={pagination.totalPages}
            totalItems={pagination.totalCount}
            itemsPerPage={pagination.pageSize}
            onPreviousPage={() => setPage(p => Math.max(1, p - 1))}
            onNextPage={() =>
              setPage(p =>
                pagination && p < pagination.totalPages ? p + 1 : p,
              )
            }
            onPageChange={setPage}
            onPageSizeChange={setPageSize}
          />
        )}
      </div>
      <DataTable
        data={servicePrograms}
        columns={columns}
        selectedItems={selectedItems}
        onSelectItem={handleSelectItem}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        loading={isPending}
        emptyState={emptyState}
        getItemId={item => item.id.toString()}
        showActions={true}
        actions={serviceProgramActions}
        fixedLayout={false}
      />

      {/* Confirmation Modal */}
      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() =>
          !isDeleting &&
          setConfirmModal({ isOpen: false, serviceProgram: null })
        }
        onConfirm={handleDeleteServiceProgram}
        title="Delete Service Program"
        message={
          confirmModal.serviceProgram
            ? `Are you sure you want to delete "${confirmModal.serviceProgram.name}"? This action cannot be undone.`
            : ""
        }
        confirmText={isDeleting ? "Deleting..." : "Delete"}
      />
    </div>
  );
}
