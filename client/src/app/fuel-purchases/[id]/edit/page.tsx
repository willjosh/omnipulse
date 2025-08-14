"use client";
import { useParams, useRouter } from "next/navigation";
import FuelPurchaseFormContainer from "@/features/fuel-purchases/components/FuelPurchaseFormContainer";
import { useFuelPurchase } from "@/features/fuel-purchases/hooks/useFuelPurchases";
import { Loading } from "@/components/ui/Feedback";

const EditFuelPurchasePage = () => {
  const params = useParams();
  const router = useRouter();
  const fuelPurchaseId = params.id as string;

  const { fuelPurchase, isPending: isLoadingFuelPurchase } =
    useFuelPurchase(fuelPurchaseId);

  if (isLoadingFuelPurchase) {
    return <Loading />;
  }

  if (!fuelPurchase) {
    router.push("/fuel-purchases");
    return <Loading />;
  }

  return (
    <FuelPurchaseFormContainer mode="edit" fuelPurchaseId={fuelPurchaseId} />
  );
};

export default EditFuelPurchasePage;
