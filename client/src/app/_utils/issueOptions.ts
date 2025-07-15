import {
  IssueCategoryEnum,
  PriorityLevelEnum,
} from "../_hooks/issue/issueEnum";

export const getCategoryOptions = () => [
  { value: IssueCategoryEnum.ENGINE.toString(), label: "Engine" },
  { value: IssueCategoryEnum.TRANSMISSION.toString(), label: "Transmission" },
  { value: IssueCategoryEnum.BRAKES.toString(), label: "Brakes" },
  { value: IssueCategoryEnum.ELECTRICAL.toString(), label: "Electrical" },
  { value: IssueCategoryEnum.BODY.toString(), label: "Body" },
  { value: IssueCategoryEnum.TIRES.toString(), label: "Tires" },
  { value: IssueCategoryEnum.HVAC.toString(), label: "HVAC" },
  { value: IssueCategoryEnum.OTHER.toString(), label: "Other" },
];

export const getPriorityOptions = () => [
  { value: PriorityLevelEnum.LOW.toString(), label: "Low" },
  { value: PriorityLevelEnum.MEDIUM.toString(), label: "Medium" },
  { value: PriorityLevelEnum.HIGH.toString(), label: "High" },
  { value: PriorityLevelEnum.CRITICAL.toString(), label: "Critical" },
];
