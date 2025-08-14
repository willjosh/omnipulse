import { render, screen, fireEvent } from "@testing-library/react";
import DataTable from "@/components/ui/Table/DataTable";

// Mock the Loading component
jest.mock("@/components/ui/Feedback", () => ({
  Loading: () => <div data-testid="loading">Loading...</div>,
}));

// Mock the ActionsColumnHeader and ActionsColumnCell
jest.mock("@/components/ui/Table/ActionsColumn", () => ({
  ActionsColumnHeader: () => <th data-testid="actions-header">Actions</th>,
  ActionsColumnCell: ({ item, actions, onActionClick }: any) => (
    <td data-testid="actions-cell">
      <button
        onClick={() =>
          onActionClick(actions[0], item, { stopPropagation: jest.fn() })
        }
      >
        Action
      </button>
    </td>
  ),
}));

describe("DataTable", () => {
  const mockData = [
    { id: "1", name: "Item 1", status: "Active" },
    { id: "2", name: "Item 2", status: "Inactive" },
  ];

  const mockColumns = [
    { key: "name", header: "Name", sortable: true },
    { key: "status", header: "Status" },
  ];

  const defaultProps = {
    data: mockData,
    columns: mockColumns,
    getItemId: (item: any) => item.id,
  };

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders table with data", () => {
    render(<DataTable {...defaultProps} />);

    expect(screen.getByText("Name")).toBeInTheDocument();
    expect(screen.getByText("Status")).toBeInTheDocument();
    expect(screen.getByText("Item 1")).toBeInTheDocument();
    expect(screen.getByText("Item 2")).toBeInTheDocument();
  });

  it("shows loading state when loading is true", () => {
    render(<DataTable {...defaultProps} loading={true} />);

    expect(screen.getByTestId("loading")).toBeInTheDocument();
    expect(screen.queryByText("Item 1")).not.toBeInTheDocument();
  });

  it("shows empty state when no data", () => {
    const emptyState = <div>No items found</div>;
    render(<DataTable {...defaultProps} data={[]} emptyState={emptyState} />);

    expect(screen.getByText("No items found")).toBeInTheDocument();
  });

  it("handles row click when onRowClick is provided", () => {
    const onRowClick = jest.fn();
    render(<DataTable {...defaultProps} onRowClick={onRowClick} />);

    const firstRow = screen.getByText("Item 1").closest("tr");
    if (firstRow) {
      fireEvent.click(firstRow);
      expect(onRowClick).toHaveBeenCalledWith(mockData[0]);
    }
  });

  it("handles column sorting when sortable and onSort provided", () => {
    const onSort = jest.fn();
    render(
      <DataTable
        {...defaultProps}
        onSort={onSort}
        sortBy="name"
        sortOrder="asc"
      />,
    );

    const nameHeader = screen.getByText("Name");
    fireEvent.click(nameHeader);

    expect(onSort).toHaveBeenCalledWith("name");
  });

  it("shows sort indicators correctly", () => {
    render(
      <DataTable
        {...defaultProps}
        onSort={jest.fn()}
        sortBy="name"
        sortOrder="asc"
      />,
    );

    expect(screen.getByText("↑")).toBeInTheDocument();
  });

  it("renders custom cell content when render function is provided", () => {
    const columnsWithRender = [
      { key: "name", header: "Name" },
      {
        key: "status",
        header: "Status",
        render: (item: any) => (
          <span data-testid={`status-${item.id}`}>{item.status}</span>
        ),
      },
    ];

    render(<DataTable {...defaultProps} columns={columnsWithRender} />);

    expect(screen.getByTestId("status-1")).toBeInTheDocument();
    expect(screen.getByTestId("status-2")).toBeInTheDocument();
  });

  it("shows actions column when showActions is true", () => {
    render(<DataTable {...defaultProps} showActions={true} />);

    expect(screen.getByTestId("actions-header")).toBeInTheDocument();
  });

  it("hides actions column when showActions is false", () => {
    render(<DataTable {...defaultProps} showActions={false} />);

    expect(screen.queryByTestId("actions-header")).not.toBeInTheDocument();
  });

  it("applies custom column widths", () => {
    const columnsWithWidth = [
      { key: "name", header: "Name", width: "200px" },
      { key: "status", header: "Status", width: "150px" },
    ];

    render(<DataTable {...defaultProps} columns={columnsWithWidth} />);

    const nameHeader = screen.getByText("Name").closest("th");
    const statusHeader = screen.getByText("Status").closest("th");

    expect(nameHeader).toHaveStyle({ width: "200px" });
    expect(statusHeader).toHaveStyle({ width: "150px" });
  });

  it("handles actions as function", () => {
    const actionsFunction = (item: any) => [
      { key: "edit", label: "Edit", onClick: jest.fn() },
      { key: "delete", label: "Delete", onClick: jest.fn() },
    ];

    render(<DataTable {...defaultProps} actions={actionsFunction} />);

    // Actions should be rendered for each row
    expect(screen.getAllByTestId("actions-cell")).toHaveLength(2);
  });

  it("applies hover styles to table rows", () => {
    render(<DataTable {...defaultProps} />);

    const rows = screen.getAllByRole("row").slice(1); // Skip header row
    rows.forEach(row => {
      expect(row).toHaveClass("hover:bg-gray-50", "cursor-pointer");
    });
  });
});
