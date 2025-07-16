"use client";

import React, { useState } from "react";
import { useInventoryItems } from "@/app/_hooks/inventory-item/useInventoryItem";
import { InventoryItemWithLabels } from "@/app/_hooks/inventory-item/inventoryItemType";
import { Package } from "lucide-react";
import { Loading } from "@/app/_features/shared/feedback";
import { DataTable, PaginationControls } from "@/app/_features/shared/table";
import { FilterBar } from "@/app/_features/shared/filter";
import { inventoryTableColumns } from "./InventoryTableColumns";

const InventoryList = () => {
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [filters, setFilters] = useState({
    page: 1,
    pageSize: 10,
    sortBy: "ItemName",
    sortOrder: "asc" as "asc" | "desc",
    search: "",
  });

  const { inventoryItems, pagination, isPending, isError } =
    useInventoryItems(filters);

  const handleSelectAll = () => {
    if (!inventoryItems) return;

    const allItemIds = inventoryItems.map(item => item.id.toString());
    const allSelected = allItemIds.every(id => selectedItems.includes(id));

    if (allSelected) {
      setSelectedItems([]);
    } else {
      setSelectedItems(allItemIds);
    }
  };

  const handleItemSelect = (itemId: string) => {
    setSelectedItems(prev =>
      prev.includes(itemId)
        ? prev.filter(id => id !== itemId)
        : [...prev, itemId],
    );
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

  const handleSort = (sortKey: string) => {
    setFilters(prev => ({
      ...prev,
      sortBy: sortKey,
      sortOrder:
        prev.sortBy === sortKey && prev.sortOrder === "asc" ? "desc" : "asc",
      page: 1, // Reset to first page when sorting
    }));
  };

  if (isPending) {
    return <Loading />;
  }

  if (isError) {
    return (
      <div className="flex-1 p-6">
        <div className="text-center py-8">
          <Package className="w-12 h-12 text-red-400 mx-auto mb-4" />
          <p className="text-red-500">Error loading inventory items</p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex-1 p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <Package className="w-8 h-8 text-[var(--primary-color)]" />
          <h1 className="text-2xl font-bold text-gray-900">Parts Inventory</h1>
        </div>
      </div>

      <div className="flex items-center justify-between mb-4">
        <FilterBar
          searchValue={filters.search}
          onSearchChange={handleSearch}
          searchPlaceholder="Search inventory"
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

      {/* Inventory Items Table */}
      <DataTable<InventoryItemWithLabels>
        data={inventoryItems || []}
        columns={inventoryTableColumns}
        selectedItems={selectedItems}
        onSelectItem={handleItemSelect}
        onSelectAll={handleSelectAll}
        actions={[]}
        showActions={false}
        loading={isPending}
        fixedLayout={false}
        getItemId={item =>
          item.id ? item.id.toString() : `temp-${Date.now()}`
        }
        emptyState={
          <div className="text-center py-8">
            <Package className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500">No inventory items found</p>
          </div>
        }
      />
    </div>
  );
};

export default InventoryList;
