export const sidebarData = [
  {
    routeLink: 'dashboard',
    icon: 'fas fa-home',
    label: 'DashBoard',
  },
  {
    icon: 'fas fa-home',
    label: 'Danh mục',
    children: [
      { icon: 'fas fa-home', label: 'Danh mục', routeLink: 'dynamic' },
      {
        icon: 'fas fa-home',
        label: 'Danh mục popup',
        routeLink: 'dynamicPopup',
      },
    ],
  },
  {
    icon: 'fas fa-home',
    label: 'Danh mục Bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb',
    children: [
      { icon: 'fas fa-home', label: 'Danh mục B1', routeLink: 'dynamic' },
      {
        icon: 'fas fa-home',
        label:
          'Danh mục popup B1 222222222222222222222222222222222222222222 toi met',
        routeLink: 'dynamicPopup',
      },
    ],
  },
];

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
