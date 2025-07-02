import { create } from "zustand";
import { WorkOrderFormData } from "../types/workOrderFormTypes";

interface WorkOrder {
  id: number;
  number: number;
  data: WorkOrderFormData;
}

interface WorkOrderListStore {
  workOrders: WorkOrder[];
  addWorkOrder: (data: WorkOrderFormData) => void;
}

let idCounter = 1;

export const useWorkOrderListStore = create<WorkOrderListStore>(set => ({
  workOrders: [],
  addWorkOrder: data =>
    set(state => {
      const newOrder = {
        id: idCounter,
        number: state.workOrders.length + 1,
        data,
      };
      idCounter += 1;
      return { workOrders: [...state.workOrders, newOrder] };
    }),
}));
