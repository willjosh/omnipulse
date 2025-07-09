"use client";
import { useParams, useRouter } from "next/navigation";
import VehicleFormContainer from "@/app/_features/vehicle/components/forms/VehicleFormContainer";
import { useVehicles } from "@/app/_hooks/vehicle/useVehicles";
import Loading from "@/app/_features/shared/Loading";

const EditVehiclePage = () => {
  const params = useParams();
  const router = useRouter();
  const vehicleId = params.id as string;

  const { vehicle, isLoadingVehicle } = useVehicles(undefined, vehicleId);

  if (isLoadingVehicle) {
    return <Loading />;
  }

  if (!vehicle) {
    router.push("/vehicles");
    return <Loading />;
  }

  return <VehicleFormContainer mode="edit" vehicleData={vehicle} />;
};

export default EditVehiclePage;
