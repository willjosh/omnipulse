"use client";
import React from "react";
import { useRouter } from "next/navigation";
import { Search, Bell, HelpCircle, Plus } from "lucide-react";

const NavBar = () => {
  const router = useRouter();
  const handleLogoClick = () => {
    router.push("/");
  };

  return (
    <nav className="fixed top-0 left-0 right-0 z-50 bg-white border-b border-gray-200 px-6 py-3 h-16 flex-shrink-0">
      <div className="flex items-center justify-between h-full">
        {/* Left side - Logo */}
        <div className="flex items-center w-1/6">
          <h1
            className="text-4xl font-bold text-primary cursor-pointer"
            onClick={handleLogoClick}
          >
            omnipulse
          </h1>
        </div>

        {/* Center - Search bar
        <div className="flex-1 mx-8">
          <div className="relative w-full">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted size-5" />
            <input
              type="text"
              placeholder="Search"
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent bg-gray-50"
            />
          </div>
        </div> */}

        {/* Right side - Icons */}
        <div className="flex items-center space-x-2">
          {/* <button className="p-2 text-muted hover:text-gray-800 hover:bg-gray-100 rounded-lg transition-colors">
            <Bell className="size-5" />
          </button> */}
          <button className="p-2 text-muted hover:text-gray-800 hover:bg-gray-100 rounded-lg transition-colors">
            <HelpCircle className="size-5" />
          </button>
          {/* <button className="p-2 text-muted hover:text-gray-800 hover:bg-gray-100 rounded-lg transition-colors">
            <Plus className="size-5" />
          </button> */}
        </div>
      </div>
    </nav>
  );
};

export default NavBar;
