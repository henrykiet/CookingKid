import { Routes } from '@angular/router';
import { DynamicGridComponent } from './dynamics/dynamic-grid/dynamic-grid.component';
import { DashboardComponent } from './app-homes/dashboard/dashboard.component';
import { DynamicPopupComponent } from './dynamics/dynamic-popup/dynamic-popup.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'dynamic', component: DynamicGridComponent },
  { path: 'dynamicPopup', component: DynamicPopupComponent },
];
