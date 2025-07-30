"use client";
import { useParams, useRouter } from "next/navigation";
import TechnicianFormContainer from "@/features/technician/components/TechnicianFormContainer";
import { useTechnician } from "@/features/technician/hooks/useTechnicians";
import { Loading } from "@/components/ui/Feedback";

const EditTechnicianPage = () => {
  const params = useParams();
  const router = useRouter();
  const technicianId = params.id as string;

  const { technician, isPending: isLoadingTechnician } =
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
