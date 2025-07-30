"use client";
import React, { useState, useMemo } from "react";
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

const TechnicianList: React.FC = () => {
  const router = useRouter();
  const [selectedTechnicians, setSelectedTechnicians] = useState<string[]>([]);
  const [filters, setFilters] = useState({
    PageNumber: 1,
    PageSize: 10,
    SortBy: "firstName",
    SortDescending: false,
    Search: "",
  });
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    technician?: Technician;
  }>({ isOpen: false });

  const { technicians, pagination, isPending, isError } =
    useTechnicians(filters);
  const deactivateTechnicianMutation = useDeactivateTechnician();

  const handleSelectAll = () => {
    if (selectedTechnicians.length === technicians.length) {
      setSelectedTechnicians([]);
    } else {
      setSelectedTechnicians(technicians.map(t => t.id));
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
    setFilters(prev => ({
      ...prev,
      SortBy: sortKey,
      SortDescending: prev.SortBy === sortKey ? !prev.SortDescending : false,
    }));
  };

  const handleSearch = (searchTerm: string) => {
    setFilters(prev => ({
      ...prev,
      Search: searchTerm,
      PageNumber: 1, // Reset to first page when searching
    }));
  };

  const handlePageChange = (newPage: number) => {
    setFilters(prev => ({ ...prev, PageNumber: newPage }));
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setFilters(prev => ({ ...prev, PageSize: newPageSize, PageNumber: 1 }));
  };

  const handleRowClick = (technician: Technician) => {
    router.push(`/contacts/${technician.id}`);
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
          router.push(`/contacts/${tech.id}`);
        },
      },
      {
        key: "edit",
        label: "Edit Technician",
        icon: <Edit />,
        onClick: (tech: Technician) => {
          router.push(`/contacts/${tech.id}/edit`);
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
        onClick={() => handleSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear filters
      </button>
    </div>
  );

  return (
    <div className="p-6 w-[1260px] min-h-screen mx-auto">
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-semibold text-gray-900">Technicians</h1>
        <div className="flex items-center gap-3">
          <OptionButton />
          <PrimaryButton onClick={() => router.push("/contacts/create")}>
            <span>+</span>
            Add Technician
          </PrimaryButton>
        </div>
      </div>

      <div className="flex items-center justify-between mb-4">
        <FilterBar
          searchValue={filters.Search}
          onSearchChange={handleSearch}
          searchPlaceholder="Search technicians"
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

      <DataTable<Technician>
        data={technicians || []}
        columns={technicianTableColumns}
        selectedItems={selectedTechnicians}
        onSelectItem={handleTechnicianSelect}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        onSort={handleSort}
        sortBy={filters.SortBy}
        sortOrder={filters.SortDescending ? "desc" : "asc"}
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
