"use client";
import { useParams, useRouter } from "next/navigation";
import TechnicianFormContainer from "@/app/_features/technician/components/forms/TechnicianFormContainer";
import { useTechnician } from "@/app/_hooks/technician/useTechnicians";
import { Loading } from "@/app/_features/shared/feedback";

const EditTechnicianPage = () => {
  const params = useParams();
  const router = useRouter();
  const technicianId = params.id as string;

  const { data: technician, isLoading: isLoadingTechnician } =
    useTechnician(technicianId);

  if (isLoadingTechnician) {
    return <Loading />;
  }

  if (!technician) {
    router.push("/contacts");
    return <Loading />;
  }

  return <TechnicianFormContainer mode="edit" technicianId={technicianId} />;
};

export default EditTechnicianPage;
