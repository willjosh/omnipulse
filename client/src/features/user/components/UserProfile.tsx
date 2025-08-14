"use client";

import React, { useState } from "react";
import { useAuthContext } from "@/features/auth/context/AuthContext";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { Loading } from "@/components/ui/Feedback";
import { User, Mail, Shield, Edit, Save, X } from "lucide-react";
import {
  getUserInitials,
  getUserDisplayName,
  getFormattedRole,
} from "@/utils/userUtils";
import { Info } from "lucide-react";

export const UserProfile: React.FC = () => {
  const { user, isLoading } = useAuthContext();
  const [isEditing, setIsEditing] = useState(false);

  const [profileForm, setProfileForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    userName: "",
  });

  React.useEffect(() => {
    console.log("user", user);
    if (user) {
      setProfileForm({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        userName: user.email,
      });
    }
  }, [user]);

  const handleEditClick = () => {
    setIsEditing(true);
  };

  const handleCancelEdit = () => {
    if (user) {
      setProfileForm({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        userName: user.email,
      });
    }
    setIsEditing(false);
  };

  const handleSaveProfile = async () => {
    if (!user) return;
    console.log("Profile update not implemented yet:", profileForm);
    setIsEditing(false);
  };

  if (isLoading) {
    return <Loading />;
  }

  if (!user) {
    return (
      <div className="flex items-center justify-center h-64">
        <p className="text-gray-500">Unable to load user profile</p>
      </div>
    );
  }

  return (
    <div className="flex-1 p-6 max-w-4xl mx-auto">
      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 rounded-full bg-[var(--primary-color)] flex items-center justify-center text-white text-xl font-semibold">
            {getUserInitials(user.firstName, user.lastName)}
          </div>
          <div>
            <h1 className="text-3xl font-bold text-gray-900">
              {getUserDisplayName(user)}
            </h1>
            <p className="text-gray-600">{getFormattedRole(user)}</p>
          </div>
        </div>

        {!isEditing && (
          <PrimaryButton onClick={handleEditClick}>
            <Edit size={14} />
            Edit Profile
          </PrimaryButton>
        )}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2 space-y-6">
          {/* Personal Information Card */}
          <div className="bg-white rounded-lg border border-[var(--border)] p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl font-semibold text-gray-900 flex items-center gap-2">
                <User size={20} />
                Personal Information
              </h2>
            </div>

            <div className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    First Name
                  </label>
                  {isEditing ? (
                    <input
                      type="text"
                      value={profileForm.firstName}
                      onChange={e =>
                        setProfileForm({
                          ...profileForm,
                          firstName: e.target.value,
                        })
                      }
                      className="w-full px-3 py-2 border border-[var(--border)] rounded-md focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
                    />
                  ) : (
                    <p className="py-2 text-gray-900">{user.firstName}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Last Name
                  </label>
                  {isEditing ? (
                    <input
                      type="text"
                      value={profileForm.lastName}
                      onChange={e =>
                        setProfileForm({
                          ...profileForm,
                          lastName: e.target.value,
                        })
                      }
                      className="w-full px-3 py-2 border border-[var(--border)] rounded-md focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
                    />
                  ) : (
                    <p className="py-2 text-gray-900">{user.lastName}</p>
                  )}
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Email Address
                </label>
                {isEditing ? (
                  <input
                    type="email"
                    value={profileForm.email}
                    onChange={e =>
                      setProfileForm({ ...profileForm, email: e.target.value })
                    }
                    className="w-full px-3 py-2 border border-[var(--border)] rounded-md focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
                  />
                ) : (
                  <p className="py-2 text-gray-900 flex items-center gap-2">
                    <Mail size={16} />
                    {user.email}
                  </p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Username
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={profileForm.userName}
                    onChange={e =>
                      setProfileForm({
                        ...profileForm,
                        userName: e.target.value,
                      })
                    }
                    className="w-full px-3 py-2 border border-[var(--border)] rounded-md focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] focus:border-transparent"
                  />
                ) : (
                  <p className="py-2 text-gray-900">@{user.email}</p>
                )}
              </div>
            </div>

            {isEditing && (
              <div className="flex justify-end gap-3 pt-4 mt-4 border-t border-[var(--border)]">
                <SecondaryButton
                  onClick={handleCancelEdit}
                  className="flex items-center gap-2"
                >
                  <X size={14} />
                  Cancel
                </SecondaryButton>
                <PrimaryButton
                  onClick={handleSaveProfile}
                  className="flex items-center gap-2"
                  disabled={false}
                >
                  <Save size={14} />
                  Save Changes
                </PrimaryButton>
              </div>
            )}
          </div>
        </div>

        <div className="space-y-6">
          {/* Account Details */}
          <div className="bg-white rounded-lg border border-[var(--border)] p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Account Details
            </h3>
            <div className="space-y-4">
              <div className="flex items-center gap-3">
                <Shield size={16} className="text-gray-500" />
                <div>
                  <p className="text-sm text-gray-600">Role</p>
                  <p className="text-gray-900">{getFormattedRole(user)}</p>
                </div>
              </div>

              <div className="flex items-center gap-3">
                <Mail size={16} className="text-gray-500" />
                <div>
                  <p className="text-sm text-gray-600">User ID</p>
                  <p className="text-gray-900 font-mono text-sm">{user.id}</p>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg border border-[var(--border)] p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Role Information
            </h3>
            <div className="space-y-2">
              <div className="flex items-center justify-between p-2 bg-gray-50 rounded-md">
                <span className="text-gray-900">{getFormattedRole(user)}</span>
                <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                  Active
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
