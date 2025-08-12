"use client";

import { useRouter, usePathname } from "next/navigation";
import { ChevronRight, ArrowLeft, Layers, User, Settings } from "lucide-react";
import LogoutButton from "./LogoutButton";
import { NavItem, NavSection } from "./types";

const settingsSections: NavSection[] = [
  {
    title: "Account",
    items: [{ label: "Profile", icon: User, path: "/settings/profile" }],
  },
];

const SettingsSidebar = () => {
  const router = useRouter();
  const pathname = usePathname();

  const handleItemClick = (item: NavItem) => {
    if (item.path) {
      router.push(item.path);
    }
  };

  const handleBackClick = () => {
    router.push("/");
  };

  const isItemActive = (item: NavItem) => {
    return pathname === item.path;
  };

  return (
    <aside className="fixed top-16 left-0 w-64 h-[calc(100vh-4rem)] bg-white border-r border-[var(--border)] text-sm flex flex-col overflow-hidden z-30 flex-shrink-0">
      {/* Header */}
      <div className="px-4 py-5 border-b border-[var(--border)]">
        <button
          onClick={handleBackClick}
          className="flex items-center gap-2 text-[var(--primary-color)] hover:text-[var(--primary-dark)] transition-colors text-sm font-medium mb-3"
        >
          <ArrowLeft size={16} />
          Back to Fleet
        </button>
        <h2 className="text-lg font-semibold text-gray-900">Settings</h2>
      </div>

      {/* Navigation Sections */}
      <nav className="flex-1 overflow-y-auto py-4">
        {settingsSections.map((section, sectionIndex) => (
          <div key={sectionIndex} className="mb-6">
            {section.title && (
              <div className="px-4 mb-2">
                <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                  {section.title}
                </h3>
              </div>
            )}
            <ul className="space-y-1">
              {section.items.map(item => {
                const isActive = isItemActive(item);
                const Icon = item.icon;

                return (
                  <li key={item.label}>
                    <div className="px-2">
                      <button
                        onClick={() => handleItemClick(item)}
                        className={`w-full flex items-center gap-3 px-3 py-2 rounded-md transition-all text-left ${
                          isActive
                            ? "bg-[var(--primary-color)] text-white"
                            : "text-gray-700 hover:bg-[var(--primary-light)] hover:text-[var(--primary-color)]"
                        }`}
                      >
                        <Icon size={18} />
                        <span className="truncate flex-1">{item.label}</span>
                        {item.path && (
                          <ChevronRight
                            size={16}
                            className="text-inherit opacity-50"
                          />
                        )}
                      </button>
                    </div>
                  </li>
                );
              })}
            </ul>
          </div>
        ))}
      </nav>

      <LogoutButton />
    </aside>
  );
};

export default SettingsSidebar;
