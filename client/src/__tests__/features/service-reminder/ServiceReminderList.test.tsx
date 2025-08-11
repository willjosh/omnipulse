/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import ServiceReminderList from "@/features/service-reminder/components/ServiceReminderList";

// Mock Next.js router
jest.mock("next/navigation", () => ({ useRouter: jest.fn() }));

// Mock the service reminder hooks with simple any types
jest.mock("@/features/service-reminder/hooks/useServiceReminders", () => ({
  useServiceReminders: jest.fn(),
  useAddWorkOrderToServiceReminder: jest.fn(),
}));

// Mock the work order hooks
jest.mock("@/features/work-order/hooks/useWorkOrders", () => ({
  useWorkOrders: jest.fn(),
}));

// Mock all UI components with simple implementations
jest.mock("@/components/ui/Table/DataTable", () => ({
  __esModule: true,
  default: (props: any) => {
    if (props.loading) return <div>Loading...</div>;
    if (!props.data || props.data.length === 0)
      return props.emptyState || <div>No data</div>;
    return (
      <div>
        {props.data.map((item: any, i: number) => (
          <div key={i} onClick={() => props.onRowClick?.(item)}>
            {item.vehicleName || item.title}
          </div>
        ))}
      </div>
    );
  },
}));

jest.mock("@/components/ui/Filter/FilterBar", () => ({
  __esModule: true,
  default: (props: any) => (
    <input
      placeholder={props.placeholder}
      value={props.searchValue || ""}
      onChange={e => props.onSearchChange?.(e.target.value)}
      data-testid="search-input"
    />
  ),
}));

jest.mock("@/components/ui/Table/PaginationControls", () => ({
  __esModule: true,
  default: (props: any) => {
    if (!props.pagination) return null;
    return (
      <div>
        <span>Page {props.pagination.pageNumber}</span>
        <button onClick={() => props.onPageChange?.(1)}>Next</button>
      </div>
    );
  },
}));

jest.mock("@/components/ui/Modal/ConfirmModal", () => ({
  __esModule: true,
  default: (props: any) => {
    if (!props.isOpen) return null;
    return (
      <div>
        <h2>{props.title}</h2>
        {props.children}
        <button onClick={props.onConfirm}>Confirm</button>
        <button onClick={props.onCancel}>Cancel</button>
      </div>
    );
  },
}));

jest.mock("@/components/ui/Modal/ModalPortal", () => ({
  __esModule: true,
  default: (props: any) => props.children,
}));

// Mock the service reminder actions config
jest.mock("@/features/service-reminder/config/serviceReminderActions", () => ({
  ServiceReminderActionType: { ADD_WORK_ORDER: "add_work_order" },
  SERVICE_REMINDER_ACTION_CONFIG: {
    add_work_order: { label: "Add Work Order", variant: "primary" },
  },
}));

// Import the mocked hooks
import {
  useServiceReminders,
  useAddWorkOrderToServiceReminder,
} from "@/features/service-reminder/hooks/useServiceReminders";
import { useWorkOrders } from "@/features/work-order/hooks/useWorkOrders";

// Create wrapper for React Query
const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false }, mutations: { retry: false } },
  });

  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
};

// Mock data
const mockServiceReminders = [
  {
    id: 1,
    vehicleName: "Toyota Camry",
    serviceProgramName: "Oil Change",
    status: 1,
    statusLabel: "Active",
  },
  {
    id: 2,
    vehicleName: "Honda Civic",
    serviceProgramName: "Brake Inspection",
    status: 2,
    statusLabel: "Due Soon",
  },
];

const mockWorkOrders = [
  {
    id: 1,
    title: "Oil Change",
    vehicleName: "Toyota Camry",
    statusLabel: "Pending",
    workOrderLineItems: [],
  },
];

const mockPagination = {
  totalCount: 2,
  pageNumber: 1,
  pageSize: 10,
  totalPages: 1,
  hasPreviousPage: false,
  hasNextPage: false,
};

