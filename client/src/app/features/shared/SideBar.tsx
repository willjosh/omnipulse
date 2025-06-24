"use client";

import { useState } from "react";
import {
  LayoutDashboard,
  Car,
  Wrench,
  ClipboardCheck,
  AlertTriangle,
  Bell,
  UserCog,
  User,
  Store,
  Boxes,
  Fuel,
  MapPin,
  FileText,
  BarChart,
  ChevronDown,
  ChevronRight,
} from "lucide-react";

type NavItem = {
  label: string;
  icon: React.ElementType;
  hasDropdown: boolean;
};

const navItems: NavItem[] = [
  { label: "Dashboard", icon: LayoutDashboard, hasDropdown: false },
  { label: "Vehicles", icon: Car, hasDropdown: true },
  { label: "Equipment", icon: Wrench, hasDropdown: true },
  { label: "Inspections", icon: ClipboardCheck, hasDropdown: true },
  { label: "Issues", icon: AlertTriangle, hasDropdown: true },
  { label: "Reminders", icon: Bell, hasDropdown: true },
  { label: "Service", icon: UserCog, hasDropdown: true },
  { label: "Contacts", icon: User, hasDropdown: true },
  { label: "Vendors", icon: Store, hasDropdown: true },
  { label: "Parts & Inventory", icon: Boxes, hasDropdown: true },
  { label: "Fuel & Energy", icon: Fuel, hasDropdown: true },
  { label: "Places", icon: MapPin, hasDropdown: true },
  { label: "Documents", icon: FileText, hasDropdown: true },
  { label: "Reports", icon: BarChart, hasDropdown: true },
];

const SideBar = () => {
  const [activeItem, setActiveItem] = useState<string>("Vehicles");
  const [expandedItems, setExpandedItems] = useState<Record<string, boolean>>({
    Vehicles: true,
  });

  const toggleExpand = (label: string) => {
    setExpandedItems((prev) => ({
      ...prev,
      [label]: !prev[label],
    }));
  };

  const handleClick = (label: string) => {
    setActiveItem(label);
  };

  return (
    <aside className="w-64 h-screen bg-white border-r border-[var(--border)] text-sm flex flex-col">
      {/* Header */}
      <div className="px-4 py-5 border-b border-[var(--border)] flex items-center gap-3">
        <div className="w-9 h-9 rounded-full bg-[var(--primary-color)] flex items-center justify-center text-white font-semibold">
          SC
        </div>
        <div>
          <div className="text-xs text-gray-500">Ciskie Contracting</div>
          <div className="text-sm font-medium text-gray-700">Sara Ciskie</div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 overflow-y-auto">
        <ul className="mt-4 space-y-1">
          {navItems.map(({ label, icon: Icon, hasDropdown }) => {
            const isActive = activeItem === label;
            const isExpanded = expandedItems[label];

            return (
              <li key={label}>
                <div className="px-2">
                  <button
                    onClick={() => {
                      handleClick(label);
                      if (hasDropdown) toggleExpand(label);
                    }}
                    className={`w-full flex items-center gap-3 px-3 py-2 rounded-md transition-all text-left ${
                      isActive
                        ? "bg-[var(--primary-color)] text-white"
                        : "text-gray-700 hover:bg-[var(--primary-light)] hover:text-[var(--primary-color)]"
                    }`}
                  >
                    <Icon size={18} />
                    <span className="truncate flex-1">{label}</span>
                    {hasDropdown &&
                      (isExpanded ? (
                        <ChevronDown size={16} className="text-inherit" />
                      ) : (
                        <ChevronRight size={16} className="text-inherit" />
                      ))}
                  </button>
                </div>

                {/* Children */}
                {hasDropdown && isExpanded && (
                  <ul className="ml-10 mt-1 space-y-1">
                    <div className="px-2">
                      <button
                        onClick={() => handleClick(`${label} - Test`)}
                        className={`w-full flex items-center gap-2 px-4 py-1.5 rounded-md transition-all text-left text-sm ${
                          activeItem === `${label} - Test`
                            ? "bg-[var(--primary-color)] text-white"
                            : "text-gray-600 hover:bg-[var(--primary-light)] hover:text-[var(--primary-color)]"
                        }`}
                      >
                        <span className="truncate">Test</span>
                      </button>
                    </div>
                  </ul>
                )}
              </li>
            );
          })}
        </ul>
      </nav>
    </aside>
  );
};

export default SideBar;
