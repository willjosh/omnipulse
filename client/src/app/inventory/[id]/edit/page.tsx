"use client";
import { useParams, useRouter } from "next/navigation";
import InventoryFormContainer from "@/features/inventory/components/InventoryFormContainer";
import { useInventory } from "@/features/inventory/hooks/useInventory";
import { Loading } from "@/components/ui/Feedback";

const EditInventoryPage = () => {
  const params = useParams();
  const router = useRouter();
  const inventoryId = parseInt(params.id as string);

  const { inventory, isPending } = useInventory(inventoryId);

  if (isPending) {
    return <Loading />;
  }

  if (!inventory) {
    router.push("/inventory");
    return <Loading />;
  }

  return <InventoryFormContainer mode="edit" inventoryData={inventory} />;
};

export default EditInventoryPage;
