"use client";
import React, { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import { X, AlertCircle } from "lucide-react";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import { useVehicles } from "@/app/_hooks/vehicle/useVehicles";
import { useServiceProgram } from "@/app/_hooks/service-program/useServicePrograms";
import {
  useAddVehicleToServiceProgram,
  useServiceProgramVehicles,
} from "@/app/_hooks/service-program/useServiceProgramVehicles";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";
import { VehicleWithLabels } from "@/app/_hooks/vehicle/vehicleType";
import Loading from "@/app/_features/shared/feedback/Loading";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import ServiceProgramHeader from "@/app/_features/service-program/components/ServiceProgramHeader";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import FormField from "@/app/_features/shared/form/FormField";

export default function AddVehicleToServiceProgramPage() {
  const params = useParams();
  const router = useRouter();
  const serviceProgramId = params.id ? Number(params.id) : undefined;

  const [search, setSearch] = useState("");
  const [selectedVehicles, setSelectedVehicles] = useState<VehicleWithLabels[]>(
    [],
  );
  const [error, setError] = useState<string | null>(null);

  // Fetch service program details
  const {
    data: serviceProgram,
    isPending: isLoadingProgram,
    isError: isProgramError,
  } = useServiceProgram(serviceProgramId!);

  // Fetch all vehicles
  const { vehicles, isLoadingVehicles } = useVehicles({
    PageNumber: 1,
    PageSize: 100,
    Search: search,
  });

  // Fetch currently assigned vehicles to check for duplicates
  const {
    serviceProgramVehicles: assignedVehicles,
    isPending: isLoadingAssignedVehicles,
  } = useServiceProgramVehicles(
    serviceProgramId!,
    { PageSize: 1000 }, // Get all assigned vehicles
  );

  const addVehicleMutation = useAddVehicleToServiceProgram();

  // Reset state when component mounts
  useEffect(() => {
    setSelectedVehicles([]);
    setError(null);
    setSearch("");
  }, []);

  // Get IDs of currently assigned vehicles
  const assignedVehicleIds = assignedVehicles.map(spv => spv.vehicleID);

  // Filter vehicles based on search
  const filteredVehicles = vehicles.filter(
    vehicle =>
      vehicle.name.toLowerCase().includes(search.toLowerCase()) ||
      vehicle.make.toLowerCase().includes(search.toLowerCase()) ||
      vehicle.model.toLowerCase().includes(search.toLowerCase()) ||
      vehicle.vin.toLowerCase().includes(search.toLowerCase()) ||
      vehicle.licensePlate.toLowerCase().includes(search.toLowerCase()),
  );

  const handleRemoveVehicle = (vehicleId: number) => {
    setSelectedVehicles(prev => prev.filter(v => v.id !== vehicleId));
    setError(null);
  };

  const handleAssignVehicles = async () => {
    if (selectedVehicles.length === 0) {
      setError("Please select at least one vehicle to assign.");
      return;
    }

    try {
      // Assign each selected vehicle
      const assignPromises = selectedVehicles.map(vehicle =>
        addVehicleMutation.mutateAsync({
          serviceProgramID: serviceProgramId!,
          vehicleID: vehicle.id,
        }),
      );

      await Promise.all(assignPromises);

      // Navigate back to service program details
      router.push(`/service-programs/${serviceProgramId}`);
    } catch (error) {
      setError("Failed to assign vehicles. Please try again.");
      console.error("Error assigning vehicles:", error);
    }
  };

  const handleBack = () => {
    router.push(`/service-programs/${serviceProgramId}`);
  };

  if (isLoadingProgram || isLoadingAssignedVehicles) {
    return <Loading />;
  }

  if (isProgramError || !serviceProgram) {
    return (
      <EmptyState
        title="Service Program not found"
        message="The service program you are looking for does not exist or could not be loaded."
      />
    );
  }

  const breadcrumbs = [
    { label: "Service Programs", href: "/service-programs" },
    {
      label: serviceProgram.name,
      href: `/service-programs/${serviceProgramId}`,
    },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      <ServiceProgramHeader
        title="New Service Program Vehicles"
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton onClick={handleBack}>Cancel</SecondaryButton>
            <PrimaryButton
              onClick={handleAssignVehicles}
              disabled={
                selectedVehicles.length === 0 || addVehicleMutation.isPending
              }
              className="ml-2"
            >
              {addVehicleMutation.isPending
                ? "Assigning..."
                : "Add Service Program Vehicles"}
            </PrimaryButton>
          </>
        }
      />

      <div className="mx-auto max-w-4xl px-6 py-8">
        {/* Error Message */}
        {error && (
          <div className="mb-6 rounded-md bg-red-50 p-4">
            <div className="flex">
              <AlertCircle className="h-5 w-5 text-red-400" />
              <div className="ml-3">
                <p className="text-sm text-red-800">{error}</p>
              </div>
            </div>
          </div>
        )}

        <FormContainer title="Add Vehicles">
          {/* Vehicle Combobox */}
          <FormField label="Vehicles">
            <Combobox
              multiple
              value={selectedVehicles}
              onChange={(vehicles: VehicleWithLabels[]) =>
                setSelectedVehicles(vehicles)
              }
              disabled={isLoadingVehicles}
            >
              <div className="relative">
                <ComboboxInput
                  className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                  displayValue={() => ""}
                  onChange={e => setSearch(e.target.value)}
                  placeholder="Select or search vehicles..."
                  disabled={isLoadingVehicles}
                />
                <ComboboxButton className="absolute inset-y-0 right-0 flex items-center pr-2">
                  <svg
                    className="h-5 w-5 text-gray-400"
                    viewBox="0 0 20 20"
                    fill="none"
                    stroke="currentColor"
                  >
                    <path
                      d="M7 7l3-3 3 3m0 6l-3 3-3-3"
                      strokeWidth="1.5"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  </svg>
                </ComboboxButton>
                <ComboboxOptions className="absolute z-10 mt-1 w-full bg-white border border-gray-200 rounded-3xl shadow-lg max-h-60 overflow-auto">
                  {isLoadingVehicles ? (
                    <div className="px-4 py-2 text-gray-500">
                      Loading vehicles...
                    </div>
                  ) : filteredVehicles.length === 0 ? (
                    <div className="px-4 py-2 text-gray-500">
                      {search
                        ? "No vehicles found matching your search."
                        : "No available vehicles to assign."}
                    </div>
                  ) : (
                    filteredVehicles.map(vehicle => {
                      const isAlreadyAssigned = assignedVehicleIds.includes(
                        vehicle.id,
                      );
                      return (
                        <ComboboxOption
                          key={vehicle.id}
                          value={vehicle}
                          disabled={isAlreadyAssigned}
                          className={({ active, disabled }) =>
                            `cursor-pointer select-none px-4 py-2 flex items-center ${
                              disabled
                                ? "opacity-50 cursor-not-allowed bg-gray-50"
                                : active
                                  ? "bg-blue-100"
                                  : ""
                            }`
                          }
                        >
                          <div className="flex-1 text-left">
                            <div className="font-medium text-gray-900">
                              {vehicle.name}
                              {isAlreadyAssigned && (
                                <span className="ml-2 text-xs text-orange-600 font-medium">
                                  (Already Assigned)
                                </span>
                              )}
                            </div>
                            <div className="text-gray-500">
                              {vehicle.year} {vehicle.make} {vehicle.model} •{" "}
                              {vehicle.licensePlate}
                            </div>
                            <div className="text-xs text-gray-400 mt-1">
                              {vehicle.vehicleGroupName} • {vehicle.statusLabel}
                            </div>
                          </div>
                          {selectedVehicles.some(v => v.id === vehicle.id) && (
                            <svg
                              className="h-5 w-5 text-blue-600 ml-2"
                              fill="none"
                              stroke="currentColor"
                              viewBox="0 0 24 24"
                            >
                              <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth="2"
                                d="M5 13l4 4L19 7"
                              />
                            </svg>
                          )}
                        </ComboboxOption>
                      );
                    })
                  )}
                </ComboboxOptions>
              </div>
              {/* Show selected as chips */}
              <div className="flex flex-wrap gap-2 mt-2">
                {selectedVehicles.map(vehicle => (
                  <span
                    key={vehicle.id}
                    className="inline-block bg-blue-100 text-blue-800 text-xs font-medium px-3 py-1 rounded-full"
                  >
                    {vehicle.name}
                  </span>
                ))}
              </div>
            </Combobox>
          </FormField>
        </FormContainer>

        {/* Selected Vehicles */}
        {selectedVehicles.length > 0 && (
          <div className="bg-white p-6 rounded-lg shadow-sm mt-6">
            <h3 className="text-lg font-semibold mb-3">
              Selected Vehicles ({selectedVehicles.length})
            </h3>
            <div className="space-y-3">
              {selectedVehicles.map(vehicle => (
                <div
                  key={vehicle.id}
                  className="flex items-center justify-between rounded-md border border-gray-200 bg-gray-50 px-4 py-3"
                >
                  <div className="flex items-center space-x-4">
                    <div className="flex-shrink-0">
                      <div className="size-10 rounded bg-gray-100 flex items-center justify-center text-sm">
                        {vehicle.name.charAt(0).toUpperCase()}
                      </div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-gray-900">
                        {vehicle.name}
                      </div>
                      <div className="text-xs text-gray-500">
                        {vehicle.year} {vehicle.make} {vehicle.model} •{" "}
                        {vehicle.licensePlate}
                      </div>
                      <div className="text-xs text-gray-400 mt-1">
                        {vehicle.vehicleGroupName} • {vehicle.statusLabel}
                      </div>
                    </div>
                  </div>
                  <button
                    onClick={() => handleRemoveVehicle(vehicle.id)}
                    className="text-gray-400 hover:text-gray-600"
                  >
                    <X className="h-4 w-4" />
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Future: Service Reminder Configuration */}
        <div className="bg-white p-6 rounded-lg shadow-sm mt-6">
          <h3 className="text-lg font-semibold mb-2">
            Service Reminder Configuration
          </h3>
          <p className="text-sm text-gray-600 mb-4">
            Configure service reminders for the selected vehicles. This feature
            will be available in a future update.
          </p>
          <div className="rounded-md bg-gray-50 p-4">
            <p className="text-sm text-gray-500">
              Service reminder configuration will be implemented here, allowing
              you to set up maintenance schedules, alerts, and notification
              preferences for the assigned vehicles.
            </p>
          </div>
        </div>
      </div>
      {/* Footer Actions */}
      <div className="max-w-4xl mx-auto w-full mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton
            onClick={handleBack}
            disabled={addVehicleMutation.isPending}
          >
            Cancel
          </SecondaryButton>
          <PrimaryButton
            onClick={handleAssignVehicles}
            disabled={
              selectedVehicles.length === 0 || addVehicleMutation.isPending
            }
          >
            {addVehicleMutation.isPending
              ? "Assigning..."
              : "Add Service Program Vehicles"}
          </PrimaryButton>
        </div>
      </div>
    </div>
  );
}
