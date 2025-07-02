import { Box } from "@mui/material";
import WorkOrderForm from "@/app/_features/work-order/components/WorkOrderForm";

const NewWorkOrderPage = () => {
  return (
    <Box sx={{ bgcolor: "#f8f9fa", minHeight: "100vh" }}>
      <WorkOrderForm />
    </Box>
  );
};

export default NewWorkOrderPage;
