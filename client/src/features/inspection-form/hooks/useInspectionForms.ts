import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { inspectionFormApi } from "../api/inspectionFormApi";
import {
  InspectionForm,
  InspectionFormFilter,
  CreateInspectionFormCommand,
  UpdateInspectionFormCommand,
} from "../types/inspectionFormType";
import { useDebounce } from "@/hooks/useDebounce";

export function useInspectionForms(filter: InspectionFormFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["inspectionForms", debouncedFilter],
    queryFn: async () => {
      return await inspectionFormApi.getInspectionForms(debouncedFilter);
    },
  });

  return {
    inspectionForms: data?.items ?? [],
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

export function useInspectionForm(id: number) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<InspectionForm>({
      queryKey: ["inspectionForm", id],
      queryFn: async () => {
        return await inspectionFormApi.getInspectionForm(id);
      },
      enabled: !!id,
    });

  return { inspectionForm: data, isPending, isError, isSuccess, error };
}

export function useCreateInspectionForm() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inspectionFormApi.createInspectionForm,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["inspectionForms"] });
    },
  });
}

export function useUpdateInspectionForm() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inspectionFormApi.updateInspectionForm,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["inspectionForms"] });
      await queryClient.invalidateQueries({
        queryKey: ["inspectionForm", variables.inspectionFormID],
      });
    },
  });
}

export function useDeactivateInspectionForm() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inspectionFormApi.deactivateInspectionForm,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["inspectionForms"] });
      await queryClient.invalidateQueries({ queryKey: ["inspectionForm", id] });
    },
  });
}
