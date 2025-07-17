"use client";

import React, { useState } from "react";
import { useInventoryItems } from "@/app/_hooks/inventory-item/useInventoryItem";
import { InventoryItemWithLabels } from "@/app/_hooks/inventory-item/inventoryItemType";
import { Loading } from "@/app/_features/shared/feedback";
import { DataTable, PaginationControls } from "@/app/_features/shared/table";
import { FilterBar } from "@/app/_features/shared/filter";
import { PrimaryButton, OptionButton } from "@/app/_features/shared/button";
import { inventoryTableColumns } from "./InventoryTableColumns";
import AddInventoryModal from "./AddInventoryModal";

const InventoryList = () => {
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
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

  const handleRowClick = (item: InventoryItemWithLabels) => {
    // TODO: Navigate to item detail page when implemented
    console.log("Clicked item:", item);
  };

  if (isError) {
    return (
      <div className="p-6 w-[1260px] min-h-screen mx-auto">
        <div className="text-center py-8">
          <div className="w-12 h-12 text-red-400 mx-auto mb-4 flex items-center justify-center">
            ⚠️
          </div>
          <p className="text-red-500">Error loading inventory items</p>
        </div>
      </div>
    );
  }

  const emptyState = (
    <div className="text-center py-8">
      <div className="w-12 h-12 text-gray-400 mx-auto mb-4 flex items-center justify-center">
        📦
      </div>
      <p className="text-gray-500">No inventory items found</p>
    </div>
  );

  return (
    <div className="p-6 w-[1260px] min-h-screen mx-auto">
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-semibold text-gray-900">
          Parts & Inventory
        </h1>
        <div className="flex items-center gap-3">
          <OptionButton />
          <PrimaryButton onClick={() => setIsAddModalOpen(true)}>
            <span>+</span>
            Add Part
          </PrimaryButton>
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

      {isPending ? (
        <Loading />
      ) : (
        <DataTable<InventoryItemWithLabels>
          data={inventoryItems || []}
          columns={inventoryTableColumns}
          selectedItems={selectedItems}
          onSelectItem={handleItemSelect}
          onSelectAll={handleSelectAll}
          onRowClick={handleRowClick}
          showActions={false}
          fixedLayout={false}
          loading={isPending}
          getItemId={item => item.id.toString()}
          emptyState={emptyState}
        />
      )}

      <AddInventoryModal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
      />
    </div>
  );
};

export default InventoryList;
