"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { PrimaryButton } from "@/components/ui/Button";
import { Plus, Trash2 } from "lucide-react";
import { FilterBar } from "@/components/ui/Filter";
import PaginationControls from "@/components/ui/Table/PaginationControls";
import { DataTable } from "@/components/ui/Table";
import { ConfirmModal } from "@/components/ui/Modal";
import {
  useServicePrograms,
  useDeleteServiceProgram,
} from "@/features/service-program/hooks/useServicePrograms";
import { ServiceProgram } from "@/features/service-program/types/serviceProgramType";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";

export default function ServiceProgramsPage() {
  const router = useRouter();
  const notify = useNotification();
  const { mutate: deleteServiceProgram, isPending: isDeleting } =
    useDeleteServiceProgram();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [sortBy, setSortBy] = useState("name");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    serviceProgram: ServiceProgram | null;
  }>({ isOpen: false, serviceProgram: null });

  useEffect(() => {
    setPage(1);
  }, [search, sortBy, sortOrder]);

  const filter = useMemo(
    () => ({
      PageNumber: page,
      PageSize: pageSize,
      Search: search,
      SortBy: sortBy,
      SortDescending: sortOrder === "desc",
    }),
    [page, pageSize, search, sortBy, sortOrder],
  );

  const { servicePrograms, pagination, isPending } = useServicePrograms(filter);

  const handleSort = (sortKey: string) => {
    if (sortBy === sortKey) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(sortKey);
      setSortOrder("asc");
    }
    setPage(1);
  };

  const columns = [
    {
      key: "name",
      header: "Name",
      sortable: true,
      width: "200px",
      render: (item: ServiceProgram) => <div>{item.name}</div>,
    },
    {
      key: "description",
      header: "Description",
      sortable: true,
      render: (item: ServiceProgram) => <div>{item.description || "-"}</div>,
      width: "260px",
    },
    {
      key: "serviceschedulecount",
      header: "# Schedules",
      sortable: false,
      render: (item: ServiceProgram) => <div>{item.serviceScheduleCount}</div>,
      width: "120px",
    },
    {
      key: "assignedvehiclecount",
      header: "# Vehicles",
      sortable: false,
      render: (item: ServiceProgram) => <div>{item.assignedVehicleCount}</div>,
      width: "120px",
    },
    {
      key: "updatedat",
      header: "Updated At",
      sortable: true,
      render: (item: ServiceProgram) => (
        <div>{new Date(item.updatedAt).toLocaleDateString()}</div>
      ),
      width: "120px",
    },
  ];

  const handleRowClick = (row: ServiceProgram) => {
    router.push(`/service-programs/${row.id}`);
  };

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

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Service Programs
        </h1>
        <PrimaryButton onClick={() => router.push("/service-programs/new")}>
          <Plus size={16} />
          Add Service Program
        </PrimaryButton>
      </div>
      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search"
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
        onRowClick={handleRowClick}
        loading={isPending}
        emptyState={emptyState}
        getItemId={item => item.id.toString()}
        showActions={true}
        actions={serviceProgramActions}
        fixedLayout={false}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
      />

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
