import { Routes } from '@angular/router';
import { DashboardComponent } from './app-homes/dashboard/dashboard.component';
import { DynamicPopupComponent } from './dynamics/dynamic-popup/dynamic-popup.component';
import { UserGridComponent } from './pages/users/user-list.page';
import { ItemGridComponent } from './pages/item-list.page';
import { ItemGroupGridComponent } from './pages/item-group-list.page';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'users', component: UserGridComponent },
  { path: 'users/popup', component: DynamicPopupComponent },
  { path: 'items', component: ItemGridComponent },
  { path: 'items/popup', component: DynamicPopupComponent },
  { path: 'item-groups', component: ItemGroupGridComponent },
  { path: 'item-groups/popup', component: DynamicPopupComponent },
];
