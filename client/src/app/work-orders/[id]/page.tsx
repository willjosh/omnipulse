"use client";

import { Box, Container } from "@mui/material";
import { notFound, useParams } from "next/navigation";
import WorkOrderHeader from "@/app/_features/work-order/components/WorkOrderDetailsHeader";
import { WorkOrderDetailsPanel } from "@/app/_features/work-order/components/WorkOrderDetailsPanel";

export default function WorkOrderPage() {
  const params = useParams();
  const workOrderId = Number(params.id);
  if (Number.isNaN(workOrderId)) {
    notFound();
  }

  return (
    <Box>
      <WorkOrderHeader workOrderId={workOrderId} />
      <Box sx={{ bgcolor: "#f8f9fa", minHeight: "100vh", py: 3 }}>
        <Container maxWidth="xl">
          <Box display="flex" gap={3}>
            <WorkOrderDetailsPanel source="latest" />
          </Box>
        </Container>
      </Box>
    </Box>
  );
}
