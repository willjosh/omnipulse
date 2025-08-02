"use client";

import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import {
  useInventoryItems,
  useDeleteInventoryItem,
} from "../hooks/useInventoryItems";
import { InventoryItemWithLabels } from "../types/inventoryItemType";
import { Loading } from "@/components/ui/Feedback";
import EmptyState from "@/components/ui/Feedback/EmptyState";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { FilterBar } from "@/components/ui/Filter";
import { PrimaryButton, OptionButton } from "@/components/ui/Button";
import { ConfirmModal } from "@/components/ui/Modal";
import { Archive, Edit, Details } from "@/components/ui/Icons";
import { inventoryTableColumns } from "../config/InventoryTableColumns";
import InventoryModal from "./InventoryModal";
import {
  InventoryActionType,
  INVENTORY_ACTION_CONFIG,
} from "../config/inventoryActions";

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
    PageNumber: 1,
    PageSize: 10,
    SortBy: "itemName",
    SortDescending: false,
    Search: "",
  });

  const { inventoryItems, pagination, isPending, isError } =
    useInventoryItems(filters);
  const deleteInventoryMutation = useDeleteInventoryItem();

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

  const handleRowClick = (item: InventoryItemWithLabels) => {
    router.push(`/parts-inventory/${item.id}`);
  };

  const handleArchiveItem = async () => {
    if (!confirmModal.item) return;

    try {
      const itemId = confirmModal.item.id;
      await deleteInventoryMutation.mutateAsync(itemId);
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
      <div className="p-6 w-full max-w-none min-h-screen">
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
    <div className="p-6 w-full max-w-none min-h-screen">
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
        message={`Are you sure you want to archive ${confirmModal.item?.itemName}?`}
        confirmText="Archive"
        cancelText="Cancel"
      />
    </div>
  );
};

export default InventoryList;
