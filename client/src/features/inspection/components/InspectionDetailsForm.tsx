import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import {
  useVehicles,
  useTechnicians,
} from "@/features/vehicle/hooks/useVehicles";
import { VehicleWithLabels } from "@/features/vehicle/types/vehicleType";
import { Technician } from "@/features/technician/types/technicianType";

export interface InspectionDetailsFormValues {
  vehicleID: number;
  technicianID: string;
}

interface InspectionDetailsFormProps {
  value: InspectionDetailsFormValues;
  errors: Partial<Record<keyof InspectionDetailsFormValues, string>>;
  onChange: (field: keyof InspectionDetailsFormValues, value: any) => void;
  disabled?: boolean;
}

const InspectionDetailsForm: React.FC<InspectionDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  // Fetch vehicles and technicians
  const { vehicles, isPending: isLoadingVehicles } = useVehicles();
  const { technicians, isPending: isLoadingTechnicians } = useTechnicians();

  // Vehicle selection state
  const [vehicleSearch, setVehicleSearch] = React.useState("");
  const filteredVehicles = React.useMemo(() => {
    if (!vehicleSearch) return vehicles;
    const searchLower = vehicleSearch.toLowerCase();
    return vehicles.filter(vehicle =>
      `${vehicle.name} ${vehicle.make} ${vehicle.model} ${vehicle.licensePlate}`
        .toLowerCase()
        .includes(searchLower),
    );
  }, [vehicles, vehicleSearch]);
  const selectedVehicle = vehicles.find(v => v.id === value.vehicleID) || null;

  // Technician selection state
  const [technicianSearch, setTechnicianSearch] = React.useState("");
  const filteredTechnicians = React.useMemo(() => {
    if (!technicianSearch) return technicians;
    const searchLower = technicianSearch.toLowerCase();
    return technicians.filter(technician =>
      `${technician.firstName} ${technician.lastName} ${technician.email}`
        .toLowerCase()
        .includes(searchLower),
    );
  }, [technicians, technicianSearch]);
  const selectedTechnician =
    technicians.find(t => t.id === value.technicianID) || null;

  return (
    <FormContainer title="Inspection Details">
      <FormField label="Vehicle" required error={errors.vehicleID}>
        <Combobox
          value={selectedVehicle}
          onChange={vehicle => {
            if (vehicle) onChange("vehicleID", vehicle.id);
          }}
          disabled={disabled || isLoadingVehicles}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(vehicle: VehicleWithLabels | null) =>
                vehicle
                  ? `${vehicle.name} - ${vehicle.make} ${vehicle.model} (${vehicle.licensePlate})`
                  : ""
              }
              onChange={event => setVehicleSearch(event.target.value)}
              placeholder="Search for a vehicle..."
            />
            <ComboboxButton className="absolute inset-y-0 right-0 flex items-center pr-2">
              <svg
                className="h-5 w-5 text-gray-400"
                viewBox="0 0 20 20"
                fill="currentColor"
                aria-hidden="true"
              >
                <path
                  fillRule="evenodd"
                  d="M10 3a1 1 0 01.707.293l3 3a1 1 0 01-1.414 1.414L10 5.414 7.707 7.707a1 1 0 01-1.414-1.414l3-3A1 1 0 0110 3zm-3.707 9.293a1 1 0 011.414 0L10 14.586l2.293-2.293a1 1 0 011.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z"
                  clipRule="evenodd"
                />
              </svg>
            </ComboboxButton>
            <ComboboxOptions className="absolute z-10 mt-1 w-full bg-white border border-gray-200 rounded-3xl shadow-lg max-h-60 overflow-auto">
              {filteredVehicles.length === 0 ? (
                <div className="relative cursor-default select-none py-2 px-4 text-gray-700">
                  No vehicles found.
                </div>
              ) : (
                filteredVehicles.map(vehicle => (
                  <ComboboxOption
                    key={vehicle.id}
                    value={vehicle}
                    className={({ active, selected }) =>
                      `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                    }
                  >
                    {({ selected }) => (
                      <>
                        <span className="flex-1">
                          {vehicle.name} - {vehicle.make} {vehicle.model} (
                          {vehicle.licensePlate})
                        </span>
                        {selected && (
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
                      </>
                    )}
                  </ComboboxOption>
                ))
              )}
            </ComboboxOptions>
          </div>
        </Combobox>
      </FormField>

      <FormField label="Technician" required error={errors.technicianID}>
        <Combobox
          value={selectedTechnician}
          onChange={technician => {
            if (technician) onChange("technicianID", technician.id);
          }}
          disabled={disabled || isLoadingTechnicians}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(technician: Technician | null) =>
                technician
                  ? `${technician.firstName} ${technician.lastName}`
                  : ""
              }
              onChange={event => setTechnicianSearch(event.target.value)}
              placeholder="Search for a technician..."
            />
            <ComboboxButton className="absolute inset-y-0 right-0 flex items-center pr-2">
              <svg
                className="h-5 w-5 text-gray-400"
                viewBox="0 0 20 20"
                fill="currentColor"
                aria-hidden="true"
              >
                <path
                  fillRule="evenodd"
                  d="M10 3a1 1 0 01.707.293l3 3a1 1 0 01-1.414 1.414L10 5.414 7.707 7.707a1 1 0 01-1.414-1.414l3-3A1 1 0 0110 3zm-3.707 9.293a1 1 0 011.414 0L10 14.586l2.293-2.293a1 1 0 011.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z"
                  clipRule="evenodd"
                />
              </svg>
            </ComboboxButton>
            <ComboboxOptions className="absolute z-10 mt-1 w-full bg-white border border-gray-200 rounded-3xl shadow-lg max-h-60 overflow-auto">
              {filteredTechnicians.length === 0 ? (
                <div className="relative cursor-default select-none py-2 px-4 text-gray-700">
                  No technicians found.
                </div>
              ) : (
                filteredTechnicians.map(technician => (
                  <ComboboxOption
                    key={technician.id}
                    value={technician}
                    className={({ active, selected }) =>
                      `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                    }
                  >
                    {({ selected }) => (
                      <>
                        <span className="flex-1">
                          {technician.firstName} {technician.lastName}
                        </span>
                        {selected && (
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
                      </>
                    )}
                  </ComboboxOption>
                ))
              )}
            </ComboboxOptions>
          </div>
        </Combobox>
      </FormField>
    </FormContainer>
  );
};

export default InspectionDetailsForm;
