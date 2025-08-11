import { render, screen, fireEvent } from "@testing-library/react";
import PaginationControls from "@/components/ui/Table/PaginationControls";

describe("PaginationControls", () => {
  const defaultProps = {
    currentPage: 1,
    totalPages: 5,
    totalItems: 100,
    itemsPerPage: 10,
    onPreviousPage: jest.fn(),
    onNextPage: jest.fn(),
  };

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders with default props", () => {
    render(<PaginationControls {...defaultProps} />);

    expect(screen.getByText("1 - 10 of 100")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: "Previous page" }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: "Next page" }),
    ).toBeInTheDocument();
  });

  it("displays correct item count range", () => {
    render(<PaginationControls {...defaultProps} currentPage={3} />);

    // Page 3 with 10 items per page: (3-1)*10 + 1 = 21 to 3*10 = 30
    expect(screen.getByText("21 - 30 of 100")).toBeInTheDocument();
  });

  it("displays zero count when no items", () => {
    render(<PaginationControls {...defaultProps} totalItems={0} />);

    expect(screen.getByText("0 - 0 of 0")).toBeInTheDocument();
  });

  it("displays correct count for last page", () => {
    render(
      <PaginationControls {...defaultProps} currentPage={10} totalPages={10} />,
    );

    // Last page: 91 - 100 of 100
    expect(screen.getByText("91 - 100 of 100")).toBeInTheDocument();
  });

  it("calls onPreviousPage when previous button is clicked", () => {
    const onPreviousPage = jest.fn();
    render(
      <PaginationControls
        {...defaultProps}
        onPreviousPage={onPreviousPage}
        currentPage={2}
      />,
    );

    const previousButton = screen.getByRole("button", {
      name: "Previous page",
    });
    fireEvent.click(previousButton);

    expect(onPreviousPage).toHaveBeenCalledTimes(1);
  });

  it("calls onNextPage when next button is clicked", () => {
    const onNextPage = jest.fn();
    render(<PaginationControls {...defaultProps} onNextPage={onNextPage} />);

    const nextButton = screen.getByRole("button", { name: "Next page" });
    fireEvent.click(nextButton);

    expect(onNextPage).toHaveBeenCalledTimes(1);
  });

  it("disables previous button on first page", () => {
    render(<PaginationControls {...defaultProps} currentPage={1} />);

    const previousButton = screen.getByRole("button", {
      name: "Previous page",
    });
    expect(previousButton).toBeDisabled();
    expect(previousButton).toHaveClass("text-gray-300", "cursor-not-allowed");
  });

  it("disables next button on last page", () => {
    render(
      <PaginationControls {...defaultProps} currentPage={5} totalPages={5} />,
    );

    const nextButton = screen.getByRole("button", { name: "Next page" });
    expect(nextButton).toBeDisabled();
    expect(nextButton).toHaveClass("text-gray-300", "cursor-not-allowed");
  });

  it("enables navigation buttons on middle pages", () => {
    render(<PaginationControls {...defaultProps} currentPage={3} />);

    const previousButton = screen.getByRole("button", {
      name: "Previous page",
    });
    const nextButton = screen.getByRole("button", { name: "Next page" });

    expect(previousButton).not.toBeDisabled();
    expect(nextButton).not.toBeDisabled();
    expect(previousButton).toHaveClass("text-gray-600", "hover:bg-gray-100");
    expect(nextButton).toHaveClass("text-gray-600", "hover:bg-gray-100");
  });

  it("shows page size selector when onPageSizeChange is provided", () => {
    const onPageSizeChange = jest.fn();
    render(
      <PaginationControls
        {...defaultProps}
        onPageSizeChange={onPageSizeChange}
      />,
    );

    expect(screen.getByText("Show")).toBeInTheDocument();
    expect(screen.getByDisplayValue("10")).toBeInTheDocument();
    expect(screen.getByText("per page")).toBeInTheDocument();
  });

  it("hides page size selector when onPageSizeChange is not provided", () => {
    render(<PaginationControls {...defaultProps} />);

    expect(screen.queryByText("Show")).not.toBeInTheDocument();
    expect(screen.queryByDisplayValue("10")).not.toBeInTheDocument();
  });

  it("calls onPageSizeChange when page size is changed", () => {
    const onPageSizeChange = jest.fn();
    render(
      <PaginationControls
        {...defaultProps}
        onPageSizeChange={onPageSizeChange}
      />,
    );

    const select = screen.getByDisplayValue("10");
    fireEvent.change(select, { target: { value: "25" } });

    expect(onPageSizeChange).toHaveBeenCalledWith(25);
  });

  it("uses default page size options", () => {
    const onPageSizeChange = jest.fn();
    render(
      <PaginationControls
        {...defaultProps}
        onPageSizeChange={onPageSizeChange}
      />,
    );

    const select = screen.getByDisplayValue("10");
    const options = Array.from(select.querySelectorAll("option")).map(
      option => option.value,
    );

    expect(options).toEqual(["10", "25", "50", "100"]);
  });

  it("uses custom page size options", () => {
    const onPageSizeChange = jest.fn();
    const customOptions = [5, 15, 30];
    render(
      <PaginationControls
        {...defaultProps}
        onPageSizeChange={onPageSizeChange}
        pageSizeOptions={customOptions}
        itemsPerPage={5}
      />,
    );

    const select = screen.getByDisplayValue("5");
    const options = Array.from(select.querySelectorAll("option")).map(
      option => option.value,
    );

    expect(options).toEqual(["5", "15", "30"]);
  });

  it("hides item count when showItemCount is false", () => {
    render(<PaginationControls {...defaultProps} showItemCount={false} />);

    expect(screen.queryByText("1 - 10 of 100")).not.toBeInTheDocument();
  });

  it("applies custom className", () => {
    render(
      <PaginationControls {...defaultProps} className="custom-pagination" />,
    );

    const container = screen
      .getByRole("button", { name: "Previous page" })
      .closest("div")?.parentElement;
    expect(container).toHaveClass("custom-pagination");
  });

  it("has correct default styling", () => {
    render(<PaginationControls {...defaultProps} />);

    const container = screen
      .getByRole("button", { name: "Previous page" })
      .closest("div")?.parentElement;
    expect(container).toHaveClass("flex", "items-center", "gap-4");
  });

  it("shows separator when both page size and item count are visible", () => {
    const onPageSizeChange = jest.fn();
    render(
      <PaginationControls
        {...defaultProps}
        onPageSizeChange={onPageSizeChange}
      />,
    );

    // The separator is a div with specific styling
    const separator = screen
      .getByRole("button", { name: "Previous page" })
      .closest("div")
      ?.parentElement?.querySelector(".h-4.w-px.bg-gray-300");
    expect(separator).toBeInTheDocument();
  });

  it("handles edge case with single page", () => {
    render(
      <PaginationControls
        {...defaultProps}
        currentPage={1}
        totalPages={1}
        totalItems={5}
      />,
    );

    expect(screen.getByText("1 - 5 of 5")).toBeInTheDocument();

    const previousButton = screen.getByRole("button", {
      name: "Previous page",
    });
    const nextButton = screen.getByRole("button", { name: "Next page" });

    expect(previousButton).toBeDisabled();
    expect(nextButton).toBeDisabled();
  });

  it("handles edge case with items less than page size", () => {
    render(
      <PaginationControls {...defaultProps} totalItems={5} itemsPerPage={10} />,
    );

    expect(screen.getByText("1 - 5 of 5")).toBeInTheDocument();
  });

  it("maintains button state consistency", () => {
    const { rerender } = render(
      <PaginationControls {...defaultProps} currentPage={1} />,
    );

    let previousButton = screen.getByRole("button", { name: "Previous page" });
    expect(previousButton).toBeDisabled();

    // Change to middle page
    rerender(<PaginationControls {...defaultProps} currentPage={3} />);
    previousButton = screen.getByRole("button", { name: "Previous page" });
    expect(previousButton).not.toBeDisabled();

    // Change to last page
    rerender(
      <PaginationControls {...defaultProps} currentPage={5} totalPages={5} />,
    );
    const nextButton = screen.getByRole("button", { name: "Next page" });
    expect(nextButton).toBeDisabled();
  });
});
