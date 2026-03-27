import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'dashboard'
  },
  {
    path: 'dashboard',
    loadChildren: () =>
      import('./features/supplier-dashboard/supplier-dashboard.module').then(
        (m) => m.SupplierDashboardModule
      )
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