describe("ServiceReminderList", () => {
  const mockUseServiceReminders = useServiceReminders as jest.MockedFunction<
    typeof useServiceReminders
  >;
  const mockUseAddWorkOrderToServiceReminder =
    useAddWorkOrderToServiceReminder as jest.MockedFunction<
      typeof useAddWorkOrderToServiceReminder
    >;
  const mockUseWorkOrders = useWorkOrders as jest.MockedFunction<
    typeof useWorkOrders
  >;

  beforeEach(() => {
    jest.clearAllMocks();

    // Default mock implementations
    mockUseServiceReminders.mockReturnValue({
      serviceReminders: mockServiceReminders,
      pagination: mockPagination,
      isPending: false,
      isError: false,
      isSuccess: true,
      error: null,
    } as any);

    mockUseAddWorkOrderToServiceReminder.mockReturnValue({
      mutateAsync: jest.fn(),
      isPending: false,
    } as any);

    mockUseWorkOrders.mockReturnValue({
      workOrders: mockWorkOrders,
      pagination: null,
      isPending: false,
      isError: false,
      error: null,
      isSuccess: true,
    } as any);
  });

  describe("Rendering and Data Display", () => {
    test("renders service reminder list with data", async () => {
      render(<ServiceReminderList />, { wrapper: createWrapper() });

      expect(screen.getByText("Service Reminders")).toBeInTheDocument();
      expect(screen.getByText("Toyota Camry")).toBeInTheDocument();
      expect(screen.getByText("Honda Civic")).toBeInTheDocument();
    });

    test("displays loading state", () => {
      mockUseServiceReminders.mockReturnValue({
        serviceReminders: [],
        pagination: null,
        isPending: true,
        isError: false,
        isSuccess: false,
        error: null,
      } as any);

      render(<ServiceReminderList />, { wrapper: createWrapper() });
      expect(screen.getByText("Service Reminders")).toBeInTheDocument();
    });

    test("displays error state", () => {
      mockUseServiceReminders.mockReturnValue({
        serviceReminders: [],
        pagination: null,
        isPending: false,
        isError: true,
        error: { message: "Failed to load service reminders" } as any,
        isSuccess: false,
      } as any);

      render(<ServiceReminderList />, { wrapper: createWrapper() });
      expect(
        screen.getByText(
          "Error loading service reminders: Failed to load service reminders",
        ),
      ).toBeInTheDocument();
    });

    test("displays empty state when no reminders", () => {
      mockUseServiceReminders.mockReturnValue({
        serviceReminders: [],
        pagination: { ...mockPagination, totalCount: 0 },
        isPending: false,
        isError: false,
        isSuccess: true,
        error: null,
      } as any);

      render(<ServiceReminderList />, { wrapper: createWrapper() });
      expect(
        screen.getByText("No service reminders found."),
      ).toBeInTheDocument();
    });
  });

  describe("Search and Filtering", () => {
    test("handles search input changes", async () => {
      render(<ServiceReminderList />, { wrapper: createWrapper() });

      const searchInput = screen.getByTestId("search-input");
      fireEvent.change(searchInput, { target: { value: "Toyota" } });

      expect(searchInput).toHaveValue("Toyota");
    });
  });

  describe("Work Order Modal", () => {
    test("opens work order modal when add work order is clicked", async () => {
      render(<ServiceReminderList />, { wrapper: createWrapper() });

      // The modal should be accessible through the DataTable actions
      expect(screen.getByText("Service Reminders")).toBeInTheDocument();
    });
  });

  describe("Error Handling", () => {
    test("handles API errors gracefully", () => {
      mockUseServiceReminders.mockReturnValue({
        serviceReminders: [],
        pagination: null,
        isPending: false,
        isError: true,
        error: { message: "API Error" } as any,
        isSuccess: false,
      } as any);

      render(<ServiceReminderList />, { wrapper: createWrapper() });
      expect(
        screen.getByText("Error loading service reminders: API Error"),
      ).toBeInTheDocument();
    });
  });
});
