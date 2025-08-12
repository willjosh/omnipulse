"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { OptionButton, PrimaryButton } from "@/components/ui/Button";
import { ConfirmModal } from "@/components/ui/Modal";
import { FilterBar } from "@/components/ui/Filter";
import { Archive, Edit, Details } from "@/components/ui/Icons";
import { technicianTableColumns } from "../config/TechnicianTableColumns";
import {
  useTechnicians,
  useDeactivateTechnician,
} from "../hooks/useTechnicians";
import { Technician } from "../types/technicianType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";

const TechnicianList: React.FC = () => {
  const router = useRouter();
  const [selectedTechnicians, setSelectedTechnicians] = useState<string[]>([]);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("firstName");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    technician?: Technician;
  }>({ isOpen: false });

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

  const { technicians, pagination, isPending, isError } =
    useTechnicians(filters);
  const deactivateTechnicianMutation = useDeactivateTechnician();

  const handleSelectAll = () => {
    if (!technicians) return;
    const allTechnicianIds = technicians.map(technician => technician.id);
    const allSelected = allTechnicianIds.every(id =>
      selectedTechnicians.includes(id),
    );
    if (allSelected) {
      setSelectedTechnicians([]);
    } else {
      setSelectedTechnicians(allTechnicianIds);
    }
  };

  const handleTechnicianSelect = (technicianId: string) => {
    setSelectedTechnicians(prev =>
      prev.includes(technicianId)
        ? prev.filter(id => id !== technicianId)
        : [...prev, technicianId],
    );
  };

  const handleSort = (sortKey: string) => {
    if (sortBy === sortKey) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(sortKey);
      setSortOrder("asc");
    }
    setPage(1);
  };

  const handleSearch = (searchTerm: string) => {
    setSearch(searchTerm);
  };

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(1);
  };

  const handleRowClick = (technician: Technician) => {
    router.push(`/technician/${technician.id}`);
  };

  const handleToggleTechnicianStatus = async () => {
    if (!confirmModal.technician) return;
    try {
      await deactivateTechnicianMutation.mutateAsync(
        confirmModal.technician.id,
      );
      setConfirmModal({ isOpen: false });
    } catch (error) {
      console.error("Error deactivating technician:", error);
    }
  };

  const getTechnicianActions = useMemo(
    () => (technician: Technician) => [
      {
        key: "view",
        label: "View Details",
        icon: <Details />,
        onClick: (tech: Technician) => {
          router.push(`/technician/${tech.id}`);
        },
      },
      {
        key: "edit",
        label: "Edit Technician",
        icon: <Edit />,
        onClick: (tech: Technician) => {
          router.push(`/technician/${tech.id}/edit`);
        },
      },
      {
        key: technician.isActive ? "deactivate" : "activate",
        label: technician.isActive ? "Deactivate" : "Activate",
        variant: technician.isActive
          ? ("danger" as const)
          : ("default" as const),
        icon: <Archive />,
        onClick: (tech: Technician) => {
          setConfirmModal({ isOpen: true, technician: tech });
        },
      },
    ],
    [router],
  );

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No technicians found.</p>
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
        <h1 className="text-2xl font-semibold text-gray-900">Technicians</h1>
        <div className="flex items-center gap-3">
          <OptionButton />
          <PrimaryButton onClick={() => router.push("/technician/create")}>
            <span>+</span>
            Add Technician
          </PrimaryButton>
        </div>
      </div>

      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          onFilterChange={() => {}}
          searchPlaceholder="Search"
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

      <DataTable<Technician>
        data={technicians || []}
        columns={technicianTableColumns}
        selectedItems={selectedTechnicians}
        onSelectItem={handleTechnicianSelect}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
        actions={getTechnicianActions}
        showActions={true}
        fixedLayout={false}
        loading={isPending}
        getItemId={technician => technician.id}
        emptyState={emptyState}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleToggleTechnicianStatus}
        title={`${confirmModal.technician?.isActive ? "Deactivate" : "Activate"} Technician`}
        message={`Are you sure you want to ${confirmModal.technician?.isActive ? "deactivate" : "activate"} ${confirmModal.technician?.firstName} ${confirmModal.technician?.lastName}?`}
        confirmText={
          confirmModal.technician?.isActive ? "Deactivate" : "Activate"
        }
        cancelText="Cancel"
      />
    </div>
  );
};

export default TechnicianList;
