import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  inspectionFormItemApi,
  convertInspectionFormItemData,
} from "../api/inspectionFormItemApi";
import {
  InspectionFormItemWithLabels,
  InspectionFormItemFilter,
  UpdateInspectionFormItemCommand,
} from "../types/inspectionFormItemType";
import { useDebounce } from "@/hooks/useDebounce";

export function useInspectionFormItems(
  inspectionFormId: number,
  filter: InspectionFormItemFilter = {},
) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["inspectionFormItems", inspectionFormId, debouncedFilter],
    queryFn: async () => {
      const data = await inspectionFormItemApi.getInspectionFormItems(
        inspectionFormId,
        debouncedFilter,
      );
      return { ...data, items: data.items.map(convertInspectionFormItemData) };
    },
    enabled: !!inspectionFormId,
  });

  return {
    inspectionFormItems: data?.items ?? [],
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

export function useInspectionFormItem(
  inspectionFormId: number,
  itemId: number,
) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<InspectionFormItemWithLabels>({
      queryKey: ["inspectionFormItem", inspectionFormId, itemId],
      queryFn: async () => {
        const data = await inspectionFormItemApi.getInspectionFormItem(
          inspectionFormId,
          itemId,
        );
        return convertInspectionFormItemData(data);
      },
      enabled: !!inspectionFormId && !!itemId,
    });

  return { inspectionFormItem: data, isPending, isError, isSuccess, error };
}

export function useCreateInspectionFormItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inspectionFormItemApi.createInspectionFormItem,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["inspectionFormItems", variables.inspectionFormID],
      });
      await queryClient.invalidateQueries({
        queryKey: ["inspectionForm", variables.inspectionFormID],
      });
      await queryClient.invalidateQueries({ queryKey: ["inspectionForms"] });
    },
  });
}

export function useUpdateInspectionFormItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({
      inspectionFormId,
      command,
    }: {
      inspectionFormId: number;
      command: UpdateInspectionFormItemCommand;
    }) => {
      return await inspectionFormItemApi.updateInspectionFormItem(
        inspectionFormId,
        command,
      );
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["inspectionFormItems", variables.inspectionFormId],
      });
      await queryClient.invalidateQueries({
        queryKey: [
          "inspectionFormItem",
          variables.inspectionFormId,
          variables.command.inspectionFormItemID,
        ],
      });
      await queryClient.invalidateQueries({ queryKey: ["inspectionForms"] });
    },
  });
}

export function useDeactivateInspectionFormItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({
      inspectionFormId,
      itemId,
    }: {
      inspectionFormId: number;
      itemId: number;
    }) => {
      return await inspectionFormItemApi.deactivateInspectionFormItem(
        inspectionFormId,
        itemId,
      );
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["inspectionFormItems", variables.inspectionFormId],
      });
      await queryClient.invalidateQueries({
        queryKey: [
          "inspectionFormItem",
          variables.inspectionFormId,
          variables.itemId,
        ],
      });
      await queryClient.invalidateQueries({ queryKey: ["inspectionForms"] });
    },
  });
}
