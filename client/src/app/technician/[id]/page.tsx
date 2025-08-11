"use client";
import React, { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { Edit, ArrowLeft, Mail, Calendar } from "lucide-react";
import { TabNavigation } from "@/components/ui/Tabs";
import { PrimaryButton } from "@/components/ui/Button";
import { Loading } from "@/components/ui/Feedback";
import { useTechnician } from "@/features/technician/hooks/useTechnicians";

const TechnicianProfilePage = () => {
  const params = useParams();
  const router = useRouter();
  const [activeTab, setActiveTab] = useState("details");

  const technicianId = params.id as string;
  const { technician, isPending, isError } = useTechnician(technicianId);

  if (isPending) {
    return <Loading />;
  }

  if (isError || !technician) {
    return (
      <div className="min-h-screen max-w-7xl shadow border-b border-gray-200 bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-semibold text-gray-900 mb-2">
            Technician Not Found
          </h2>
          <p className="text-gray-600 mb-4">
            {"The technician you're looking for doesn't exist."}
          </p>
          <PrimaryButton onClick={() => router.push("/technician")}>
            Back to Technicians
          </PrimaryButton>
        </div>
      </div>
    );
  }

  const tabs = [
    { key: "details", label: "Details", count: undefined },
    { key: "assignments", label: "Vehicle Assignments", count: undefined },
    { key: "workorders", label: "Work Orders", count: undefined },
    { key: "performance", label: "Performance", count: undefined },
    { key: "documents", label: "Documents", count: undefined },
  ];

  const handleEdit = () => {
    router.push(`/technician/${technicianId}/edit`);
  };

  const handleBack = () => {
    router.push("/technician");
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  const getYearsOfService = (hireDate: string) => {
    const hired = new Date(hireDate);
    const now = new Date();
    const years = now.getFullYear() - hired.getFullYear();
    const months = now.getMonth() - hired.getMonth();

    if (months < 0 || (months === 0 && now.getDate() < hired.getDate())) {
      return years - 1;
    }
    return years;
  };

  const renderDetailTab = () => (
    <div className="grid grid-cols-2 gap-6">
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900">
              Personal Information
            </h2>
            <button className="text-sm text-gray-500">All Fields</button>
          </div>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Full Name</span>
            <span className="text-sm text-gray-900">{`${technician.firstName} ${technician.lastName}`}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              First Name
            </span>
            <span className="text-sm text-gray-900">
              {technician.firstName}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Last Name</span>
            <span className="text-sm text-gray-900">{technician.lastName}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Email</span>
            <span className="text-sm text-blue-600">{technician.email}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Hire Date</span>
            <span className="text-sm text-gray-900">
              {formatDate(technician.hireDate)}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Years of Service
            </span>
            <span className="text-sm text-gray-900">
              {getYearsOfService(technician.hireDate)} years
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Status</span>
            <div className="flex items-center">
              <div
                className={`w-2 h-2 rounded-full mr-2 ${
                  technician.isActive ? "bg-green-500" : "bg-red-500"
                }`}
              ></div>
              <span className="text-sm text-gray-900">
                {technician.isActive ? "Active" : "Inactive"}
              </span>
            </div>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Employee ID
            </span>
            <span className="text-sm text-gray-900">{technician.id}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Department
            </span>
            <span className="text-sm text-gray-900">Maintenance</span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">Role</span>
            <span className="text-sm text-gray-900">Technician</span>
          </div>
        </div>
      </div>
      <div className="space-y-6">
        <div className="bg-white rounded-3xl border border-gray-200">
          <div className="p-4 border-b border-gray-200">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-semibold text-gray-900">
                Recent Work Orders
              </h3>
              <div className="flex items-center space-x-4 text-sm text-primary hover:text-blue-700">
                <button>View All</button>
              </div>
            </div>
          </div>
          <div className="p-4 text-center text-gray-500">
            <p className="text-sm">{`No recent work orders ${"(Future Implementation)"}`}</p>
          </div>
        </div>
        <div className="bg-white rounded-3xl border border-gray-200">
          <div className="p-4 border-b border-gray-200">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-semibold text-gray-900">
                Vehicle Assignments
              </h3>
              <div className="flex items-center space-x-4 text-sm text-primary hover:text-blue-700">
                <button>View All</button>
              </div>
            </div>
          </div>
          <div className="p-4 text-center text-gray-500">
            <p className="text-sm">{`No vehicles currently assigned ${"(Future Implementation)"}`}</p>
          </div>
        </div>
      </div>
    </div>
  );

  const renderOtherTabs = (text: string) => (
    <div className="bg-white rounded-3xl border border-gray-200 p-8 text-center">
      <p className="text-gray-500">{text}</p>
    </div>
  );

  const renderTab = () => {
    switch (activeTab) {
      case "details":
        return renderDetailTab();
      case "assignments":
        return renderOtherTabs("Vehicle assignments coming soon");
      case "workorders":
        return renderOtherTabs("Work orders coming soon");
      case "performance":
        return renderOtherTabs("Performance coming soon");
      case "documents":
        return renderOtherTabs("Documents coming soon");
    }
  };

  return (
    <div className="min-h-screen shadow border-b border-gray-200 bg-gray-50">
      <div className="bg-white">
        <div className="px-6 py-4">
          <div className="flex items-center space-x-4 mb-4">
            <button
              onClick={handleBack}
              className="flex items-center text-gray-600 hover:text-blue-500"
            >
              <ArrowLeft className="w-4 h-4 mr-1" />
              <span className="text-sm">Technicians</span>
            </button>
          </div>
          <div className="flex items-start justify-between">
            <div className="flex items-start space-x-4">
              <div className="w-22 h-22 bg-primary rounded-3xl flex items-center justify-center text-white font-semibold text-lg">
                {getInitials(technician.firstName, technician.lastName)}
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900 mb-1">
                  {`${technician.firstName} ${technician.lastName}`}
                </h1>
                <p className="text-gray-600 mb-2">
                  {technician.email} • Hired {formatDate(technician.hireDate)}
                </p>
                <div className="flex items-center space-x-4 text-sm">
                  <span
                    className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                      technician.isActive
                        ? "bg-green-100 text-green-800"
                        : "bg-red-100 text-red-800"
                    }`}
                  >
                    <div
                      className={`w-1.5 h-1.5 rounded-full mr-1.5 ${
                        technician.isActive ? "bg-green-500" : "bg-red-500"
                      }`}
                    ></div>
                    {technician.isActive ? "Active" : "Inactive"}
                  </span>
                  <span className="text-gray-600">
                    {getYearsOfService(technician.hireDate)} years of service
                  </span>
                  <span className="text-gray-600">Maintenance</span>
                  <span className="text-gray-500">Technician</span>
                </div>
              </div>
            </div>
            <div className="flex items-center space-x-3">
              <PrimaryButton onClick={handleEdit}>
                <Edit className="w-4 h-4 mr-2" />
                Edit Technician
              </PrimaryButton>
            </div>
          </div>
        </div>
        <div className="w-full px-6">
          <TabNavigation
            tabs={tabs}
            activeTab={activeTab}
            onTabChange={setActiveTab}
          />
        </div>
        <div className="flex justify-center bg-gray-50">
          <div className="w-full max-w-7xl p-6">{renderTab()}</div>
        </div>
      </div>
    </div>
  );
};

export default TechnicianProfilePage;
