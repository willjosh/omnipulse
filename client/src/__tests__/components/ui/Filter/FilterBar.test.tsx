import { render, screen, fireEvent } from "@testing-library/react";
import FilterBar from "@/components/ui/Filter/FilterBar";

describe("FilterBar", () => {
  const defaultProps = {
    searchValue: "",
    onSearchChange: jest.fn(),
    onFilterChange: jest.fn(),
  };

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders with default props", () => {
    render(<FilterBar {...defaultProps} />);

    const searchInput = screen.getByRole("textbox");
    expect(searchInput).toBeInTheDocument();
    expect(searchInput).toHaveAttribute("placeholder", "Search...");
  });

  it("renders with custom search placeholder", () => {
    render(
      <FilterBar {...defaultProps} searchPlaceholder="Search inventory..." />,
    );

    const searchInput = screen.getByRole("textbox");
    expect(searchInput).toHaveAttribute("placeholder", "Search inventory...");
  });

  it("displays current search value", () => {
    render(<FilterBar {...defaultProps} searchValue="current search" />);

    const searchInput = screen.getByRole("textbox");
    expect(searchInput).toHaveValue("current search");
  });

  it("calls onSearchChange when search input changes", () => {
    const onSearchChange = jest.fn();
    render(<FilterBar {...defaultProps} onSearchChange={onSearchChange} />);

    const searchInput = screen.getByRole("textbox");
    fireEvent.change(searchInput, { target: { value: "new search" } });

    expect(onSearchChange).toHaveBeenCalledWith("new search");
  });

  it("applies custom className", () => {
    render(<FilterBar {...defaultProps} className="custom-filter-bar" />);

    const filterBar = screen.getByRole("textbox").closest("div")
      ?.parentElement?.parentElement;
    expect(filterBar).toHaveClass("custom-filter-bar");
  });

  it("has correct default styling", () => {
    render(<FilterBar {...defaultProps} />);

    const filterBar = screen.getByRole("textbox").closest("div")
      ?.parentElement?.parentElement;
    expect(filterBar).toHaveClass("flex", "items-center", "justify-center");
  });

  it("renders search input with correct styling", () => {
    render(<FilterBar {...defaultProps} />);

    const searchInput = screen.getByRole("textbox");
    expect(searchInput).toHaveClass("bg-white");
  });

  it("handles empty search value correctly", () => {
    const onSearchChange = jest.fn();
    render(<FilterBar {...defaultProps} onSearchChange={onSearchChange} />);

    const searchInput = screen.getByRole("textbox");
    fireEvent.change(searchInput, { target: { value: "" } });

    expect(onSearchChange).toHaveBeenCalledWith("");
  });

  it("maintains search input focus after changes", () => {
    render(<FilterBar {...defaultProps} />);

    const searchInput = screen.getByRole("textbox");
    searchInput.focus();
    expect(searchInput).toHaveFocus();

    fireEvent.change(searchInput, { target: { value: "test" } });
    expect(searchInput).toHaveFocus();
  });

  it("renders with proper accessibility attributes", () => {
    render(<FilterBar {...defaultProps} />);

    const searchInput = screen.getByRole("textbox");
    expect(searchInput).toHaveAttribute("aria-label", "Search");
  });

  it("handles rapid search input changes", () => {
    const onSearchChange = jest.fn();
    render(<FilterBar {...defaultProps} onSearchChange={onSearchChange} />);

    const searchInput = screen.getByRole("textbox");

    // Clear the mock before testing
    onSearchChange.mockClear();

    // Rapid changes - each change should trigger a call
    fireEvent.change(searchInput, { target: { value: "a" } });
    fireEvent.change(searchInput, { target: { value: "ab" } });
    fireEvent.change(searchInput, { target: { value: "abc" } });

    // Verify the final call was made with the correct value
    expect(onSearchChange).toHaveBeenLastCalledWith("abc");
    // Verify that at least one call was made
    expect(onSearchChange).toHaveBeenCalled();
  });
});
