"use client";

import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { useInventories, useDeleteInventory } from "../hooks/useInventory";
import { Inventory } from "../types/inventoryType";
import { Loading } from "@/components/ui/Feedback";
import EmptyState from "@/components/ui/Feedback/EmptyState";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { FilterBar } from "@/components/ui/Filter";
import { PrimaryButton, OptionButton } from "@/components/ui/Button";
import { ConfirmModal } from "@/components/ui/Modal";
import { inventoryTableColumns } from "../config/inventoryTableColumns";
import {
  InventoryActionType,
  INVENTORY_ACTION_CONFIG,
} from "../config/inventoryActions";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { Details, Edit, Archive } from "@/components/ui/Icons";

const InventoryList = () => {
  const router = useRouter();
  const notify = useNotification();
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    item?: Inventory;
  }>({ isOpen: false });
  const [filters, setFilters] = useState({
    PageNumber: 1,
    PageSize: 10,
    SortBy: "id",
    SortDescending: false,
    Search: "",
  });

  const { inventories, pagination, isPending, isError } =
    useInventories(filters);
  const deleteInventoryMutation = useDeleteInventory();
  const handleSelectAll = () => {
    if (!inventories) return;

    const allItemIds = inventories.map(item => item.id.toString());
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
      SortBy: sortKey,
      SortDescending: prev.SortBy === sortKey ? !prev.SortDescending : false,
      PageNumber: 1,
    }));
  };

  const handleSearch = (searchTerm: string) => {
    setFilters(prev => ({ ...prev, Search: searchTerm, PageNumber: 1 }));
  };

  const handlePageChange = (newPage: number) => {
    setFilters(prev => ({ ...prev, PageNumber: newPage }));
  };

  const handleRowClick = (item: Inventory) => {
    console.log("View inventory:", item.id);
  };

  const handleArchiveItem = async () => {
    if (!confirmModal.item) return;

    try {
      const itemId = confirmModal.item.id;
      await deleteInventoryMutation.mutateAsync(itemId);
      setConfirmModal({ isOpen: false });
      notify("Inventory archived successfully", "success");
    } catch (error) {
      console.error("Error archiving inventory:", error);
      notify("Failed to archive inventory", "error");
    }
  };

  const inventoryActions = useMemo(
    () => [
      // {
      //   key: InventoryActionType.VIEW,
      //   label: INVENTORY_ACTION_CONFIG[InventoryActionType.VIEW].label,
      //   icon: <Details />,
      //   onClick: (item: Inventory) => {
      //     console.log("View inventory:", item.id);
      //     notify(
      //       "Viewing Inventory Details - Future Implementation",
      //       "success",
      //     );
      //   },
      // },
      {
        key: InventoryActionType.EDIT,
        label: INVENTORY_ACTION_CONFIG[InventoryActionType.EDIT].label,
        icon: <Edit />,
        onClick: (item: Inventory) => {
          router.push(`/inventory/${item.id}/edit`);
        },
      },
      {
        key: InventoryActionType.ARCHIVE,
        label: INVENTORY_ACTION_CONFIG[InventoryActionType.ARCHIVE].label,
        variant: INVENTORY_ACTION_CONFIG[InventoryActionType.ARCHIVE].variant,
        icon: <Archive />,
        onClick: (item: Inventory) => {
          setConfirmModal({ isOpen: true, item });
        },
      },
    ],
    [],
  );

  if (isError) {
    return (
      <div className="p-6 w-full max-w-none">
        <EmptyState
          icon="⚠️"
          title="Error Loading Inventory"
          message="Unable to load inventory. Please check your connection and try again."
          className="text-red-500"
        />
      </div>
    );
  }

  const emptyState = (
    <EmptyState
      icon="📦"
      title="No Inventory Found"
      message="Track your inventory levels, stock locations, and reorder points here."
      action={
        <PrimaryButton
          onClick={() => setFilters(prev => ({ ...prev, Search: "" }))}
        >
          Clear Search
        </PrimaryButton>
      }
    />
  );

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-semibold text-gray-900">Inventory</h1>
        <div className="flex items-center gap-3">
          <OptionButton />
        </div>
      </div>

      <div className="flex items-center justify-between mb-4">
        <FilterBar
          searchValue={filters.Search}
          onSearchChange={handleSearch}
          searchPlaceholder="Search inventory"
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

      {isPending ? (
        <Loading />
      ) : (
        <DataTable<Inventory>
          data={inventories || []}
          columns={inventoryTableColumns}
          selectedItems={selectedItems}
          onSelectItem={handleItemSelect}
          onSelectAll={handleSelectAll}
          onRowClick={handleRowClick}
          onSort={handleSort}
          sortBy={filters.SortBy}
          sortOrder={filters.SortDescending ? "desc" : "asc"}
          actions={inventoryActions}
          showActions={true}
          fixedLayout={false}
          loading={isPending}
          getItemId={item => {
            // Use the most unique fields available
            const name = item?.inventoryItemName || "unknown";
            const cost = item?.unitCost || 0;
            const qty = item?.quantityOnHand || 0;
            return `inv-${name}-${cost}-${qty}`;
          }}
          emptyState={emptyState}
        />
      )}

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleArchiveItem}
        title="Archive Inventory"
        message={`Are you sure you want to archive the inventory for ${confirmModal.item?.inventoryItemName}?`}
        confirmText="Archive"
        cancelText="Cancel"
      />
    </div>
  );
};

export default InventoryList;
