"use client";

import { Box, Container } from "@mui/material";
import { notFound, useParams, useRouter } from "next/navigation";
import WorkOrderDetailsHeader from "@/features/work-order/components/WorkOrderDetailsHeader";
import { WorkOrderDetailsPanel } from "@/features/work-order/components/WorkOrderDetailsPanel";
import { useWorkOrder } from "@/features/work-order/hooks/useWorkOrders";

export default function WorkOrderPage() {
  const router = useRouter();
  const params = useParams();
  const workOrderId = Number(params.id);

  if (Number.isNaN(workOrderId)) {
    notFound();
  }

  const { workOrder, isPending, isError } = useWorkOrder(workOrderId);

  if (isPending) {
    return <div>Loading...</div>;
  }

  if (isError || !workOrder) {
    return <div>Error loading work order</div>;
  }

  const handleStatusChange = (status: string) => {
    // TODO: Implement status change logic
    console.log("Status changed to:", status);
  };

  const handleEdit = () => {
    router.push(`/work-orders/${workOrderId}/edit`);
  };

  const handleDelete = () => {
    // TODO: Implement delete logic
    console.log("Delete work order");
  };

  return (
    <Box>
      <WorkOrderDetailsHeader
        workOrder={workOrder}
        onStatusChange={handleStatusChange}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />
      <Box sx={{ bgcolor: "#f8f9fa", minHeight: "100vh", py: 3 }}>
        <Container maxWidth="xl">
          <Box display="flex" gap={3}>
            <WorkOrderDetailsPanel source="list" workOrderId={workOrderId} />
          </Box>
        </Container>
      </Box>
    </Box>
  );
}
