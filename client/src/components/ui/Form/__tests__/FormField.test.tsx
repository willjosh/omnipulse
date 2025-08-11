import { render, screen } from "@testing-library/react";
import FormField from "../FormField";

describe("FormField", () => {
  const defaultProps = {
    label: "Email Address",
    children: <input type="email" placeholder="Enter email" />,
  };

  it("renders label correctly", () => {
    render(<FormField {...defaultProps} />);

    expect(screen.getByText("Email Address")).toBeInTheDocument();
  });

  it("renders children correctly", () => {
    render(<FormField {...defaultProps} />);

    const input = screen.getByPlaceholderText("Enter email");
    expect(input).toBeInTheDocument();
    expect(input).toHaveAttribute("type", "email");
  });

  it("associates label with input using htmlFor", () => {
    render(<FormField {...defaultProps} htmlFor="email-input" />);

    const label = screen.getByText("Email Address");
    const input = screen.getByPlaceholderText("Enter email");

    expect(label).toHaveAttribute("for", "email-input");
    // Note: The input needs to have an id attribute for the association to work
    // This test shows the label is properly configured
  });

  it("shows required indicator when required is true", () => {
    render(<FormField {...defaultProps} required />);

    const requiredIndicator = screen.getByText("*");
    expect(requiredIndicator).toBeInTheDocument();
    expect(requiredIndicator).toHaveClass("text-red-500");
  });

  it("does not show required indicator when required is false", () => {
    render(<FormField {...defaultProps} required={false} />);

    expect(screen.queryByText("*")).not.toBeInTheDocument();
  });

  it("shows error message when error is provided", () => {
    const errorMessage = "Please enter a valid email address";
    render(<FormField {...defaultProps} error={errorMessage} />);

    expect(screen.getByText(errorMessage)).toBeInTheDocument();
    expect(screen.getByText(errorMessage)).toHaveClass(
      "text-sm",
      "text-red-500",
    );
  });

  it("does not show error message when error is not provided", () => {
    render(<FormField {...defaultProps} />);

    expect(
      screen.queryByText("Please enter a valid email address"),
    ).not.toBeInTheDocument();
  });

  it("applies custom className", () => {
    render(<FormField {...defaultProps} className="custom-field" />);

    const fieldContainer = screen.getByText("Email Address").closest("div");
    expect(fieldContainer).toHaveClass("custom-field");
  });

  it("has correct default structure", () => {
    render(<FormField {...defaultProps} />);

    const fieldContainer = screen.getByText("Email Address").closest("div");
    expect(fieldContainer).toHaveClass("flex", "flex-col", "gap-1");
  });

  it("renders with complex children", () => {
    const complexChildren = (
      <div>
        <input type="text" />
        <button type="button">Submit</button>
      </div>
    );

    render(<FormField {...defaultProps} children={complexChildren} />);

    expect(screen.getByRole("button")).toBeInTheDocument();
    expect(screen.getByRole("textbox")).toBeInTheDocument();
  });

  it("handles empty label gracefully", () => {
    render(<FormField {...defaultProps} label="" />);

    const fieldContainer = screen
      .getByPlaceholderText("Enter email")
      .closest("div");
    expect(fieldContainer).toBeInTheDocument();
  });
});
