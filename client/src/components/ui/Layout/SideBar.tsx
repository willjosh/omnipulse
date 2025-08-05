"use client";

import { useState, useEffect } from "react";
import { useRouter, usePathname } from "next/navigation";
import { useAuthContext } from "@/features/auth/context/AuthContext";
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
  Settings,
} from "lucide-react";
import SettingsSidebar from "./SettingsSidebar";
import { NavChild, NavItem } from "./types";

const navItems: NavItem[] = [
  { label: "Dashboard", icon: LayoutDashboard, path: "/" },
  {
    label: "Vehicles",
    icon: Car,
    children: [
      { label: "Vehicle List", path: "/vehicles" },
      { label: "Vehicle Assignment", path: "/vehicles/assignment" },
    ],
  },
  // { label: "Equipment", icon: Wrench },
  {
    label: "Inspections",
    icon: ClipboardCheck,
    children: [
      { label: "Inspection History", path: "/inspections" },
      { label: "Forms", path: "/inspection-forms" },
    ],
  },
  { label: "Issues", icon: AlertTriangle, path: "/issues" },
  // {
  //   label: "Issues",
  //   icon: AlertTriangle,
  //   children: [
  //     { label: "Issues", path: "/issues" },
  //     // { label: "Faults", path: "/issues/faults" },
  //     // { label: "Recalls", path: "/issues/recalls" },
  //   ],
  // },
  { label: "Service Reminders", icon: Bell, path: "/reminders/service" },
  // {
  //   label: "Reminders",
  //   icon: Bell,
  //   children: [
  //     { label: "Service Reminders", path: "/reminders/service" },
  //     { label: "Vehicle Renewals", path: "/reminders/vehicle-renewals" },
  //     { label: "Contact Renewals", path: "/reminders/contact-renewals" },
  //   ],
  // },
  {
    label: "Service",
    icon: UserCog,
    children: [
      { label: "Service History", path: "/service/history" },
      { label: "Work Orders", path: "/work-orders" },
      { label: "Service Tasks", path: "/service-tasks" },
      { label: "Service Schedules", path: "/service-schedules" },
      { label: "Service Programs", path: "/service-programs" },
    ],
  },
  { label: "Technicians", icon: User, path: "/technician" },
  // { label: "Vendors", icon: Store },
  {
    label: "Inventories",
    icon: Boxes,
    children: [
      { label: "Inventory", path: "/inventory" },
      { label: "Items", path: "/inventory-item" },
    ],
  },
  // { label: "Fuel & Energy", icon: Fuel },
  // { label: "Places", icon: MapPin },
  // { label: "Documents", icon: FileText },
  // { label: "Reports", icon: BarChart },
  { label: "Settings", icon: Settings, path: "/settings" },
];

const SideBar = () => {
  const router = useRouter();
  const pathname = usePathname();
  const { user } = useAuthContext();
  const [activeParent, setActiveParent] = useState<string>("");
  const [expandedItems, setExpandedItems] = useState<Record<string, boolean>>(
    {},
  );

  // Generate user initials from first and last name
  const getUserInitials = (firstName?: string, lastName?: string) => {
    if (!firstName || !lastName) return "NN"; // fallback to current initials
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  // Get user display name
  const getUserDisplayName = () => {
    if (!user?.firstName || !user?.lastName) return "No Name"; // fallback
    return `${user.firstName} ${user.lastName}`;
  };

  // Format user role for display
  const getFormattedRole = () => {
    if (!user?.role) return "No Role"; // fallback
    return user.role === "FleetManager" ? "Fleet Manager" : user.role;
  };

  useEffect(() => {
    if (activeParent) {
      const activeParentItem = navItems.find(
        item => item.label === activeParent,
      );
      if (activeParentItem?.children) {
        const isOnActiveParentChild = activeParentItem.children.some(
          child => pathname === child.path,
        );
        if (!isOnActiveParentChild) {
          setActiveParent("");
        }
      }
    }
  }, [pathname, activeParent]);

  const isSettingsPage = pathname.startsWith("/settings");

  if (isSettingsPage) {
    return <SettingsSidebar />;
  }

  const toggleExpand = (label: string) => {
    setExpandedItems(prev => ({ ...prev, [label]: !prev[label] }));
  };

  const handleParentClick = (label: string, hasChildren: boolean) => {
    if (hasChildren) {
      setActiveParent(label);
      toggleExpand(label);
    } else {
      const item = navItems.find(item => item.label === label);
      if (item?.path) {
        router.push(item.path);
      }
    }
  };

  const handleChildClick = (child: NavChild) => {
    setActiveParent("");
    router.push(child.path);
  };

  const isParentActive = (label: string) => {
    const item = navItems.find(item => item.label === label);

    if (!item?.children) {
      return pathname === item?.path;
    }

    const hasActiveChild = item.children.some(child => pathname === child.path);
    return activeParent === label && !hasActiveChild;
  };

  const isChildActive = (child: NavChild) => {
    return pathname === child.path;
  };

  return (
    <aside className="fixed top-16 left-0 w-64 h-[calc(100vh-4rem)] bg-white border-r border-[var(--border)] text-sm flex flex-col overflow-hidden z-30 flex-shrink-0">
      {/* Header */}
      <div className="px-4 py-5 border-b border-[var(--border)] flex items-center gap-3 flex-shrink-0">
        <div className="w-9 h-9 rounded-full bg-[var(--primary-color)] flex items-center justify-center text-white font-semibold">
          {getUserInitials(user?.firstName, user?.lastName)}
        </div>
        <div>
          <div className="text-xs text-gray-500">{getFormattedRole()}</div>
          <div className="text-sm font-medium text-gray-700">
            {getUserDisplayName()}
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 overflow-y-auto">
        <ul className="mt-4 space-y-1">
          {navItems.map(({ label, icon: Icon, children }) => {
            const isActive = isParentActive(label);
            const isExpanded = expandedItems[label];

            return (
              <li key={label}>
                <div className="px-2">
                  <button
                    onClick={() => handleParentClick(label, !!children)}
                    className={`w-full flex items-center gap-3 px-3 py-2 rounded-md transition-all text-left ${
                      isActive
                        ? "bg-[var(--primary-color)] text-white"
                        : "text-gray-700 hover:bg-[var(--primary-light)] hover:text-[var(--primary-color)]"
                    }`}
                  >
                    <Icon size={18} />
                    <span className="truncate flex-1">{label}</span>
                    {!!children &&
                      (isExpanded ? (
                        <ChevronDown size={16} className="text-inherit" />
                      ) : (
                        <ChevronRight size={16} className="text-inherit" />
                      ))}
                  </button>
                </div>

                {/* Children */}
                {!!children && isExpanded && (
                  <ul className="ml-10 mt-1 space-y-1">
                    {children.map(child => (
                      <div key={child.path} className="px-2">
                        <button
                          onClick={() => handleChildClick(child)}
                          className={`w-full flex items-center gap-2 px-4 py-1.5 rounded-md transition-all text-left text-sm ${
                            isChildActive(child)
                              ? "bg-[var(--primary-color)] text-white"
                              : "text-gray-600 hover:bg-[var(--primary-light)] hover:text-[var(--primary-color)]"
                          }`}
                        >
                          <span className="truncate">{child.label}</span>
                        </button>
                      </div>
                    ))}
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
