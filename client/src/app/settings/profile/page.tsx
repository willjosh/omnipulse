"use client";
import React from "react";
import { UserProfile } from "@/app/_features/user/components/UserProfile";

const ProfilePage = () => {
  return (
    <div className="flex justify-center min-h-screen overflow-hidden">
      <UserProfile />
    </div>
  );
};

export default ProfilePage;
