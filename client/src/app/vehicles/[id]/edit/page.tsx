"use client";
import { useParams, useRouter } from "next/navigation";
import VehicleFormContainer from "@/features/vehicle/components/VehicleFormContainer";
import { useVehicle } from "@/features/vehicle/hooks/useVehicles";
import { Loading } from "@/components/ui/Feedback";

const EditVehiclePage = () => {
  const params = useParams();
  const router = useRouter();
  const vehicleId = params.id as string;

  const { vehicle, isPending } = useVehicle(vehicleId);

  if (isPending) {
    return <Loading />;
  }

  if (!vehicle) {
    router.push("/vehicles");
    return <Loading />;
  }

  return <VehicleFormContainer mode="edit" vehicleData={vehicle} />;
};

export default EditVehiclePage;
