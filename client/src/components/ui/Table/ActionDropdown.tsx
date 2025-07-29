import React, { useState } from "react";
import { OptionButton } from "../Button";

export interface ActionItem {
  key: string;
  label: string;
  icon?: React.ReactNode;
  onClick: (item: any) => void;
  variant?: "default" | "danger";
}

interface ActionDropdownProps {
  item: any;
  actions: ActionItem[];
  onActionClick: (
    action: ActionItem,
    item: any,
    event: React.MouseEvent,
  ) => void;
}

const ActionDropdown: React.FC<ActionDropdownProps> = ({
  item,
  actions,
  onActionClick,
}) => {
  const [isOpen, setIsOpen] = useState(false);

  const handleToggle = () => {
    setIsOpen(!isOpen);
  };

  const handleActionClick = (action: ActionItem, event: React.MouseEvent) => {
    event.stopPropagation();
    setIsOpen(false);
    onActionClick(action, item, event);
  };

  return (
    <>
      <div className="relative" onClick={e => e.stopPropagation()}>
        <div onClick={e => e.stopPropagation()}>
          <OptionButton
            onClick={handleToggle}
            className="p-1 rounded-full hover:bg-gray-100 transition-colors"
          />
        </div>

        {isOpen && actions.length > 0 && (
          <div
            className="absolute right-0 mt-1 w-44 bg-white rounded-md shadow-lg border border-gray-200 z-30"
            onClick={e => e.stopPropagation()}
          >
            <div className="py-1">
              {actions.map(action => (
                <button
                  key={action.key}
                  onClick={e => handleActionClick(action, e)}
                  className={`flex items-center w-full text-left px-3 py-2 text-sm hover:bg-gray-100 transition-colors ${
                    action.variant === "danger"
                      ? "text-red-600 hover:bg-red-50"
                      : "text-gray-700"
                  }`}
                >
                  {action.icon && (
                    <span className="mr-2 size-4 flex-shrink-0">
                      {action.icon}
                    </span>
                  )}
                  <span className="truncate">{action.label}</span>
                </button>
              ))}
            </div>
          </div>
        )}
      </div>

      {isOpen && (
        <div
          className="fixed inset-0 z-20"
          onClick={e => {
            e.stopPropagation();
            setIsOpen(false);
          }}
        />
      )}
    </>
  );
};

export default ActionDropdown;
