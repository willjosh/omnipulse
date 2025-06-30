"use client";
import React from "react";
import { useRouter } from "next/navigation";
import { useVehicleFormStore } from "../../store/VehicleFormStore";
import { FormSection } from "../../types/VehicleFormTypes";
import VehicleDetailsForm from "./VehicleDetailsForm";
import VehicleMaintenanceForm from "./VehicleMaintenanceForm";
import VehicleLifecycleForm from "./VehicleLifecycleForm";
import VehicleFinancialForm from "./VehicleFinancialForm";
import VehicleSpecificationsForm from "./VehicleSpecificationsForm";

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

const VehicleFormContainer: React.FC = () => {
  const router = useRouter();
  const {
    currentSection,
    setCurrentSection,
    isDetailsComplete,
    setShowValidation,
    resetForm,
  } = useVehicleFormStore();

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
    // add TanStack React Query
    console.log("Save and continue");
    handleNext();
  };

  const handleSaveVehicle = () => {
    if (!isDetailsComplete()) {
      setShowValidation(true);
      return;
    }
    setShowValidation(false);
    // add TanStack React Query
    console.log("Save vehicle");
    resetForm();
    router.push("/vehicles");
  };

  const handleCancel = () => {
    resetForm();
    console.log("Cancel adding new vehicle");
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

  return (
    <div className="max-w-4xl mx-auto my-16">
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
                {/* Progress Circle */}
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

      {/* Form Details */}
      <div className="rounded-2xl p-6 bg-white">
        {renderCurrentSection()}

        <div className="flex justify-between items-center pt-6 mt-6 border-t border-gray-200">
          <div className="flex space-x-3">
            <button
              onClick={handleCancel}
              className="px-6 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              Cancel
            </button>

            {!isFirstStep && (
              <button
                onClick={handlePrevious}
                className="px-6 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                Previous
              </button>
            )}
          </div>

          <div className="flex space-x-3">
            {!isLastStep && (
              <button
                onClick={handleSaveAndContinue}
                className="px-6 py-2 text-sm font-medium text-white bg-[var(--primary-color)] rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Continue
              </button>
            )}

            <button
              onClick={handleSaveVehicle}
              className="px-6 py-2 text-sm font-medium text-white bg-[var(--primary-color)] rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Save Vehicle
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VehicleFormContainer;
