import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import FuelPurchaseList from "@/features/fuel-purchases/components/FuelPurchaseList";

// Mock the hooks
jest.mock("@/features/fuel-purchases/hooks/useFuelPurchases", () => ({
  useFuelPurchases: () => ({
    fuelPurchases: [],
    pagination: null,
    isPending: false,
    isError: false,
    isSuccess: true,
    error: null,
  }),
  useDeleteFuelPurchase: () => ({ mutateAsync: jest.fn(), isPending: false }),
}));

// Mock the vehicle hooks
jest.mock("@/features/vehicle/hooks/useVehicles", () => ({
  useVehicles: () => ({ vehicles: [], isPending: false }),
  useTechnicians: () => ({ technicians: [], isPending: false }),
}));

// Mock the router
jest.mock("next/navigation", () => ({
  useRouter: () => ({ push: jest.fn() }),
}));

// Mock the notification
jest.mock("@/components/ui/Feedback/NotificationProvider", () => ({
  useNotification: () => jest.fn(),
}));

describe("FuelPurchaseList", () => {
  let queryClient: QueryClient;

  beforeEach(() => {
    queryClient = new QueryClient({
      defaultOptions: { queries: { retry: false } },
    });
  });

  it("renders without crashing", () => {
    render(
      <QueryClientProvider client={queryClient}>
        <FuelPurchaseList />
      </QueryClientProvider>,
    );

    expect(screen.getByText("Fuel Purchases")).toBeInTheDocument();
    expect(screen.getByText("Add Fuel Purchase")).toBeInTheDocument();
  });

  it("shows empty state when no fuel purchases", () => {
    render(
      <QueryClientProvider client={queryClient}>
        <FuelPurchaseList />
      </QueryClientProvider>,
    );

    expect(screen.getByText("No fuel purchases found.")).toBeInTheDocument();
  });
});
