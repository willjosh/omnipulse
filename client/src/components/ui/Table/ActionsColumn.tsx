import React from "react";
import ActionDropdown, { ActionItem } from "./ActionDropdown";

interface ActionsColumnProps {
  data: any[];
  actions: ActionItem[];
  onActionClick: (
    action: ActionItem,
    item: any,
    event: React.MouseEvent,
  ) => void;
}

export const ActionsColumnHeader: React.FC = () => (
  <th className="px-2 py-3 text-left w-12" style={{ width: "48px" }}></th>
);

export const ActionsColumnCell: React.FC<{
  item: any;
  actions: ActionItem[];
  onActionClick: (
    action: ActionItem,
    item: any,
    event: React.MouseEvent,
  ) => void;
}> = ({ item, actions, onActionClick }) => (
  <td
    className="px-2 py-3 whitespace-nowrap text-right text-sm font-medium w-12"
    style={{ width: "48px" }}
    onClick={e => e.stopPropagation()}
  >
    <ActionDropdown
      item={item}
      actions={actions}
      onActionClick={onActionClick}
    />
  </td>
);

const ActionsColumn: React.FC<ActionsColumnProps> = ({
  data,
  actions,
  onActionClick,
}) => {
  return (
    <>
      {data.map(item => (
        <ActionsColumnCell
          key={item.id}
          item={item}
          actions={actions}
          onActionClick={onActionClick}
        />
      ))}
    </>
  );
};

export default ActionsColumn;
