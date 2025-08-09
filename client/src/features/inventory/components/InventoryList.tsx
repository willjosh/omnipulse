"use client";

import React, { useState, useMemo, useEffect } from "react";
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
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";

const InventoryList = () => {
  const router = useRouter();
  const notify = useNotification();
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("id");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    item?: Inventory;
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
    // Page reset is handled by useEffect
  };

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(1);
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
        <PrimaryButton onClick={() => setSearch("")}>
          Clear Search
        </PrimaryButton>
      }
    />
  );

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">Inventory</h1>
        <div className="flex items-center gap-3">
          <OptionButton />
        </div>
      </div>

      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={handleSearch}
          searchPlaceholder="Search inventory..."
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
          sortBy={sortBy}
          sortOrder={sortOrder}
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
