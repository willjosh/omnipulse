import React from "react";
import { TabNavigation } from "@/components/ui";

interface IssueTabsProps {
  activeTab: string;
  onTabChange: (tabKey: string) => void;
  tabCounts?: {
    all?: number;
    open?: number;
    overdue?: number;
    resolved?: number;
    closed?: number;
  };
}

const tabs = [
  { key: "all", label: "All" },
  { key: "open", label: "Open" },
  { key: "overdue", label: "Overdue" },
  { key: "resolved", label: "Resolved" },
  { key: "closed", label: "Closed" },
];

export const IssueTabs: React.FC<IssueTabsProps> = ({
  activeTab,
  onTabChange,
  tabCounts,
}) => {
  const tabsWithCounts = tabs.map(tab => ({
    ...tab,
    count: tabCounts?.[tab.key as keyof typeof tabCounts],
  }));
  return (
    <TabNavigation
      tabs={tabsWithCounts}
      activeTab={activeTab}
      onTabChange={onTabChange}
    />
  );
};
