import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import ConfirmModal from "@/components/ui/Modal/ConfirmModal";

// Mock the ModalPortal component to avoid portal-related issues in tests
jest.mock("@/components/ui/Modal/ModalPortal", () => {
  return function MockModalPortal({
    children,
    isOpen,
  }: {
    children: React.ReactNode;
    isOpen: boolean;
  }) {
    if (!isOpen) return null;
    return <div data-testid="modal-portal">{children}</div>;
  };
});

describe("ConfirmModal", () => {
  const defaultProps = {
    isOpen: true,
    onClose: jest.fn(),
    onConfirm: jest.fn(),
    title: "Confirm Action",
    message: "Are you sure you want to proceed?",
  };

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders when open", () => {
    render(<ConfirmModal {...defaultProps} />);

    expect(screen.getByText("Confirm Action")).toBeInTheDocument();
    expect(
      screen.getByText("Are you sure you want to proceed?"),
    ).toBeInTheDocument();
    expect(screen.getByText("Confirm")).toBeInTheDocument();
    expect(screen.getByText("Cancel")).toBeInTheDocument();
  });

  it("does not render when closed", () => {
    render(<ConfirmModal {...defaultProps} isOpen={false} />);

    expect(screen.queryByText("Confirm Action")).not.toBeInTheDocument();
    expect(
      screen.queryByText("Are you sure you want to proceed?"),
    ).not.toBeInTheDocument();
  });

  it("calls onClose when cancel button is clicked", () => {
    render(<ConfirmModal {...defaultProps} />);

    const cancelButton = screen.getByText("Cancel");
    fireEvent.click(cancelButton);

    expect(defaultProps.onClose).toHaveBeenCalledTimes(1);
  });

  it("calls onConfirm and onClose when confirm button is clicked", () => {
    render(<ConfirmModal {...defaultProps} />);

    const confirmButton = screen.getByText("Confirm");
    fireEvent.click(confirmButton);

    expect(defaultProps.onConfirm).toHaveBeenCalledTimes(1);
    expect(defaultProps.onClose).toHaveBeenCalledTimes(1);
  });

  it("calls onClose when backdrop is clicked", () => {
    render(<ConfirmModal {...defaultProps} />);

    const backdrop = screen
      .getByTestId("modal-portal")
      .querySelector(".backdrop-brightness-50");
    if (backdrop) {
      fireEvent.click(backdrop);
      expect(defaultProps.onClose).toHaveBeenCalledTimes(1);
    }
  });

  it("uses custom confirm and cancel text", () => {
    render(
      <ConfirmModal {...defaultProps} confirmText="Delete" cancelText="Keep" />,
    );

    expect(screen.getByText("Delete")).toBeInTheDocument();
    expect(screen.getByText("Keep")).toBeInTheDocument();
  });

  it("applies correct styling to confirm button", () => {
    render(<ConfirmModal {...defaultProps} />);

    const confirmButton = screen.getByText("Confirm");
    expect(confirmButton).toHaveClass("bg-red-600", "text-white", "rounded-md");
  });

  it("applies correct styling to cancel button", () => {
    render(<ConfirmModal {...defaultProps} />);

    const cancelButton = screen.getByText("Cancel");
    expect(cancelButton).toHaveClass("text-gray-600", "hover:text-gray-800");
  });

  it("has correct modal structure", () => {
    render(<ConfirmModal {...defaultProps} />);

    expect(screen.getByTestId("modal-portal")).toBeInTheDocument();
    expect(screen.getByRole("heading", { level: 3 })).toBeInTheDocument();
    expect(
      screen.getByText("Are you sure you want to proceed?"),
    ).toBeInTheDocument();
  });
});
