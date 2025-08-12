"use client";

import React from "react";
import { useRouter } from "next/navigation";
import { LogOut } from "lucide-react";
import { useAuthContext } from "@/features/auth/context/AuthContext";

const LogoutButton = () => {
  const router = useRouter();
  const { logout } = useAuthContext();

  const handleLogout = () => {
    logout();
    router.push("/login");
  };

  return (
    <div className="p-2 border-t border-[var(--border)] flex-shrink-0">
      <button
        onClick={handleLogout}
        className="w-full flex items-center gap-3 px-3 py-2 rounded-md transition-all text-left text-gray-700 hover:bg-red-50 hover:text-red-600"
      >
        <LogOut size={18} />
        <span className="truncate flex-1">Log Out</span>
      </button>
    </div>
  );
};

export default LogoutButton;
