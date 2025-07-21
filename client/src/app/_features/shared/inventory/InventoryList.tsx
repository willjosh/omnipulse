"use client";

import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import {
  useInventoryItems,
  useDeactivateInventoryItem,
} from "@/app/_hooks/inventory-item/useInventoryItem";
import { InventoryItemWithLabels } from "@/app/_hooks/inventory-item/inventoryItemType";
import { Loading } from "@/app/_features/shared/feedback";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import { DataTable, PaginationControls } from "@/app/_features/shared/table";
import { FilterBar } from "@/app/_features/shared/filter";
import { PrimaryButton, OptionButton } from "@/app/_features/shared/button";
import { ConfirmModal } from "@/app/_features/shared/modal";
import { Archive, Edit, Details } from "@/app/_features/shared/icons";
import { inventoryTableColumns } from "./InventoryTableColumns";
import InventoryModal from "./InventoryModal";
import {
  InventoryActionType,
  INVENTORY_ACTION_CONFIG,
} from "./inventoryActions";

const InventoryList = () => {
  const router = useRouter();
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [modal, setModal] = useState<{
    isOpen: boolean;
    mode: "create" | "edit";
    item?: InventoryItemWithLabels;
  }>({ isOpen: false, mode: "create" });
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    item?: InventoryItemWithLabels;
  }>({ isOpen: false });
  const [filters, setFilters] = useState({
    page: 1,
    pageSize: 10,
    sortBy: "ItemName",
    sortOrder: "asc" as "asc" | "desc",
    search: "",
  });

  const { inventoryItems, pagination, isPending, isError } =
    useInventoryItems(filters);
  const deactivateInventoryMutation = useDeactivateInventoryItem();

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
      page: 1,
    }));
  };

  const handleSearch = (searchTerm: string) => {
    setFilters(prev => ({ ...prev, search: searchTerm, page: 1 }));
  };

  const handlePageChange = (newPage: number) => {
    setFilters(prev => ({ ...prev, page: newPage }));
  };

  const handleRowClick = (item: InventoryItemWithLabels) => {
    router.push(`/parts-inventory/${item.id}`);
  };

  const handleArchiveItem = async () => {
    if (!confirmModal.item) return;

    try {
      const itemId = confirmModal.item.id;
      await deactivateInventoryMutation.mutateAsync(itemId);
      setConfirmModal({ isOpen: false });
    } catch (error) {
      console.error("Error archiving inventory item:", error);
    }
  };

  const inventoryActions = useMemo(
    () => [
      {
        key: InventoryActionType.VIEW,
        label: INVENTORY_ACTION_CONFIG[InventoryActionType.VIEW].label,
        icon: <Details />,
        onClick: (item: InventoryItemWithLabels) => {
          router.push(`/parts-inventory/${item.id}`);
        },
      },
      {
        key: InventoryActionType.EDIT,
        label: INVENTORY_ACTION_CONFIG[InventoryActionType.EDIT].label,
        icon: <Edit />,
        onClick: (item: InventoryItemWithLabels) => {
          setModal({ isOpen: true, mode: "edit", item });
        },
      },
      {
        key: InventoryActionType.ARCHIVE,
        label: INVENTORY_ACTION_CONFIG[InventoryActionType.ARCHIVE].label,
        variant: INVENTORY_ACTION_CONFIG[InventoryActionType.ARCHIVE].variant,
        icon: <Archive />,
        onClick: (item: InventoryItemWithLabels) => {
          setConfirmModal({ isOpen: true, item });
        },
      },
    ],
    [router],
  );

  if (isError) {
    return (
      <div className="p-6 w-[1260px] min-h-screen mx-auto">
        <EmptyState
          icon="⚠️"
          title="Error Loading Inventory"
          message="Unable to load inventory items. Please check your connection and try again."
          className="text-red-500"
        />
      </div>
    );
  }

  const emptyState = (
    <EmptyState
      icon="📦"
      title="No Inventory Items Found"
      message="Get started by adding your first inventory item to track your parts and supplies."
      action={
        <PrimaryButton
          onClick={() => setModal({ isOpen: true, mode: "create" })}
        >
          <span>+</span>
          Add Your First Item
        </PrimaryButton>
      }
    />
  );

  return (
    <div className="p-6 w-[1260px] min-h-screen mx-auto">
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-semibold text-gray-900">
          Parts & Inventory
        </h1>
        <div className="flex items-center gap-3">
          <OptionButton />
          <PrimaryButton
            onClick={() => setModal({ isOpen: true, mode: "create" })}
          >
            <span>+</span>
            Add Item
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
          actions={inventoryActions}
          showActions={true}
          fixedLayout={false}
          loading={isPending}
          getItemId={item => item.id.toString()}
          emptyState={emptyState}
        />
      )}

      <InventoryModal
        isOpen={modal.isOpen}
        onClose={() => setModal({ isOpen: false, mode: "create" })}
        mode={modal.mode}
        item={modal.item}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false })}
        onConfirm={handleArchiveItem}
        title="Archive Item"
        message={`Are you sure you want to archive ${confirmModal.item?.ItemName}?`}
        confirmText="Archive"
        cancelText="Cancel"
      />
    </div>
  );
};

export default InventoryList;
