"use client";
import React from "react";
import { useRouter } from "next/navigation";
import { User, Layers, Settings } from "lucide-react";

const SettingsPage = () => {
  const router = useRouter();

  const settingsOptions = [
    {
      title: "Profile",
      description: "Manage your personal information and account details",
      icon: User,
      path: "/settings/profile",
    },
    {
      title: "Vehicle Groups",
      description: "Organize and manage vehicle group configurations",
      icon: Layers,
      path: "/settings/vehicle-groups",
    },
    {
      title: "Vehicle Status",
      description: "Manage vehicle status configurations",
      icon: Layers,
      path: "/settings/vehicle-status",
    },
  ];

  return (
    <div className="flex-1 p-6 max-w-4xl mx-auto">
      <div className="flex items-center gap-3 mb-8">
        <Settings className="w-8 h-8 text-[var(--primary-color)]" />
        <h1 className="text-3xl font-bold text-gray-900">Settings</h1>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {settingsOptions.map(option => {
          const IconComponent = option.icon;
          return (
            <button
              key={option.path}
              onClick={() => router.push(option.path)}
              className="p-6 bg-white rounded-lg border border-[var(--border)] hover:border-[var(--primary-color)] transition-colors text-left group"
            >
              <div className="flex items-center gap-4 mb-4">
                <div className="w-12 h-12 rounded-lg bg-[var(--primary-light)] flex items-center justify-center group-hover:bg-[var(--primary-color)] transition-colors">
                  <IconComponent
                    size={24}
                    className="text-[var(--primary-color)] group-hover:text-white transition-colors"
                  />
                </div>
                <h2 className="text-xl font-semibold text-gray-900">
                  {option.title}
                </h2>
              </div>
              <p className="text-gray-600">{option.description}</p>
            </button>
          );
        })}
      </div>
    </div>
  );
};

export default SettingsPage;
