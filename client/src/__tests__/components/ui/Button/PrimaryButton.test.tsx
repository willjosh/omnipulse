import { render, screen, fireEvent } from "@testing-library/react";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";

describe("PrimaryButton", () => {
  it("renders with children text", () => {
    render(<PrimaryButton>Click me</PrimaryButton>);

    const button = screen.getByRole("button", { name: "Click me" });
    expect(button).toBeInTheDocument();
  });

  it("handles click events", () => {
    const handleClick = jest.fn();
    render(<PrimaryButton onClick={handleClick}>Click me</PrimaryButton>);

    const button = screen.getByRole("button", { name: "Click me" });
    fireEvent.click(button);

    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it("applies disabled state correctly", () => {
    render(<PrimaryButton disabled>Click me</PrimaryButton>);

    const button = screen.getByRole("button", { name: "Click me" });
    expect(button).toBeDisabled();
    expect(button).toHaveClass("opacity-50", "cursor-not-allowed");
  });

  it("applies custom className", () => {
    render(<PrimaryButton className="custom-class">Click me</PrimaryButton>);

    const button = screen.getByRole("button", { name: "Click me" });
    expect(button).toHaveClass("custom-class");
  });

  it("sets correct button type", () => {
    render(<PrimaryButton type="submit">Submit</PrimaryButton>);

    const button = screen.getByRole("button", { name: "Submit" });
    expect(button).toHaveAttribute("type", "submit");
  });

  it("defaults to button type when not specified", () => {
    render(<PrimaryButton>Click me</PrimaryButton>);

    const button = screen.getByRole("button", { name: "Click me" });
    expect(button).toHaveAttribute("type", "button");
  });
});
