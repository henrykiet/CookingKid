export interface menuData {
  label: string;
  routeLink?: string;
  icon: string;
  children?: menuData[];
  expanded?: boolean;
}

export interface SidebarToggle {
  screenWidth: number;
  collapsed: boolean;
}
