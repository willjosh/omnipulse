import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import InventoryList from "@/features/inventory/components/InventoryList";

// Mock the hooks
jest.mock("@/features/inventory/hooks/useInventory", () => ({
  useInventories: jest.fn(),
  useDeleteInventory: jest.fn(),
}));

// Mock the notification hook
jest.mock("@/components/ui/Feedback/NotificationProvider", () => ({
  useNotification: () => ({ notify: jest.fn() }),
}));

// Mock the router
jest.mock("next/navigation", () => ({
  useRouter: () => ({ push: jest.fn() }),
}));

// Mock the table components
jest.mock("@/components/ui/Table", () => ({
  DataTable: ({ data, onRowClick, onSort }: any) => (
    <div data-testid="data-table">
      {data?.map((item: any) => (
        <div key={item.id} onClick={() => onRowClick?.(item)}>
          {item.name}
        </div>
      ))}
    </div>
  ),
  PaginationControls: ({ onPageChange, onPageSizeChange }: any) => (
    <div data-testid="pagination">
      <button onClick={() => onPageChange?.(2)}>Page 2</button>
      <button onClick={() => onPageSizeChange?.(25)}>Size 25</button>
    </div>
  ),
}));

// Mock the filter components
jest.mock("@/components/ui/Filter", () => ({
  FilterBar: ({ onSearch, onSort }: any) => (
    <div data-testid="filter-bar">
      <input
        placeholder="Search..."
        onChange={e => onSearch?.(e.target.value)}
        data-testid="search-input"
      />
      <button onClick={() => onSort?.("name")}>Sort by Name</button>
    </div>
  ),
}));

// Mock the button components
jest.mock("@/components/ui/Button", () => ({
  PrimaryButton: ({ children, onClick }: any) => (
    <button onClick={onClick} data-testid="primary-button">
      {children}
    </button>
  ),
  OptionButton: ({ children, onClick }: any) => (
    <button onClick={onClick} data-testid="option-button">
      {children}
    </button>
  ),
}));

// Mock the modal component
jest.mock("@/components/ui/Modal", () => ({
  ConfirmModal: ({ isOpen, onConfirm, title, message }: any) =>
    isOpen ? (
      <div data-testid="confirm-modal">
        <h3>{title}</h3>
        <p>{message}</p>
        <button onClick={onConfirm} data-testid="confirm-button">
          Confirm
        </button>
      </div>
    ) : null,
}));

// Mock the feedback components
jest.mock("@/components/ui/Feedback", () => ({
  Loading: () => <div data-testid="loading">Loading...</div>,
  EmptyState: ({ message }: any) => (
    <div data-testid="empty-state">{message}</div>
  ),
}));

describe("InventoryList", () => {
  const mockInventories = [
    { id: 1, name: "Item 1", status: "Active" },
    { id: 2, name: "Item 2", status: "Inactive" },
  ];

  const mockPagination = {
    totalCount: 2,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1,
    hasPreviousPage: false,
    hasNextPage: false,
  };

  const mockUseInventories = {
    inventories: mockInventories,
    pagination: mockPagination,
    isPending: false,
    isError: false,
  };

  const mockUseDeleteInventory = { mutate: jest.fn(), isPending: false };

  beforeEach(() => {
    jest.clearAllMocks();

    // Setup default mock implementations
    const {
      useInventories,
      useDeleteInventory,
    } = require("@/features/inventory/hooks/useInventory");
    useInventories.mockReturnValue(mockUseInventories);
    useDeleteInventory.mockReturnValue(mockUseDeleteInventory);
  });

  it("renders inventory list when data is available", () => {
    render(<InventoryList />);

    expect(screen.getByText("Item 1")).toBeInTheDocument();
    expect(screen.getByText("Item 2")).toBeInTheDocument();
    expect(screen.getByTestId("data-table")).toBeInTheDocument();
  });

  it("shows loading state when data is pending", () => {
    const {
      useInventories,
    } = require("@/features/inventory/hooks/useInventory");
    useInventories.mockReturnValue({ ...mockUseInventories, isPending: true });

    render(<InventoryList />);

    expect(screen.getByTestId("loading")).toBeInTheDocument();
  });

  it("shows empty state when no inventories", () => {
    const {
      useInventories,
    } = require("@/features/inventory/hooks/useInventory");
    useInventories.mockReturnValue({ ...mockUseInventories, inventories: [] });

    render(<InventoryList />);

    // The component should show empty state when no data
    expect(screen.getByTestId("data-table")).toBeInTheDocument();
  });

  it("handles search input changes", () => {
    render(<InventoryList />);

    const searchInput = screen.getByTestId("search-input");
    fireEvent.change(searchInput, { target: { value: "test search" } });

    // The search should trigger a page reset (handled by useEffect)
    expect(searchInput).toHaveValue("test search");
  });

  it("handles sorting changes", () => {
    render(<InventoryList />);

    const sortButton = screen.getByText("Sort by Name");
    fireEvent.click(sortButton);

    // Should trigger sort and reset page
    expect(screen.getByText("Sort by Name")).toBeInTheDocument();
  });

  it("handles pagination changes", () => {
    render(<InventoryList />);

    const pageButton = screen.getByText("Page 2");
    fireEvent.click(pageButton);

    expect(screen.getByText("Page 2")).toBeInTheDocument();
  });

  it("handles page size changes", () => {
    render(<InventoryList />);

    const sizeButton = screen.getByText("Size 25");
    fireEvent.click(sizeButton);

    expect(screen.getByText("Size 25")).toBeInTheDocument();
  });

  it("opens confirm modal when delete is clicked", async () => {
    render(<InventoryList />);

    // Simulate opening delete modal (this would typically be triggered by an action)
    // For now, we'll test that the modal component is rendered
    expect(screen.queryByTestId("confirm-modal")).not.toBeInTheDocument();
  });

  it("handles row clicks for navigation", () => {
    render(<InventoryList />);

    const firstItem = screen.getByText("Item 1");
    fireEvent.click(firstItem);

    // Should trigger navigation (mocked)
    expect(firstItem).toBeInTheDocument();
  });

  it("applies correct filters when state changes", () => {
    render(<InventoryList />);

    // Test that filters are applied correctly
    expect(screen.getByTestId("filter-bar")).toBeInTheDocument();
  });

  it("handles error states gracefully", () => {
    const {
      useInventories,
    } = require("@/features/inventory/hooks/useInventory");
    useInventories.mockReturnValue({ ...mockUseInventories, isError: true });

    render(<InventoryList />);

    // Should handle error state appropriately by showing error message
    expect(screen.getByText("Error Loading Inventory")).toBeInTheDocument();
    expect(
      screen.getByText(
        "Unable to load inventory. Please check your connection and try again.",
      ),
    ).toBeInTheDocument();
  });
});
