import React, { useState } from "react";
import { Plus, Edit, Trash2, CheckCircle } from "lucide-react";
import { PrimaryButton } from "@/components/ui/Button";
import {
  useInspectionFormItems,
  useDeactivateInspectionFormItem,
} from "../hooks/useInspectionFormItems";
import { InspectionFormItemWithLabels } from "../types/inspectionFormItemType";
import { InspectionFormItemTypeEnum } from "../types/inspectionFormEnum";
import { getInspectionFormItemTypeLabel } from "../utils/inspectionFormEnumHelper";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";

interface InspectionItemListFormProps {
  inspectionFormId: number;
  onEditItem?: (item: InspectionFormItemWithLabels) => void;
  onAddItem?: () => void;
}

const InspectionItemListForm: React.FC<InspectionItemListFormProps> = ({
  inspectionFormId,
  onEditItem,
  onAddItem,
}) => {
  const notify = useNotification();
  const { inspectionFormItems, isPending, isError, error } =
    useInspectionFormItems(inspectionFormId);
  const { mutate: deactivateItem, isPending: isDeactivating } =
    useDeactivateInspectionFormItem();

  const handleAddItem = () => {
    if (onAddItem) {
      onAddItem();
    }
  };

  const handleEditItem = (item: InspectionFormItemWithLabels) => {
    if (onEditItem) {
      onEditItem(item);
    }
  };

  const handleDeleteItem = (item: InspectionFormItemWithLabels) => {
    if (
      window.confirm(`Are you sure you want to delete "${item.itemLabel}"?`)
    ) {
      deactivateItem(
        { inspectionFormId, itemId: item.id },
        {
          onSuccess: () => {
            notify("Inspection item deleted successfully!", "success");
          },
          onError: error => {
            console.error("Failed to delete inspection item:", error);
            notify("Failed to delete inspection item", "error");
          },
        },
      );
    }
  };

  const getItemTypeIcon = (type: InspectionFormItemTypeEnum) => {
    switch (type) {
      case InspectionFormItemTypeEnum.PassFail:
        return <CheckCircle size={16} className="text-green-600" />;
      default:
        return null;
    }
  };

  if (isPending) {
    return (
      <div className="bg-white p-6 rounded-lg shadow">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold text-gray-900">
            Inspection Items
          </h2>
          <PrimaryButton onClick={handleAddItem} disabled>
            <Plus size={18} className="mr-2" />
            Add Item
          </PrimaryButton>
        </div>
        <div className="text-center py-8">
          <div className="text-gray-500">Loading inspection items...</div>
        </div>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="bg-white p-6 rounded-lg shadow">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold text-gray-900">
            Inspection Items
          </h2>
          <PrimaryButton onClick={handleAddItem}>
            <Plus size={18} className="mr-2" />
            Add Item
          </PrimaryButton>
        </div>
        <div className="text-center py-8">
          <div className="text-red-500">
            Error loading inspection items: {error?.message}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white p-6 rounded-lg shadow">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-semibold text-gray-900">
          Inspection Items
        </h2>
        <PrimaryButton onClick={handleAddItem}>
          <Plus size={18} className="mr-2" />
          Add Item
        </PrimaryButton>
      </div>

      {inspectionFormItems.length === 0 ? (
        <div className="text-center py-8">
          <div className="text-gray-500 mb-2">
            No inspection items added yet.
          </div>
          <div className="text-sm text-gray-400">
            Click "Add Item" to create your first inspection checklist item.
          </div>
        </div>
      ) : (
        <div className="space-y-4">
          {inspectionFormItems.map(item => (
            <div
              key={item.id}
              className="border border-gray-200 rounded-lg p-4 hover:border-gray-300 transition-colors"
            >
              <div className="flex justify-between items-start">
                <div className="flex-1">
                  <div className="flex items-center gap-2 mb-2">
                    <span className="font-medium text-gray-900">
                      {item.itemLabel}
                      {item.isRequired && (
                        <span className="text-red-500 ml-1">*</span>
                      )}
                    </span>
                    <div className="flex items-center gap-1 text-sm text-gray-600">
                      {getItemTypeIcon(item.inspectionFormItemTypeEnum)}
                      <span>
                        {getInspectionFormItemTypeLabel(
                          item.inspectionFormItemTypeEnum,
                        )}
                      </span>
                    </div>
                  </div>

                  {item.itemDescription && (
                    <div className="text-sm text-gray-600 mb-2">
                      {item.itemDescription}
                    </div>
                  )}

                  {item.itemInstructions && (
                    <div className="text-sm text-gray-500">
                      <span className="font-medium">Instructions:</span>{" "}
                      {item.itemInstructions}
                    </div>
                  )}
                </div>

                <div className="flex gap-2 ml-4">
                  <button
                    onClick={() => handleEditItem(item)}
                    className="p-2 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-md transition-colors"
                    title="Edit item"
                  >
                    <Edit size={16} />
                  </button>
                  <button
                    onClick={() => handleDeleteItem(item)}
                    disabled={isDeactivating}
                    className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-md transition-colors disabled:opacity-50"
                    title="Delete item"
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default InspectionItemListForm;
