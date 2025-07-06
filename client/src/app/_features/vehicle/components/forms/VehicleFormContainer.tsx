"use client";
import React, { useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import { useVehicleFormStore } from "../../store/VehicleFormStore";
import { FormSection } from "../../types/VehicleFormTypes";
import { MOCK_VEHICLES } from "../../types/VehicleListTypes";
import VehicleDetailsForm from "./VehicleDetailsForm";
import VehicleMaintenanceForm from "./VehicleMaintenanceForm";
import VehicleLifecycleForm from "./VehicleLifecycleForm";
import VehicleFinancialForm from "./VehicleFinancialForm";
import VehicleSpecificationsForm from "./VehicleSpecificationsForm";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import SecondaryButton from "@/app/_features/shared/button/SecondaryButton";

const steps = [
  { section: FormSection.DETAILS, label: "Details", isRequired: true },
  { section: FormSection.MAINTENANCE, label: "Maintenance", isRequired: false },
  { section: FormSection.LIFECYCLE, label: "Lifecycle", isRequired: false },
  { section: FormSection.FINANCIAL, label: "Financial", isRequired: false },
  {
    section: FormSection.SPECIFICATIONS,
    label: "Specifications",
    isRequired: false,
  },
];

interface VehicleFormContainerProps {
  mode: "create" | "edit";
}

const VehicleFormContainer: React.FC<VehicleFormContainerProps> = ({
  mode,
}) => {
  const router = useRouter();
  const params = useParams();
  const {
    currentSection,
    setCurrentSection,
    isDetailsComplete,
    setShowValidation,
    resetForm,
    initializeForEdit,
    initializeForCreate,
    mode: storeMode,
    vehicleId,
  } = useVehicleFormStore();

  // depends on what mode
  useEffect(() => {
    if (mode === "create") {
      initializeForCreate();
    } else if (mode === "edit" && params.id) {
      const vehicleIdFromParams = params.id as string;
      const vehicleData = MOCK_VEHICLES.find(v => v.id === vehicleIdFromParams);

      if (vehicleData) {
        initializeForEdit(vehicleIdFromParams, vehicleData);
      } else {
        console.error(`Vehicle with ID ${vehicleIdFromParams} not found`);
        router.push("/vehicles");
      }
    }
  }, [mode, params.id, initializeForEdit, initializeForCreate, router]);

  const currentStepIndex = steps.findIndex(
    step => step.section === currentSection,
  );
  const isFirstStep = currentStepIndex === 0;
  const isLastStep = currentStepIndex === steps.length - 1;

  const handleNext = () => {
    if (!isLastStep) {
      const nextStep = steps[currentStepIndex + 1];
      setCurrentSection(nextStep.section);
    }
  };

  const handlePrevious = () => {
    if (!isFirstStep) {
      const previousStep = steps[currentStepIndex - 1];
      setCurrentSection(previousStep.section);
    }
  };

  const handleStepClick = (section: FormSection) => {
    if (section !== FormSection.DETAILS && !isDetailsComplete()) {
      setShowValidation(true);
      return;
    }
    setCurrentSection(section);
  };

  const handleSaveAndContinue = () => {
    if (!isDetailsComplete()) {
      setShowValidation(true);
      return;
    }
    setShowValidation(false);
    // TODO: add TanStack React Query for API calls
    console.log(`${storeMode === "create" ? "Save" : "Update"} and continue`);
    handleNext();
  };

  const handleSaveVehicle = () => {
    if (!isDetailsComplete()) {
      setShowValidation(true);
      return;
    }
    setShowValidation(false);
    // TODO: add TanStack React Query for API calls
    if (storeMode === "create") {
      console.log("Save vehicle");
    } else {
      console.log(`Update vehicle ${vehicleId}`);
    }
    resetForm();
    router.push("/vehicles");
  };

  const handleCancel = () => {
    resetForm();
    console.log(
      `Cancel ${storeMode === "create" ? "adding new" : "editing"} vehicle`,
    );
    router.push("/vehicles");
  };

  const renderCurrentSection = () => {
    switch (currentSection) {
      case FormSection.DETAILS:
        return <VehicleDetailsForm />;
      case FormSection.MAINTENANCE:
        return <VehicleMaintenanceForm />;
      case FormSection.LIFECYCLE:
        return <VehicleLifecycleForm />;
      case FormSection.FINANCIAL:
        return <VehicleFinancialForm />;
      case FormSection.SPECIFICATIONS:
        return <VehicleSpecificationsForm />;
      default:
        return <VehicleDetailsForm />;
    }
  };

  const pageTitle =
    storeMode === "create"
      ? "Add New Vehicle"
      : `Edit Vehicle${vehicleId ? ` - ${vehicleId}` : ""}`;
  const saveButtonText =
    storeMode === "create" ? "Save Vehicle" : "Update Vehicle";

  return (
    <div className="max-w-4xl mx-auto my-16">
      {/* Page Title */}
      <div className="mb-6">
        <h1 className="text-2xl font-semibold text-gray-900">{pageTitle}</h1>
      </div>

      {/* Step Indicator */}
      <div className="mb-8">
        <div className="flex items-center justify-between">
          {steps.map((step, index) => {
            const isActive = currentSection === step.section;
            const isCompleted =
              index < currentStepIndex ||
              (step.section === FormSection.DETAILS && isDetailsComplete());

            const isAccessible =
              step.section === FormSection.DETAILS || isDetailsComplete();

            return (
              <div key={step.section} className="flex items-center">
                {/* Show form progress */}
                <div className="flex items-center">
                  <button
                    onClick={() => handleStepClick(step.section)}
                    disabled={!isAccessible}
                    className={`
                      flex items-center justify-center w-8 h-8 rounded-full text-sm font-medium transition-colors
                      ${
                        isCompleted || isActive
                          ? "bg-blue-600 text-white hover:bg-blue-700"
                          : isAccessible
                            ? "bg-gray-100 text-gray-600 border-2 border-gray-300 hover:bg-gray-200"
                            : "bg-gray-100 text-gray-400 border-2 border-gray-200 cursor-not-allowed"
                      }
                    `}
                  >
                    <span>{index + 1}</span>
                  </button>
                  <button
                    onClick={() => handleStepClick(step.section)}
                    disabled={!isAccessible}
                    className="ml-3"
                  >
                    <div
                      className={`text-sm font-medium transition-colors ${
                        isActive
                          ? "text-blue-600"
                          : isAccessible
                            ? "text-gray-900 hover:text-blue-600"
                            : "text-gray-400 cursor-not-allowed"
                      }`}
                    >
                      {step.label}
                      {!step.isRequired && (
                        <span className="text-gray-500 text-xs ml-1">
                          (Optional)
                        </span>
                      )}
                    </div>
                  </button>
                </div>

                {index < steps.length - 1 && (
                  <div
                    className={`flex-1 h-0.5 mx-4 ${isCompleted ? "bg-blue-600" : "bg-gray-200"}`}
                  />
                )}
              </div>
            );
          })}
        </div>
      </div>

      {/* Show Form details */}
      <div className="rounded-2xl p-6 bg-white">
        {renderCurrentSection()}

        <div className="flex justify-between items-center pt-6 mt-6 border-t border-gray-200">
          <div className="flex space-x-3">
            <SecondaryButton onClick={handleCancel}>
              <span>Cancel</span>
            </SecondaryButton>

            {!isFirstStep && (
              <SecondaryButton onClick={handlePrevious}>
                <span>Previous</span>
              </SecondaryButton>
            )}
          </div>

          <div className="flex space-x-3">
            {!isLastStep && (
              <PrimaryButton onClick={handleSaveAndContinue}>
                <span>Continue</span>
              </PrimaryButton>
            )}

            <PrimaryButton onClick={handleSaveVehicle}>
              <span>{saveButtonText}</span>
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VehicleFormContainer;
