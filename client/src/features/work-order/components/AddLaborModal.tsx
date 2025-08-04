import React, { useState, useMemo } from "react";
import ModalPortal from "@/components/ui/Modal/ModalPortal";
import FormField from "@/components/ui/Form/FormField";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import { useTechnicians } from "@/features/technician/hooks/useTechnicians";

interface AddLaborModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (labor: {
    assignedToUserID: string;
    laborHours: number;
    hourlyRate: number;
  }) => void;
  initialValues?: {
    assignedToUserID?: string;
    laborHours?: number;
    hourlyRate?: number;
  };
}

const AddLaborModal: React.FC<AddLaborModalProps> = ({
  isOpen,
  onClose,
  onSave,
  initialValues = {},
}) => {
  const [assignedToUserID, setAssignedToUserID] = useState(
    initialValues.assignedToUserID || "",
  );
  const [laborHours, setLaborHours] = useState(initialValues.laborHours || "");
  const [hourlyRate, setHourlyRate] = useState(initialValues.hourlyRate || "");
  const [technicianSearch, setTechnicianSearch] = useState("");

  const { technicians, isPending: isLoadingTechnicians } = useTechnicians({
    PageNumber: 1,
    PageSize: 100,
    Search: "",
  });

  const technicianOptions = useMemo(
    () =>
      technicians.map(
        (t: { id: string; firstName: string; lastName: string }) => ({
          value: t.id,
          label: `${t.firstName} ${t.lastName}`,
        }),
      ),
    [technicians],
  );

  const filteredTechnicians = useMemo(() => {
    if (!technicianSearch) return technicianOptions;
    const searchLower = technicianSearch.toLowerCase();
    return technicianOptions.filter(t =>
      t.label.toLowerCase().includes(searchLower),
    );
  }, [technicianSearch, technicianOptions]);

  const selectedTechnician =
    technicianOptions.find(t => t.value === assignedToUserID) || null;

  const handleSave = () => {
    const laborHoursNum = typeof laborHours === "number" ? laborHours : 0;
    const hourlyRateNum = typeof hourlyRate === "number" ? hourlyRate : 0;

    if (!assignedToUserID || laborHoursNum <= 0 || hourlyRateNum <= 0) {
      return; // Validation
    }

    onSave({
      assignedToUserID,
      laborHours: laborHoursNum,
      hourlyRate: hourlyRateNum,
    });
    onClose();
  };

  const handleClose = () => {
    setAssignedToUserID(initialValues.assignedToUserID || "");
    setLaborHours(initialValues.laborHours || "");
    setHourlyRate(initialValues.hourlyRate || "");
    setTechnicianSearch("");
    onClose();
  };

  return (
    <ModalPortal isOpen={isOpen}>
      <div className="fixed inset-0 backdrop-brightness-50 bg-opacity-50 flex items-center justify-center z-[100]">
        <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
          {/* Header */}
          <div className="px-6 py-4 border-b border-gray-200">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold text-gray-900">Add Labor</h2>
              <button
                onClick={handleClose}
                className="text-gray-400 hover:text-gray-600"
              >
                <svg
                  className="w-6 h-6"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth="2"
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </button>
            </div>
          </div>

          {/* Content */}
          <div className="px-6 py-4 space-y-4">
            {/* Technician */}
            <FormField label="Technician" required>
              <Combobox
                value={selectedTechnician}
                onChange={t => t && setAssignedToUserID(t.value)}
                disabled={isLoadingTechnicians}
              >
                <div className="relative">
                  <ComboboxInput
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                    displayValue={(
                      technician: { value: string; label: string } | null,
                    ) => technician?.label || ""}
                    onChange={e => setTechnicianSearch(e.target.value)}
                    placeholder="Search technicians..."
                    disabled={isLoadingTechnicians}
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
                  <ComboboxOptions className="absolute z-10 mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-lg max-h-60 overflow-auto">
                    {filteredTechnicians.length === 0 ? (
                      <div className="px-4 py-2 text-gray-500">
                        No technicians found.
                      </div>
                    ) : (
                      filteredTechnicians.map(opt => (
                        <ComboboxOption
                          key={opt.value}
                          value={opt}
                          className={({ active, selected }) =>
                            `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                          }
                        >
                          {({ selected }) => (
                            <>
                              <span className="flex-1">{opt.label}</span>
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

            {/* Labor Hours */}
            <FormField label="Labor Hours" required>
              <input
                type="number"
                min={0}
                step={0.1}
                value={laborHours || ""}
                onChange={e =>
                  setLaborHours(e.target.value ? Number(e.target.value) : "")
                }
                placeholder="Enter labor hours"
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              />
            </FormField>

            {/* Hourly Rate */}
            <FormField label="Hourly Rate" required>
              <input
                type="number"
                min={0}
                step={0.01}
                value={hourlyRate || ""}
                onChange={e =>
                  setHourlyRate(e.target.value ? Number(e.target.value) : "")
                }
                placeholder="Enter hourly rate"
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              />
            </FormField>

            {/* Total Cost Preview */}
            {typeof laborHours === "number" &&
              typeof hourlyRate === "number" &&
              laborHours > 0 &&
              hourlyRate > 0 && (
                <div className="bg-blue-50 p-3 rounded-lg">
                  <div className="text-sm text-blue-800">
                    <span className="font-medium">Total Labor Cost:</span> $
                    {(laborHours * hourlyRate).toFixed(2)}
                  </div>
                </div>
              )}
          </div>

          {/* Footer */}
          <div className="px-6 py-4 border-t border-gray-200 bg-gray-50">
            <div className="flex justify-end space-x-3">
              <button
                onClick={handleClose}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                onClick={handleSave}
                disabled={
                  !assignedToUserID ||
                  typeof laborHours !== "number" ||
                  laborHours <= 0 ||
                  typeof hourlyRate !== "number" ||
                  hourlyRate <= 0
                }
                className="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Add Labor
              </button>
            </div>
          </div>
        </div>
      </div>
    </ModalPortal>
  );
};

export default AddLaborModal;
