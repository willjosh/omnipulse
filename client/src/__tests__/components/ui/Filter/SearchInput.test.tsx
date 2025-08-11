import {
  render,
  screen,
  fireEvent,
  waitFor,
  act,
} from "@testing-library/react";
import SearchInput from "@/components/ui/Filter/SearchInput";

describe("SearchInput", () => {
  const defaultProps = { value: "", onChange: jest.fn() };

  beforeEach(() => {
    jest.clearAllMocks();
    // Enable fake timers for each test that needs them
    jest.useFakeTimers();
  });

  afterEach(() => {
    // Clean up timers and restore real timers
    jest.clearAllTimers();
    jest.useRealTimers();
  });

  it("renders with default props", () => {
    render(<SearchInput {...defaultProps} />);

    const input = screen.getByRole("textbox");
    expect(input).toBeInTheDocument();
    expect(input).toHaveAttribute("placeholder", "Search...");
    expect(input).toHaveAttribute("aria-label", "Search");
  });

  it("renders with custom placeholder and aria-label", () => {
    render(
      <SearchInput
        {...defaultProps}
        placeholder="Search items..."
        ariaLabel="Search inventory"
      />,
    );

    const input = screen.getByRole("textbox");
    expect(input).toHaveAttribute("placeholder", "Search items...");
    expect(input).toHaveAttribute("aria-label", "Search inventory");
  });

  it("handles input changes without debounce", () => {
    const onChange = jest.fn();
    render(
      <SearchInput {...defaultProps} onChange={onChange} debounceMs={0} />,
    );

    const input = screen.getByRole("textbox");
    fireEvent.change(input, { target: { value: "test" } });

    expect(onChange).toHaveBeenCalledWith("test");
  });

  it("handles input changes with debounce", async () => {
    const onChange = jest.fn();
    render(
      <SearchInput {...defaultProps} onChange={onChange} debounceMs={300} />,
    );

    const input = screen.getByRole("textbox");
    fireEvent.change(input, { target: { value: "test" } });

    // Should not call onChange immediately
    expect(onChange).not.toHaveBeenCalled();

    // Fast-forward time to trigger debounce
    act(() => {
      jest.advanceTimersByTime(300);
    });

    await waitFor(() => {
      expect(onChange).toHaveBeenCalledWith("test");
    });
  });

  it("debounces multiple rapid changes", async () => {
    const onChange = jest.fn();
    render(
      <SearchInput {...defaultProps} onChange={onChange} debounceMs={300} />,
    );

    const input = screen.getByRole("textbox");

    // Type rapidly
    fireEvent.change(input, { target: { value: "t" } });
    fireEvent.change(input, { target: { value: "te" } });
    fireEvent.change(input, { target: { value: "tes" } });
    fireEvent.change(input, { target: { value: "test" } });

    // Should not call onChange yet
    expect(onChange).not.toHaveBeenCalled();

    // Fast-forward time
    act(() => {
      jest.advanceTimersByTime(300);
    });

    await waitFor(() => {
      // Should only call onChange once with the final value
      expect(onChange).toHaveBeenCalledTimes(1);
      expect(onChange).toHaveBeenCalledWith("test");
    });
  });

  it("shows clear button when input has value", () => {
    render(<SearchInput {...defaultProps} value="test" />);

    const clearButton = screen.getByRole("button", { name: "Clear search" });
    expect(clearButton).toBeInTheDocument();
  });

  it("hides clear button when input is empty", () => {
    render(<SearchInput {...defaultProps} value="" />);

    const clearButton = screen.queryByRole("button", { name: "Clear search" });
    expect(clearButton).not.toBeInTheDocument();
  });

  it("clears input when clear button is clicked", () => {
    const onChange = jest.fn();
    render(<SearchInput {...defaultProps} value="test" onChange={onChange} />);

    const clearButton = screen.getByRole("button", { name: "Clear search" });
    fireEvent.click(clearButton);

    expect(onChange).toHaveBeenCalledWith("");
  });

  it("syncs internal value with external value prop", () => {
    const { rerender } = render(
      <SearchInput {...defaultProps} value="initial" />,
    );

    const input = screen.getByRole("textbox");
    expect(input).toHaveValue("initial");

    // Update the value prop
    rerender(<SearchInput {...defaultProps} value="updated" />);

    expect(input).toHaveValue("updated");
  });

  it("applies custom container and input classes", () => {
    render(
      <SearchInput
        {...defaultProps}
        containerClassName="custom-container"
        inputClassName="custom-input"
      />,
    );

    const container = screen.getByRole("textbox").closest("div");
    const input = screen.getByRole("textbox");

    expect(container).toHaveClass("custom-container");
    expect(input).toHaveClass("custom-input");
  });

  it("applies full width styling when fullWidth is true", () => {
    render(<SearchInput {...defaultProps} fullWidth={true} />);

    const container = screen.getByRole("textbox").closest("div");
    const input = screen.getByRole("textbox");

    expect(container).toHaveClass("w-full");
    expect(input).toHaveClass("w-full");
  });

  it("applies default width when fullWidth is false", () => {
    render(<SearchInput {...defaultProps} fullWidth={false} />);

    const input = screen.getByRole("textbox");
    expect(input).toHaveClass("w-48");
  });

  it("shows search icon", () => {
    render(<SearchInput {...defaultProps} />);

    // The search icon should be present (it's an SVG)
    const searchIcon = screen
      .getByRole("textbox")
      .parentElement?.querySelector("svg");
    expect(searchIcon).toBeInTheDocument();
  });

  it("maintains focus after clearing", () => {
    const onChange = jest.fn();
    render(<SearchInput {...defaultProps} value="test" onChange={onChange} />);

    const input = screen.getByRole("textbox");
    const clearButton = screen.getByRole("button", { name: "Clear search" });

    // Focus the input
    input.focus();
    expect(input).toHaveFocus();

    // Click clear button
    fireEvent.click(clearButton);

    // Input should still be focused
    expect(input).toHaveFocus();
  });

  it("handles empty string input correctly", () => {
    const onChange = jest.fn();
    render(
      <SearchInput {...defaultProps} onChange={onChange} debounceMs={0} />,
    );

    const input = screen.getByRole("textbox");
    fireEvent.change(input, { target: { value: "" } });

    expect(onChange).toHaveBeenCalledWith("");
  });

  it("cancels previous debounce timeout on new input", async () => {
    const onChange = jest.fn();
    render(
      <SearchInput {...defaultProps} onChange={onChange} debounceMs={300} />,
    );

    const input = screen.getByRole("textbox");

    // Type first character
    fireEvent.change(input, { target: { value: "a" } });

    // Wait a bit but not enough to trigger debounce
    act(() => {
      jest.advanceTimersByTime(150);
    });

    // Type second character (should cancel first debounce)
    fireEvent.change(input, { target: { value: "ab" } });

    // Wait for debounce
    act(() => {
      jest.advanceTimersByTime(300);
    });

    await waitFor(() => {
      // Should only call onChange once with final value
      expect(onChange).toHaveBeenCalledTimes(1);
      expect(onChange).toHaveBeenCalledWith("ab");
    });
  });
});
