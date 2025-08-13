import React from "react";
import { renderHook, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import {
  useVehicles,
  useVehicle,
  useCreateVehicle,
  useUpdateVehicle,
  useDeactivateVehicle,
  useVehicleAssignedData,
  useVehicleStatusData,
  useVehicleGroups,
  useTechnicians,
} from "@/features/vehicle/hooks/useVehicles";
import { vehicleApi } from "@/features/vehicle/api/vehicleApi";
import { useDebounce } from "@/hooks/useDebounce";

// Mock the vehicle API
jest.mock("@/features/vehicle/api/vehicleApi", () => ({
  vehicleApi: {
    getVehicles: jest.fn(),
    getVehicle: jest.fn(),
    createVehicle: jest.fn(),
    updateVehicle: jest.fn(),
    deactivateVehicle: jest.fn(),
    getVehicleAssignedData: jest.fn(),
    getVehicleStatusData: jest.fn(),
    getVehicleGroups: jest.fn(),
    getTechnicians: jest.fn(),
  },
  convertVehicleData: jest.fn(vehicle => vehicle), // Return the vehicle unchanged for simplicity
}));

// Mock the useDebounce hook
jest.mock("@/hooks/useDebounce", () => ({ useDebounce: jest.fn() }));

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false }, mutations: { retry: false } },
  });

  function TestWrapper({ children }: { children: React.ReactNode }) {
    return React.createElement(
      QueryClientProvider,
      { client: queryClient },
      children,
    );
  }
  return TestWrapper;
};

const mockVehicleApi = vehicleApi as jest.Mocked<typeof vehicleApi>;
const mockUseDebounce = useDebounce as jest.MockedFunction<typeof useDebounce>;

// Mock data
const mockVehicle = {
  id: 1,
  name: "Test Vehicle",
  make: "Toyota",
  model: "Camry",
  year: 2020,
  vin: "ABC123",
  licensePlate: "XYZ789",
  licensePlateExpirationDate: "2025-12-31",
  vehicleType: 1,
  vehicleTypeLabel: "Truck",
  vehicleGroupID: 1,
  vehicleGroupName: "Fleet A",
  assignedTechnicianName: "John Doe",
  assignedTechnicianID: "tech1",
  status: 1,
  statusLabel: "Active",
  mileage: 50000,
  engineHours: 2000,
  location: "Main Garage",
  trim: "SE",
  fuelCapacity: 50,
  fuelType: 1,
  purchaseDate: "2020-01-01",
  purchasePrice: 25000,
};

const mockPaginatedResponse = {
  items: [mockVehicle],
  totalCount: 1,
  pageNumber: 1,
  pageSize: 10,
  totalPages: 1,
  hasPreviousPage: false,
  hasNextPage: false,
};

