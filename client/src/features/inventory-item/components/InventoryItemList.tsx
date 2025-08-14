"use client";

import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import {
  useInventoryItems,
  useDeleteInventoryItem,
} from "../hooks/useInventoryItem";
import { InventoryItemWithLabels } from "../types/inventoryItemType";
import { Loading } from "@/components/ui/Feedback";
import EmptyState from "@/components/ui/Feedback/EmptyState";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { FilterBar } from "@/components/ui/Filter";
import { PrimaryButton, OptionButton } from "@/components/ui/Button";
import { ConfirmModal } from "@/components/ui/Modal";
import { Archive, Edit, Details } from "@/components/ui/Icons";
import { inventoryTableColumns } from "../config/InventoryItemTableColumns";
import InventoryModal from "./InventoryItemModal";
import {
  InventoryActionType,
  INVENTORY_ACTION_CONFIG,
} from "../config/inventoryItemActions";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { Plus } from "lucide-react";

const InventoryItemList = () => {
  const router = useRouter();
  const notify = useNotification();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("itemName");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
  const [modal, setModal] = useState<{
    isOpen: boolean;
    mode: "create" | "edit";
    item?: InventoryItemWithLabels;
  }>({ isOpen: false, mode: "create" });
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    item?: InventoryItemWithLabels;
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

  const { inventoryItems, pagination, isPending, isError } =
    useInventoryItems(filters);
  const deleteInventoryMutation = useDeleteInventoryItem();

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

  const handleRowClick = (item: InventoryItemWithLabels) => {
    router.push(`/inventory-item/${item.id}`);
  };

  const handleArchiveItem = async () => {
    if (!confirmModal.item) return;

    try {
      const itemId = confirmModal.item.id;
      await deleteInventoryMutation.mutateAsync(itemId);
      setConfirmModal({ isOpen: false });
      notify("Inventory item archived successfully", "success");
    } catch (error) {
      console.error("Error archiving inventory item:", error);
      notify("Failed to archive inventory item", "error");
    }
  };

  const inventoryItemAction = useMemo(
    () => [
      {
        key: InventoryActionType.VIEW,
        label: INVENTORY_ACTION_CONFIG[InventoryActionType.VIEW].label,
        icon: <Details />,
        onClick: (item: InventoryItemWithLabels) => {
          router.push(`/inventory-item/${item.id}`);
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
      <div className="p-6 w-full max-w-none">
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
          <Plus size={16} />
          Add Your First Item
        </PrimaryButton>
      }
    />
  );

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Inventory Items
        </h1>
        <div className="flex items-center gap-3">
          <OptionButton />
          <PrimaryButton
            onClick={() => setModal({ isOpen: true, mode: "create" })}
          >
            <Plus size={16} />
            Add Item
          </PrimaryButton>
        </div>
      </div>

      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search"
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
        <DataTable<InventoryItemWithLabels>
          data={inventoryItems || []}
          columns={inventoryTableColumns}
          onRowClick={handleRowClick}
          onSort={handleSort}
          sortBy={sortBy}
          sortOrder={sortOrder}
          actions={inventoryItemAction}
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

export default InventoryItemList;
