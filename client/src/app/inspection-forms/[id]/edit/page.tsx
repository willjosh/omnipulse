"use client";
import React, { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import InspectionFormHeader from "@/features/inspection-form/components/InspectionFormHeader";
import InspectionFormDetailsForm, {
  InspectionFormDetailsFormValues,
} from "@/features/inspection-form/components/InspectionFormDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useUpdateInspectionForm } from "@/features/inspection-form/hooks/useInspectionForms";
import { useInspectionForm } from "@/features/inspection-form/hooks/useInspectionForms";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";

const initialForm: InspectionFormDetailsFormValues = {
  title: "",
  description: "",
  isActive: true,
};

export default function EditInspectionFormPage() {
  const params = useParams();
  const router = useRouter();
  const inspectionFormId = Number(params.id);

  const [form, setForm] =
    useState<InspectionFormDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof InspectionFormDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);

  const { mutate: updateInspectionForm, isPending } = useUpdateInspectionForm();
  const { inspectionForm, isPending: isFormLoading } =
    useInspectionForm(inspectionFormId);
  const notify = useNotification();

  // Populate form when inspection form data loads
  useEffect(() => {
    if (inspectionForm) {
      setForm({
        title: inspectionForm.title,
        description: inspectionForm.description || "",
        isActive: inspectionForm.isActive,
      });
    }
  }, [inspectionForm]);

  const breadcrumbs = [
    { label: "Inspection Forms", href: "/inspection-forms" },
    {
      label: inspectionForm?.title || "Loading...",
      href: `/inspection-forms/${inspectionFormId}`,
    },
    { label: "Edit" },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form.title.trim()) newErrors.title = "Title is required.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (
    field: keyof InspectionFormDetailsFormValues,
    value: any,
  ) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const handleCancel = () => {
    router.push(`/inspection-forms/${inspectionFormId}`);
  };

  const handleSave = async () => {
    if (!validate()) return;
    setIsSaving(true);
    updateInspectionForm(
      {
        inspectionFormID: inspectionFormId,
        title: form.title,
        description: form.description,
        isActive: form.isActive,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Inspection form updated successfully!", "success");
          router.push(`/inspection-forms/${inspectionFormId}`);
        },
        onError: () => setIsSaving(false),
      },
    );
  };

  if (isFormLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!inspectionForm) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Form Not Found
          </h2>
          <p className="text-gray-600 mb-6">
            The inspection form you are looking for does not exist.
          </p>
          <PrimaryButton onClick={() => router.push("/inspection-forms")}>
            Back to Forms
          </PrimaryButton>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <InspectionFormHeader
        title="Edit Inspection Form"
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
              {isSaving || isPending ? "Updating..." : "Update"}
            </PrimaryButton>
          </>
        }
      />
      <div className="px-6 pb-12 mt-4 max-w-2xl mx-auto">
        <InspectionFormDetailsForm
          value={form}
          errors={errors}
          onChange={handleChange}
          disabled={isSaving || isPending}
        />
      </div>
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
            {isSaving || isPending ? "Updating..." : "Update Inspection Form"}
          </PrimaryButton>
        </div>
      </div>
    </div>
  );
}
