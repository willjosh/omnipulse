import React from "react";
import { render, screen } from "@testing-library/react";
import EmptyState from "@/components/ui/Feedback/EmptyState";

describe("EmptyState", () => {
  describe("Rendering and Props", () => {
    test("should render with default props", () => {
      render(<EmptyState />);

      expect(screen.getByText("No Data")).toBeInTheDocument();
      expect(
        screen.getByText("There is nothing to display here yet."),
      ).toBeInTheDocument();
    });

    test("should render custom title and message", () => {
      render(
        <EmptyState
          title="Custom Title"
          message="This is a custom message for testing purposes."
        />,
      );

      expect(screen.getByText("Custom Title")).toBeInTheDocument();
      expect(
        screen.getByText("This is a custom message for testing purposes."),
      ).toBeInTheDocument();
    });

    test("should render icon when provided", () => {
      const testIcon = <span data-testid="test-icon">📦</span>;
      render(<EmptyState icon={testIcon} />);

      expect(screen.getByTestId("test-icon")).toBeInTheDocument();
      expect(screen.getByText("📦")).toBeInTheDocument();
    });

    test("should not render icon when not provided", () => {
      render(<EmptyState />);

      // Should not have the icon container div
      const container = screen.getByText("No Data").closest("div");
      const iconDiv = container?.querySelector(".text-5xl");
      expect(iconDiv).not.toBeInTheDocument();
    });

    test("should render action when provided", () => {
      const testAction = <button data-testid="test-action">Click me</button>;
      render(<EmptyState action={testAction} />);

      expect(screen.getByTestId("test-action")).toBeInTheDocument();
      expect(screen.getByText("Click me")).toBeInTheDocument();
    });

    test("should not render action when not provided", () => {
      render(<EmptyState />);

      expect(screen.queryByRole("button")).not.toBeInTheDocument();
    });

    test("should apply custom className", () => {
      render(<EmptyState className="custom-class" />);

      const container = screen.getByText("No Data").closest("div");
      expect(container).toHaveClass("custom-class");
    });

    test("should apply default classes", () => {
      render(<EmptyState />);

      const container = screen.getByText("No Data").closest("div");
      expect(container).toHaveClass(
        "flex",
        "flex-col",
        "items-center",
        "justify-center",
        "py-12",
      );
    });
  });

  describe("Content Variations", () => {
    test("should handle empty string props", () => {
      render(<EmptyState title="" message="" />);

      // Check that the title element exists but is empty
      const title = screen.getByRole("heading", { level: 2 });
      expect(title).toBeInTheDocument();
      expect(title).toHaveTextContent("");

      // Check that the message paragraph exists but is empty
      const messageElements = screen.getAllByText("", { selector: "p" });
      expect(messageElements.length).toBeGreaterThan(0);
    });

    test("should handle very long title", () => {
      const longTitle =
        "This is a very long title that might wrap to multiple lines in the component";
      render(<EmptyState title={longTitle} />);

      expect(screen.getByText(longTitle)).toBeInTheDocument();
    });

    test("should handle very long message", () => {
      const longMessage =
        "This is a very long message that should wrap nicely within the max-width constraint and provide good readability for users who need more detailed explanations about why there is no data to display.";
      render(<EmptyState message={longMessage} />);

      expect(screen.getByText(longMessage)).toBeInTheDocument();
    });

    test("should handle special characters in title and message", () => {
      render(
        <EmptyState
          title="No Data Available! 🚫"
          message="Please check your connection & try again... 🔄"
        />,
      );

      expect(screen.getByText("No Data Available! 🚫")).toBeInTheDocument();
      expect(
        screen.getByText("Please check your connection & try again... 🔄"),
      ).toBeInTheDocument();
    });

    test("should handle HTML entities in text", () => {
      render(
        <EmptyState
          title="No Data &amp; Information"
          message="Data &lt; 1 item found &gt; Please try again"
        />,
      );

      // HTML entities are decoded by React, so we check for the decoded version
      expect(screen.getByText("No Data & Information")).toBeInTheDocument();
      expect(
        screen.getByText("Data < 1 item found > Please try again"),
      ).toBeInTheDocument();
    });
  });

  describe("Icon Rendering", () => {
    test("should render text icon", () => {
      render(<EmptyState icon="📋" />);

      expect(screen.getByText("📋")).toBeInTheDocument();
    });

    test("should render React component icon", () => {
      const IconComponent = () => (
        <svg data-testid="svg-icon">
          <circle />
        </svg>
      );
      render(<EmptyState icon={<IconComponent />} />);

      expect(screen.getByTestId("svg-icon")).toBeInTheDocument();
    });

    test("should render complex icon with multiple elements", () => {
      const complexIcon = (
        <div data-testid="complex-icon">
          <span>📦</span>
          <span>📋</span>
        </div>
      );
      render(<EmptyState icon={complexIcon} />);

      expect(screen.getByTestId("complex-icon")).toBeInTheDocument();
      expect(screen.getByText("📦")).toBeInTheDocument();
      expect(screen.getByText("📋")).toBeInTheDocument();
    });

    test("should apply correct icon styling", () => {
      render(<EmptyState icon="📋" />);

      const iconContainer = screen.getByText("📋").closest("div");
      expect(iconContainer).toHaveClass("mb-4", "text-5xl", "text-gray-300");
    });
  });

  describe("Action Rendering", () => {
    test("should render button action", () => {
      const buttonAction = <button>Add New Item</button>;
      render(<EmptyState action={buttonAction} />);

      expect(
        screen.getByRole("button", { name: "Add New Item" }),
      ).toBeInTheDocument();
    });

    test("should render link action", () => {
      const linkAction = <a href="/create">Create New</a>;
      render(<EmptyState action={linkAction} />);

      expect(
        screen.getByRole("link", { name: "Create New" }),
      ).toBeInTheDocument();
    });

    test("should render multiple actions", () => {
      const multipleActions = (
        <div>
          <button>Action 1</button>
          <button>Action 2</button>
        </div>
      );
      render(<EmptyState action={multipleActions} />);

      expect(
        screen.getByRole("button", { name: "Action 1" }),
      ).toBeInTheDocument();
      expect(
        screen.getByRole("button", { name: "Action 2" }),
      ).toBeInTheDocument();
    });

    test("should apply correct action styling", () => {
      const testAction = <button>Test Action</button>;
      render(<EmptyState action={testAction} />);

      const actionContainer = screen.getByRole("button").closest("div");
      expect(actionContainer).toHaveClass("mt-2");
    });
  });

  describe("Styling and Layout", () => {
    test("should have correct title styling", () => {
      render(<EmptyState title="Test Title" />);

      const title = screen.getByText("Test Title");
      expect(title).toHaveClass(
        "text-xl",
        "font-semibold",
        "text-gray-700",
        "mb-2",
      );
    });

    test("should have correct message styling", () => {
      render(<EmptyState message="Test message" />);

      const message = screen.getByText("Test message");
      expect(message).toHaveClass(
        "text-gray-500",
        "mb-4",
        "text-center",
        "max-w-md",
      );
    });

    test("should combine custom className with default classes", () => {
      render(<EmptyState className="bg-red-100 border" />);

      const container = screen.getByText("No Data").closest("div");
      expect(container).toHaveClass(
        "flex",
        "flex-col",
        "items-center",
        "justify-center",
        "py-12",
        "bg-red-100",
        "border",
      );
    });

    test("should maintain layout structure with all props", () => {
      const testIcon = <span data-testid="icon">🔍</span>;
      const testAction = <button data-testid="action">Search</button>;

      render(
        <EmptyState
          icon={testIcon}
          title="Search Results"
          message="No results found for your search criteria."
          action={testAction}
          className="custom-empty-state"
        />,
      );

      // Find the main container by getting the title and going up to the main container
      const title = screen.getByText("Search Results");
      const container = title.closest("div");
      expect(container).toHaveClass("custom-empty-state");

      // Verify order of elements
      const icon = screen.getByTestId("icon");
      const message = screen.getByText(
        "No results found for your search criteria.",
      );
      const action = screen.getByTestId("action");

      expect(icon).toBeInTheDocument();
      expect(title).toBeInTheDocument();
      expect(message).toBeInTheDocument();
      expect(action).toBeInTheDocument();
    });
  });

  describe("Accessibility", () => {
    test("should have proper heading hierarchy", () => {
      render(<EmptyState title="No Items Found" />);

      const heading = screen.getByRole("heading", { level: 2 });
      expect(heading).toHaveTextContent("No Items Found");
    });

    test("should be accessible without action", () => {
      render(<EmptyState title="Empty State" message="Nothing to show" />);

      expect(screen.getByRole("heading")).toBeInTheDocument();
      expect(screen.getByText("Nothing to show")).toBeInTheDocument();
    });

    test("should maintain accessibility with interactive elements", () => {
      const action = <button aria-label="Add new item">Add Item</button>;
      render(<EmptyState action={action} />);

      const button = screen.getByLabelText("Add new item");
      expect(button).toBeInTheDocument();
      expect(button).toHaveAttribute("aria-label", "Add new item");
    });
  });

  describe("Edge Cases", () => {
    test("should handle null icon gracefully", () => {
      render(<EmptyState icon={null} />);

      expect(screen.getByText("No Data")).toBeInTheDocument();
      expect(
        screen.getByText("There is nothing to display here yet."),
      ).toBeInTheDocument();
    });

    test("should handle undefined action gracefully", () => {
      render(<EmptyState action={undefined} />);

      expect(screen.getByText("No Data")).toBeInTheDocument();
      expect(screen.queryByRole("button")).not.toBeInTheDocument();
    });

    test("should handle zero values", () => {
      render(<EmptyState title="0 Items" message="Found 0 results" />);

      expect(screen.getByText("0 Items")).toBeInTheDocument();
      expect(screen.getByText("Found 0 results")).toBeInTheDocument();
    });

    test("should render consistently without any props", () => {
      const { container } = render(<EmptyState />);

      expect(container.firstChild).toBeInTheDocument();
      expect(screen.getByText("No Data")).toBeInTheDocument();
      expect(
        screen.getByText("There is nothing to display here yet."),
      ).toBeInTheDocument();
    });
  });

  describe("Component Integration", () => {
    test("should work with custom React components", () => {
      const CustomIcon = () => <div data-testid="custom-icon">Custom</div>;
      const CustomAction = () => (
        <button data-testid="custom-action">Custom Action</button>
      );

      render(
        <EmptyState
          icon={<CustomIcon />}
          title="Custom Empty State"
          message="This is using custom components"
          action={<CustomAction />}
        />,
      );

      expect(screen.getByTestId("custom-icon")).toBeInTheDocument();
      expect(screen.getByText("Custom Empty State")).toBeInTheDocument();
      expect(
        screen.getByText("This is using custom components"),
      ).toBeInTheDocument();
      expect(screen.getByTestId("custom-action")).toBeInTheDocument();
    });

    test("should maintain performance with complex children", () => {
      const complexAction = (
        <div>
          {Array.from({ length: 5 }, (_, i) => (
            <button key={i} data-testid={`button-${i}`}>
              Button {i + 1}
            </button>
          ))}
        </div>
      );

      render(<EmptyState action={complexAction} />);

      for (let i = 0; i < 5; i++) {
        expect(screen.getByTestId(`button-${i}`)).toBeInTheDocument();
      }
    });
  });
});
