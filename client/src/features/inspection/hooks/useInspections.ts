import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  inspectionApi,
  convertInspectionData,
  convertSingleInspectionData,
} from "../api/inspectionApi";
import {
  SingleInspectionWithLabels,
  InspectionFilter,
} from "../types/inspectionType";
import { useDebounce } from "@/hooks/useDebounce";

export function useInspections(filter: InspectionFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["inspections", debouncedFilter],
    queryFn: async () => {
      const data = await inspectionApi.getInspections(debouncedFilter);
      return { ...data, items: data.items.map(convertInspectionData) };
    },
  });

  return {
    inspections: data?.items ?? [],
    pagination: data
      ? {
          totalCount: data.totalCount,
          pageNumber: data.pageNumber,
          pageSize: data.pageSize,
          totalPages: data.totalPages,
          hasPreviousPage: data.hasPreviousPage,
          hasNextPage: data.hasNextPage,
        }
      : null,
    isPending,
    isError,
    isSuccess,
    error,
  };
}

export function useInspection(id: number) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<SingleInspectionWithLabels>({
      queryKey: ["inspection", id],
      queryFn: async () => {
        const data = await inspectionApi.getInspection(id);
        return convertSingleInspectionData(data);
      },
      enabled: !!id,
    });

  return { inspection: data, isPending, isError, isSuccess, error };
}

export function useCreateInspection() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inspectionApi.createInspection,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["inspections"] });
    },
  });
}
