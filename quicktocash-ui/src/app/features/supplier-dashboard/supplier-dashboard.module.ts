import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { SupplierDashboardRoutingModule } from './supplier-dashboard-routing.module';
import { SupplierDashboardPageComponent } from './pages/supplier-dashboard-page.component';

@NgModule({
  declarations: [SupplierDashboardPageComponent],
  imports: [SharedModule, SupplierDashboardRoutingModule]
})
export class SupplierDashboardModule {}
