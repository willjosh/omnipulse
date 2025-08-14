import { ServiceScheduleWithLabels } from "@/features/service-schedule/types/serviceScheduleType";
import React from "react";

export const serviceScheduleTableColumns = [
  {
    key: "name",
    header: "Name",
    width: "200px",
    sortable: true,
    render: (schedule: ServiceScheduleWithLabels) => <div>{schedule.name}</div>,
  },
  {
    key: "scheduleType",
    header: "Schedule Type",
    width: "140px",
    sortable: false,
    render: (schedule: ServiceScheduleWithLabels) => (
      <div>{schedule.scheduleTypeLabel}</div>
    ),
  },
  {
    key: "frequency",
    header: "Frequency",
    width: "150px",
    sortable: false,
    render: (schedule: ServiceScheduleWithLabels) => {
      if (schedule.timeIntervalValue && schedule.timeIntervalUnitLabel) {
        return (
          <div>
            {`${schedule.timeIntervalValue} ${
              schedule.timeIntervalValue === 1
                ? schedule.timeIntervalUnitLabel.replace(/s$/, "")
                : schedule.timeIntervalUnitLabel
            }`}
          </div>
        );
      } else if (schedule.mileageInterval) {
        return <div>{`${schedule.mileageInterval} km`}</div>;
      }
      return <div>-</div>;
    },
  },
  {
    key: "buffer",
    header: "Buffer",
    width: "150px",
    sortable: false,
    render: (schedule: ServiceScheduleWithLabels) => {
      if (schedule.timeBufferValue && schedule.timeBufferUnitLabel) {
        return (
          <div>
            {`${schedule.timeBufferValue} ${
              schedule.timeBufferValue === 1
                ? schedule.timeBufferUnitLabel.replace(/s$/, "")
                : schedule.timeBufferUnitLabel
            }`}
          </div>
        );
      } else if (schedule.mileageBuffer) {
        return <div>{`${schedule.mileageBuffer} km`}</div>;
      }
      return <div>-</div>;
    },
  },
  {
    key: "firstService",
    header: "First Service",
    width: "150px",
    sortable: false,
    render: (schedule: ServiceScheduleWithLabels) => {
      if (schedule.firstServiceDate) {
        return (
          <div>{new Date(schedule.firstServiceDate).toLocaleDateString()}</div>
        );
      } else if (schedule.firstServiceMileage) {
        return <div>{`${schedule.firstServiceMileage} km`}</div>;
      }
      return <div>-</div>;
    },
  },
];
