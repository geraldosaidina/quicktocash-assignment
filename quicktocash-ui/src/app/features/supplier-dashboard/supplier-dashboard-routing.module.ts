import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupplierDashboardPageComponent } from './pages/supplier-dashboard-page.component';

const routes: Routes = [
  {
    path: '',
    component: SupplierDashboardPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupplierDashboardRoutingModule {}
