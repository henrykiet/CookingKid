import { Routes } from '@angular/router';
import { DashboardComponent } from './app-homes/dashboard/dashboard.component';
import { DynamicPopupComponent } from './dynamics/dynamic-popup/dynamic-popup.component';
import { DynamicGridComponent } from './dynamics/dynamic-grid/dynamic-grid.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'grid/:controllerName',
    component: DynamicGridComponent,
    children: [{ path: 'popup', component: DynamicPopupComponent }],
  },
  {
    path: 'popup/:controllerName',
    component: DynamicPopupComponent,
    // Bạn có thể thêm tham số action hoặc ID nếu muốn
  },
  { path: 'dashboard', component: DashboardComponent },
];
