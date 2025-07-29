export type NavChild = { label: string; path: string };

export type NavItem = {
  label: string;
  icon: React.ElementType;
  path?: string;
  children?: NavChild[];
};

export type NavSection = { title?: string; items: NavItem[] };
