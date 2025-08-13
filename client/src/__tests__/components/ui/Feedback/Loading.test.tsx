import React from "react";
import { render, screen } from "@testing-library/react";
import Loading from "@/components/ui/Feedback/Loading";

describe("Loading", () => {
  describe("Rendering and Content", () => {
    test("should render loading text", () => {
      render(<Loading />);

      expect(screen.getByText("Loading...")).toBeInTheDocument();
    });

    test("should render loading spinner", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      expect(spinner).toBeInTheDocument();
    });

    test("should have proper container structure", () => {
      const { container } = render(<Loading />);

      const outerContainer = container.firstChild;
      expect(outerContainer).toHaveClass(
        "flex",
        "justify-center",
        "items-center",
      );

      const innerContainer = container.querySelector(".p-8");
      expect(innerContainer).toBeInTheDocument();
    });
  });

  describe("Styling and Layout", () => {
    test("should apply correct container classes", () => {
      const { container } = render(<Loading />);

      const outerDiv = container.firstChild;
      expect(outerDiv).toHaveClass("flex", "justify-center", "items-center");
    });

    test("should apply correct inner container classes", () => {
      const { container } = render(<Loading />);

      const innerDiv = container.querySelector(".p-8");
      expect(innerDiv).toHaveClass("p-8");
    });

    test("should apply correct spinner classes", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      expect(spinner).toHaveClass(
        "animate-spin",
        "rounded-full",
        "size-8",
        "border-b-2",
        "border-blue-600",
        "mx-auto",
      );
    });

    test("should apply correct text classes", () => {
      render(<Loading />);

      const text = screen.getByText("Loading...");
      expect(text).toHaveClass("mt-2", "text-gray-500");
    });

    test("should have proper text positioning", () => {
      render(<Loading />);

      const text = screen.getByText("Loading...");
      expect(text.tagName).toBe("P");
      expect(text).toHaveClass("mt-2");
    });
  });

  describe("Animation and Visual Effects", () => {
    test("should have spinning animation class", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      expect(spinner).toHaveClass("animate-spin");
    });

    test("should have circular shape", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      expect(spinner).toHaveClass("rounded-full");
    });

    test("should have proper size", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      expect(spinner).toHaveClass("size-8");
    });

    test("should have proper border styling", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      expect(spinner).toHaveClass("border-b-2", "border-blue-600");
    });

    test("should be centered horizontally", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      expect(spinner).toHaveClass("mx-auto");
    });
  });

  describe("Accessibility", () => {
    test("should provide loading indication text", () => {
      render(<Loading />);

      const loadingText = screen.getByText("Loading...");
      expect(loadingText).toBeInTheDocument();
    });

    test("should be readable by screen readers", () => {
      render(<Loading />);

      // The loading text should be discoverable
      expect(screen.getByText("Loading...")).toBeVisible();
    });

    test("should not have any interactive elements", () => {
      render(<Loading />);

      expect(screen.queryByRole("button")).not.toBeInTheDocument();
      expect(screen.queryByRole("link")).not.toBeInTheDocument();
      expect(screen.queryByRole("textbox")).not.toBeInTheDocument();
    });

    test("should have proper semantic structure", () => {
      render(<Loading />);

      const text = screen.getByText("Loading...");
      expect(text.tagName).toBe("P");
    });
  });

  describe("Component Structure", () => {
    test("should have two main sections: spinner and text", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      const text = screen.getByText("Loading...");

      expect(spinner).toBeInTheDocument();
      expect(text).toBeInTheDocument();
    });

    test("should have spinner before text", () => {
      const { container } = render(<Loading />);

      const innerContainer = container.querySelector(".p-8");
      const children = Array.from(innerContainer?.children || []);

      expect(children).toHaveLength(2);
      expect(children[0]).toHaveClass("animate-spin");
      expect(children[1]).toHaveTextContent("Loading...");
    });

    test("should maintain consistent hierarchy", () => {
      const { container } = render(<Loading />);

      // Check nesting: outer div > inner div > (spinner + text)
      const outerDiv = container.firstChild as HTMLElement;
      const innerDiv = outerDiv.querySelector(".p-8");
      const spinner = innerDiv?.querySelector(".animate-spin");
      const text = innerDiv?.querySelector("p");

      expect(outerDiv).toBeInTheDocument();
      expect(innerDiv).toBeInTheDocument();
      expect(spinner).toBeInTheDocument();
      expect(text).toBeInTheDocument();
    });
  });

  describe("Rendering Consistency", () => {
    test("should render the same content on multiple renders", () => {
      const { rerender } = render(<Loading />);

      expect(screen.getByText("Loading...")).toBeInTheDocument();

      rerender(<Loading />);

      expect(screen.getByText("Loading...")).toBeInTheDocument();
    });

    test("should maintain DOM structure across renders", () => {
      const { container, rerender } = render(<Loading />);

      const initialHTML = container.innerHTML;

      rerender(<Loading />);

      expect(container.innerHTML).toBe(initialHTML);
    });

    test("should not have any props or configuration", () => {
      // This component doesn't accept props, so we just verify it renders
      const { container } = render(<Loading />);

      expect(container.firstChild).toBeInTheDocument();
      expect(screen.getByText("Loading...")).toBeInTheDocument();
    });
  });

  describe("Visual Layout", () => {
    test("should center content both horizontally and vertically", () => {
      const { container } = render(<Loading />);

      const outerContainer = container.firstChild;
      expect(outerContainer).toHaveClass(
        "flex",
        "justify-center",
        "items-center",
      );
    });

    test("should have proper spacing around content", () => {
      const { container } = render(<Loading />);

      const innerContainer = container.querySelector(".p-8");
      expect(innerContainer).toHaveClass("p-8");
    });

    test("should have proper spacing between spinner and text", () => {
      render(<Loading />);

      const text = screen.getByText("Loading...");
      expect(text).toHaveClass("mt-2");
    });

    test("should maintain compact layout", () => {
      const { container } = render(<Loading />);

      // The component should not take up excessive space
      const outerDiv = container.firstChild as HTMLElement;
      const innerDiv = container.querySelector(".p-8");

      expect(outerDiv).toHaveClass("flex");
      expect(innerDiv).toHaveClass("p-8");

      // Should have minimal required children
      expect(innerDiv?.children).toHaveLength(2);
    });
  });

  describe("Color and Theme", () => {
    test("should use blue color for spinner", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      expect(spinner).toHaveClass("border-blue-600");
    });

    test("should use gray color for text", () => {
      render(<Loading />);

      const text = screen.getByText("Loading...");
      expect(text).toHaveClass("text-gray-500");
    });

    test("should have consistent color scheme", () => {
      const { container } = render(<Loading />);

      const spinner = container.querySelector(".animate-spin");
      const text = screen.getByText("Loading...");

      // Verify color consistency (blue for active element, gray for text)
      expect(spinner).toHaveClass("border-blue-600");
      expect(text).toHaveClass("text-gray-500");
    });
  });

  describe("Performance and Optimization", () => {
    test("should render quickly without complex calculations", () => {
      const startTime = performance.now();
      render(<Loading />);
      const endTime = performance.now();

      // Should render very quickly (arbitrary threshold for test environment)
      expect(endTime - startTime).toBeLessThan(100);
    });

    test("should have minimal DOM footprint", () => {
      const { container } = render(<Loading />);

      // Should have a small DOM tree: 1 outer div + 1 inner div + 2 elements
      const allElements = container.querySelectorAll("*");
      expect(allElements.length).toBeLessThanOrEqual(4);
    });

    test("should not create any side effects", () => {
      const consoleSpy = jest
        .spyOn(console, "error")
        .mockImplementation(() => {});

      render(<Loading />);

      expect(consoleSpy).not.toHaveBeenCalled();

      consoleSpy.mockRestore();
    });
  });
});
