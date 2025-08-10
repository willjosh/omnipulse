import React from "react";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { useRouter } from "next/navigation";

export interface BreadcrumbItem {
  label: string;
  href?: string;
}

interface BreadcrumbsProps {
  items: BreadcrumbItem[];
  separator?: React.ReactNode;
  className?: string;
}

const Breadcrumbs: React.FC<BreadcrumbsProps> = ({
  items,
  separator = <span className="mx-2 text-gray-400">/</span>,
  className = "",
}) => {
  const router = useRouter();

  // If only one breadcrumb item, show a simple back arrow navigation
  if (items.length === 1) {
    const item = items[0];
    return (
      <nav
        className={`flex items-center text-sm ${className}`}
        aria-label="Breadcrumb"
      >
        {item.href ? (
          <button
            onClick={() => router.push(item.href!)}
            className="flex items-center text-gray-600 hover:text-blue-500 transition-colors"
          >
            <ArrowLeft className="w-4 h-4 mr-1" />
            <span className="text-sm">{item.label}</span>
          </button>
        ) : (
          <span className="flex items-center text-gray-600">
            <ArrowLeft className="w-4 h-4 mr-1" />
            <span className="text-sm">{item.label}</span>
          </span>
        )}
      </nav>
    );
  }

  // For multiple breadcrumb items, show full breadcrumb trail with ">" separator
  return (
    <nav
      className={`flex items-center text-sm ${className}`}
      aria-label="Breadcrumb"
    >
      {items.map((item, idx) => (
        <span key={idx} className="flex items-center">
          {item.href ? (
            <Link
              href={item.href}
              className="text-gray-600 hover:text-blue-500"
            >
              {item.label}
            </Link>
          ) : (
            <span className="text-gray-700 font-medium">{item.label}</span>
          )}
          {idx < items.length - 1 && separator}
        </span>
      ))}
    </nav>
  );
};

export default Breadcrumbs;
