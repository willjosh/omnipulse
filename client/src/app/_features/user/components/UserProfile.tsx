"use client";

import React, { useState } from "react";
import { useUser } from "@/app/_hooks/user/useUser";
import { UpdateUserProfileCommand } from "@/app/_hooks/user/userTypes";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";
import { Loading } from "@/app/_features/shared/feedback";
import { User, Mail, Calendar, Shield, Edit, Save, X } from "lucide-react";

export const UserProfile: React.FC = () => {
  const { user, isLoading, updateUserProfileMutation } = useUser();
  const [isEditing, setIsEditing] = useState(false);

  const [profileForm, setProfileForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    userName: "",
  });

  React.useEffect(() => {
    if (user) {
      setProfileForm({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        userName: user.userName,
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
        userName: user.userName,
      });
    }
    setIsEditing(false);
  };

  const handleSaveProfile = async () => {
    if (!user) return;

    const updateData: UpdateUserProfileCommand = {
      id: user.id,
      ...profileForm,
    };

    try {
      await updateUserProfileMutation.mutateAsync(updateData);
      setIsEditing(false);
    } catch (error) {
      console.error("Error updating profile:", error);
    }
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

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  return (
    <div className="flex-1 p-6 max-w-4xl mx-auto">
      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 rounded-full bg-[var(--primary-color)] flex items-center justify-center text-white text-xl font-semibold">
            {user.initials}
          </div>
          <div>
            <h1 className="text-3xl font-bold text-gray-900">
              {user.fullName}
            </h1>
            <p className="text-gray-600">{user.roles?.join(", ") || "User"}</p>
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
        {/* Main Profile Information */}
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
                  <p className="py-2 text-gray-900">@{user.userName}</p>
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
                  disabled={updateUserProfileMutation.isPending}
                >
                  <Save size={14} />
                  {updateUserProfileMutation.isPending
                    ? "Saving..."
                    : "Save Changes"}
                </PrimaryButton>
              </div>
            )}
          </div>
        </div>

        {/* Sidebar Information */}
        <div className="space-y-6">
          {/* Account Details */}
          <div className="bg-white rounded-lg border border-[var(--border)] p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Account Details
            </h3>
            <div className="space-y-4">
              <div className="flex items-center gap-3">
                <Calendar size={16} className="text-gray-500" />
                <div>
                  <p className="text-sm text-gray-600">Hire Date</p>
                  <p className="text-gray-900">{formatDate(user.hireDate)}</p>
                </div>
              </div>

              <div className="flex items-center gap-3">
                <Shield size={16} className="text-gray-500" />
                <div>
                  <p className="text-sm text-gray-600">Status</p>
                  <span
                    className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                      user.isActive
                        ? "bg-green-100 text-green-800"
                        : "bg-red-100 text-red-800"
                    }`}
                  >
                    {user.isActive ? "Active" : "Inactive"}
                  </span>
                </div>
              </div>
            </div>
          </div>

          {/* Roles */}
          {user.roles && user.roles.length > 0 && (
            <div className="bg-white rounded-lg border border-[var(--border)] p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">
                Roles
              </h3>
              <div className="space-y-2">
                {user.roles.map((role, index) => (
                  <div
                    key={index}
                    className="flex items-center justify-between p-2 bg-gray-50 rounded-md"
                  >
                    <span className="text-gray-900">{role}</span>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
