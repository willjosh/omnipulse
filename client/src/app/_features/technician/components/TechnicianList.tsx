"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/app/_features/shared/table";
import { OptionButton, PrimaryButton } from "@/app/_features/shared/button";

import { FilterBar } from "@/app/_features/shared/filter";
import { technicianTableColumns } from "./TechnicianTableColumns";
import { useTechnicians } from "@/app/_hooks/technician/useTechnicians";
import { Technician } from "@/app/_hooks/technician/technicianType";

const TechnicianList: React.FC = () => {
  const router = useRouter();
  const [selectedTechnicians, setSelectedTechnicians] = useState<string[]>([]);
  const [filters, setFilters] = useState({
    page: 1,
    pageSize: 10,
    sortBy: "FirstName",
    sortOrder: "asc" as "asc" | "desc",
    search: "",
  });

  const { technicians, pagination, isPending, isError } =
    useTechnicians(filters);

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
      sortBy: sortKey,
      sortOrder:
        prev.sortBy === sortKey && prev.sortOrder === "asc" ? "desc" : "asc",
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
    setFilters(prev => ({ ...prev, pageSize: newPageSize, page: 1 }));
  };

  const handleRowClick = (technician: Technician) => {};

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
          searchValue={filters.search}
          onSearchChange={handleSearch}
          searchPlaceholder="Search technicians"
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

      <DataTable<Technician>
        data={technicians || []}
        columns={technicianTableColumns}
        selectedItems={selectedTechnicians}
        onSelectItem={handleTechnicianSelect}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        actions={[]}
        showActions={true}
        fixedLayout={false}
        loading={isPending}
        getItemId={technician => technician.id}
        emptyState={emptyState}
      />
    </div>
  );
};

export default TechnicianList;
