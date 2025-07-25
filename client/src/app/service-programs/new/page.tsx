"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import ServiceProgramHeader from "@/app/_features/service-program/components/ServiceProgramHeader";
import ServiceProgramDetailsForm, {
  ServiceProgramDetailsFormValues,
} from "@/app/_features/service-program/components/ServiceProgramDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";
import { useCreateServiceProgram } from "@/app/_hooks/service-program/useServicePrograms";

const initialForm: ServiceProgramDetailsFormValues = {
  name: "",
  description: "",
};

export default function CreateServiceProgramPage() {
  const router = useRouter();
  const [form, setForm] =
    useState<ServiceProgramDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceProgramDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);
  const { mutate: createServiceProgram, isPending } = useCreateServiceProgram();

  const breadcrumbs = [
    { label: "Service Programs", href: "/service-programs" },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form.name.trim()) newErrors.name = "Name is required.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (
    field: keyof ServiceProgramDetailsFormValues,
    value: any,
  ) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const handleCancel = () => {
    router.push("/service-programs");
  };

  const handleSave = async () => {
    if (!validate()) return;
    setIsSaving(true);
    createServiceProgram(
      { name: form.name, description: form.description, isActive: true },
      {
        onSuccess: () => {
          setIsSaving(false);
          router.push("/service-programs");
        },
        onError: () => setIsSaving(false),
      },
    );
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <ServiceProgramHeader
        title="New Service Program"
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton
              onClick={handleCancel}
              disabled={isSaving || isPending}
            >
              Cancel
            </SecondaryButton>
            <PrimaryButton
              onClick={handleSave}
              disabled={isSaving || isPending}
            >
              {isSaving || isPending ? "Saving..." : "Save"}
            </PrimaryButton>
          </>
        }
      />
      <div className="px-6 pb-12 mt-4 max-w-2xl mx-auto">
        <ServiceProgramDetailsForm
          value={form}
          errors={errors}
          onChange={handleChange}
          disabled={isSaving || isPending}
        />
      </div>
      {/* Footer Actions */}
      <div className="max-w-2xl mx-auto w-full mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton
            onClick={handleCancel}
            disabled={isSaving || isPending}
          >
            Cancel
          </SecondaryButton>
          <PrimaryButton onClick={handleSave} disabled={isSaving || isPending}>
            {isSaving || isPending ? "Saving..." : "Save"}
          </PrimaryButton>
        </div>
      </div>
    </div>
  );
}