describe("Vehicle hooks", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseDebounce.mockImplementation(value => value);
  });

  describe("useVehicles", () => {
    test("should fetch vehicles successfully", async () => {
      mockVehicleApi.getVehicles.mockResolvedValue(mockPaginatedResponse);

      const { result } = renderHook(() => useVehicles(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.isSuccess).toBe(true);
      });

      expect(result.current.vehicles).toEqual([mockVehicle]);
      expect(result.current.pagination).toEqual({
        totalCount: 1,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      });
      expect(mockVehicleApi.getVehicles).toHaveBeenCalledWith({});
    });

    test("should handle search filter with debounce", async () => {
      const searchTerm = "Toyota";
      mockUseDebounce.mockReturnValue(searchTerm);
      mockVehicleApi.getVehicles.mockResolvedValue(mockPaginatedResponse);

      const { result } = renderHook(() => useVehicles({ Search: searchTerm }), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.isSuccess).toBe(true);
      });

      expect(mockUseDebounce).toHaveBeenCalledWith(searchTerm, 300);
      expect(mockVehicleApi.getVehicles).toHaveBeenCalledWith({
        Search: searchTerm,
      });
    });

    test("should handle API error", async () => {
      const error = new Error("Failed to fetch vehicles");
      mockVehicleApi.getVehicles.mockRejectedValue(error);

      const { result } = renderHook(() => useVehicles(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.isError).toBe(true);
      });

      expect(result.current.error).toBe(error);
      expect(result.current.vehicles).toEqual([]);
      expect(result.current.pagination).toBeNull();
    });

    test("should handle loading state", () => {
      mockVehicleApi.getVehicles.mockImplementation(
        () => new Promise(() => {}),
      );

      const { result } = renderHook(() => useVehicles(), {
        wrapper: createWrapper(),
      });

      expect(result.current.isPending).toBe(true);
      expect(result.current.vehicles).toEqual([]);
    });
  });

  describe("useVehicle", () => {
    test("should fetch single vehicle successfully", async () => {
      mockVehicleApi.getVehicle.mockResolvedValue(mockVehicle);

      const { result } = renderHook(() => useVehicle("1"), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.isSuccess).toBe(true);
      });

      expect(result.current.vehicle).toEqual(mockVehicle);
      expect(mockVehicleApi.getVehicle).toHaveBeenCalledWith("1");
    });

    test("should not fetch when id is empty", () => {
      const { result } = renderHook(() => useVehicle(""), {
        wrapper: createWrapper(),
      });

      // With our fix, isPending should immediately be false for invalid IDs
      expect(result.current.isPending).toBe(false);
      expect(mockVehicleApi.getVehicle).not.toHaveBeenCalled();
    });

    test("should handle fetch error", async () => {
      const error = new Error("Vehicle not found");
      mockVehicleApi.getVehicle.mockRejectedValue(error);

      const { result } = renderHook(() => useVehicle("999"), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.isError).toBe(true);
      });

      expect(result.current.error).toBe(error);
    });
  });

  describe("useCreateVehicle", () => {
    test("should create vehicle successfully", async () => {
      const newVehicle = {
        name: "New Vehicle",
        make: "Honda",
        model: "Civic",
        year: 2023,
        vin: "VIN123456789",
        licensePlate: "ABC123",
        licensePlateExpirationDate: "2025-12-31",
        vehicleType: 1,
        vehicleGroupID: 1,
        assignedTechnicianID: "tech1",
        status: 1,
        mileage: 0,
        engineHours: 0,
        trim: "LX",
        fuelCapacity: 45,
        fuelType: 1,
        purchaseDate: "2023-01-01",
        purchasePrice: 30000,
        location: "Main Garage",
      };
      mockVehicleApi.createVehicle.mockResolvedValue(mockVehicle);

      const { result } = renderHook(() => useCreateVehicle(), {
        wrapper: createWrapper(),
      });

      result.current.mutate(newVehicle);

      await waitFor(() => {
        expect(result.current.isSuccess).toBe(true);
      });

      expect(mockVehicleApi.createVehicle).toHaveBeenCalledWith(newVehicle);
    });

    test("should handle creation error", async () => {
      const error = new Error("Creation failed");
      mockVehicleApi.createVehicle.mockRejectedValue(error);

      const { result } = renderHook(() => useCreateVehicle(), {
        wrapper: createWrapper(),
      });

      const completeVehicle = {
        name: "New Vehicle",
        make: "Honda",
        model: "Civic",
        year: 2023,
        vin: "VIN123456789",
        licensePlate: "ABC123",
        licensePlateExpirationDate: "2025-12-31",
        vehicleType: 1,
        vehicleGroupID: 1,
        assignedTechnicianID: "tech1",
        status: 1,
        mileage: 0,
        engineHours: 0,
        trim: "LX",
        fuelCapacity: 45,
        fuelType: 1,
        purchaseDate: "2023-01-01",
        purchasePrice: 30000,
        location: "Main Garage",
      };
      result.current.mutate(completeVehicle);

      await waitFor(() => {
        expect(result.current.isError).toBe(true);
      });

      expect(result.current.error).toBe(error);
    });
  });

  describe("useUpdateVehicle", () => {
    test("should update vehicle successfully", async () => {
      const updateData = {
        vehicleID: 1,
        name: "Updated Vehicle",
        make: "Toyota",
        model: "Camry",
        year: 2020,
        vin: "ABC123",
        licensePlate: "XYZ789",
        licensePlateExpirationDate: "2025-12-31",
        vehicleType: 1,
        vehicleGroupID: 1,
        assignedTechnicianID: "tech1",
        status: 1,
        mileage: 50000,
        engineHours: 2000,
        trim: "SE",
        fuelCapacity: 50,
        fuelType: 1,
        purchaseDate: "2020-01-01",
        purchasePrice: 25000,
        location: "Main Garage",
      };
      mockVehicleApi.updateVehicle.mockResolvedValue(mockVehicle);

      const { result } = renderHook(() => useUpdateVehicle(), {
        wrapper: createWrapper(),
      });

      result.current.mutate(updateData);

      await waitFor(() => {
        expect(result.current.isSuccess).toBe(true);
      });

      expect(mockVehicleApi.updateVehicle).toHaveBeenCalledWith(updateData);
    });
  });

  describe("useDeactivateVehicle", () => {
    test("should deactivate vehicle successfully", async () => {
      mockVehicleApi.deactivateVehicle.mockResolvedValue(undefined);

      const { result } = renderHook(() => useDeactivateVehicle(), {
        wrapper: createWrapper(),
      });

      result.current.mutate("1");

      await waitFor(() => {
        expect(result.current.isSuccess).toBe(true);
      });

      expect(mockVehicleApi.deactivateVehicle).toHaveBeenCalledWith("1");
    });
  });

  describe("useVehicleAssignedData", () => {
    test("should fetch assigned data successfully", async () => {
      const assignedData = {
        assignedVehicleCount: 5,
        unassignedVehicleCount: 3,
      };
      mockVehicleApi.getVehicleAssignedData.mockResolvedValue(assignedData);

      const { result } = renderHook(() => useVehicleAssignedData(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.isSuccess).toBe(true);
      });

      expect(result.current.assignedVehicleCount).toBe(5);
      expect(result.current.unassignedVehicleCount).toBe(3);
    });

    test("should handle default values when no data", () => {
      mockVehicleApi.getVehicleAssignedData.mockImplementation(
        () => new Promise(() => {}),
      );

      const { result } = renderHook(() => useVehicleAssignedData(), {
        wrapper: createWrapper(),
      });

      expect(result.current.assignedVehicleCount).toBe(0);
      expect(result.current.unassignedVehicleCount).toBe(0);
    });
  });

  describe("useVehicleStatusData", () => {
    test("should fetch status data successfully", async () => {
      const statusData = {
        activeVehicleCount: 10,
        inactiveVehicleCount: 2,
        maintenanceVehicleCount: 3,
        outOfServiceVehicleCount: 1,
      };
      mockVehicleApi.getVehicleStatusData.mockResolvedValue(statusData);

      const { result } = renderHook(() => useVehicleStatusData(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.isSuccess).toBe(true);
      });

      expect(result.current.activeVehicleCount).toBe(10);
      expect(result.current.inactiveVehicleCount).toBe(2);
      expect(result.current.maintenanceVehicleCount).toBe(3);
      expect(result.current.outOfServiceVehicleCount).toBe(1);
    });
  });

  describe("useVehicleGroups", () => {
    test("should fetch vehicle groups successfully", async () => {
      const groups = [
        { id: 1, name: "Fleet A", description: "Main fleet", isActive: true },
        {
          id: 2,
          name: "Fleet B",
          description: "Secondary fleet",
          isActive: true,
        },
      ];
      mockVehicleApi.getVehicleGroups.mockResolvedValue(groups);

      const { result } = renderHook(() => useVehicleGroups(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.vehicleGroups).toEqual(groups);
      });

      expect(mockVehicleApi.getVehicleGroups).toHaveBeenCalled();
    });
  });

  describe("useTechnicians", () => {
    test("should fetch technicians successfully", async () => {
      const technicians = [
        {
          id: "1",
          firstName: "John",
          lastName: "Doe",
          email: "john@example.com",
          isActive: true,
          hireDate: "2020-01-01",
        },
      ];
      mockVehicleApi.getTechnicians.mockResolvedValue(technicians);

      const { result } = renderHook(() => useTechnicians(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.technicians).toEqual(technicians);
      });

      expect(mockVehicleApi.getTechnicians).toHaveBeenCalled();
    });

    test("should handle missing hireDate", async () => {
      const technicians = [
        {
          id: "1",
          firstName: "John",
          lastName: "Doe",
          email: "john@example.com",
          isActive: true,
          hireDate: "",
        },
      ];
      mockVehicleApi.getTechnicians.mockResolvedValue(technicians);

      const { result } = renderHook(() => useTechnicians(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => {
        expect(result.current.technicians[0].hireDate).toBe("");
      });
    });
  });
});
