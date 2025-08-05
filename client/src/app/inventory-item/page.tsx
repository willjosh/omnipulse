import React from "react";
import InventoryList from "../../features/inventory-item/components/InventoryItemList";

const page = () => {
  return (
    <div className="flex justify-center h-screen overflow-hidden">
      <InventoryList />
    </div>
  );
};

export default page;
