import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { InvoiceService } from '../../../core/services/invoice.service';
import { EarlyPaymentEligibility, Invoice } from '../../../shared/models';

@Component({
  selector: 'app-supplier-dashboard-page',
  standalone: false,
  templateUrl: './supplier-dashboard-page.component.html',
  styleUrl: './supplier-dashboard-page.component.scss'
})
export class SupplierDashboardPageComponent implements OnInit {
  readonly displayedColumns = ['invoiceNumber', 'supplierName', 'status', 'amount', 'dueDate', 'actions'];

  readonly filtersForm;
  readonly requestForm;

  invoices: Invoice[] = [];
  selectedInvoice: Invoice | null = null;
  eligibility: EarlyPaymentEligibility | null = null;
  loading = false;
  submitting = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly invoiceService: InvoiceService,
    private readonly snackBar: MatSnackBar
  ) {
    this.filtersForm = this.fb.group({
      supplierId: ['SUP-100', Validators.required]
    });

    this.requestForm = this.fb.group({
      disbursementAmount: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices(): void {
    if (this.filtersForm.invalid) {
      return;
    }

    const supplierId = this.filtersForm.controls.supplierId.value?.trim() ?? '';
    if (!supplierId) {
      return;
    }

    this.loading = true;
    this.invoiceService
      .getInvoicesBySupplier(supplierId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (invoices) => {
          this.invoices = invoices;
          this.selectedInvoice = null;
          this.eligibility = null;
        },
        error: (error) => this.showError(error)
      });
  }

  viewInvoice(invoiceId: string): void {
    this.loading = true;
    this.invoiceService
      .getInvoiceById(invoiceId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (invoice) => {
          this.selectedInvoice = invoice;
          this.eligibility = null;
        },
        error: (error) => this.showError(error)
      });
  }

  checkEligibility(invoiceId: string): void {
    this.loading = true;
    this.invoiceService
      .getEarlyPaymentEligibility(invoiceId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (eligibility) => {
          this.eligibility = eligibility;
          this.requestForm.patchValue({
            disbursementAmount: eligibility.disbursementAmount
          });
        },
        error: (error) => this.showError(error)
      });
  }

  submitEarlyPaymentRequest(): void {
    if (!this.selectedInvoice || this.requestForm.invalid) {
      return;
    }

    this.submitting = true;
    this.invoiceService
      .submitEarlyPaymentRequest(this.selectedInvoice.invoiceId, {
        disbursementAmount: this.requestForm.controls.disbursementAmount.value ?? 0
      })
      .pipe(finalize(() => (this.submitting = false)))
      .subscribe({
        next: () => {
          this.snackBar.open('Early payment request submitted.', 'Close', { duration: 3000 });
        },
        error: (error) => this.showError(error)
      });
  }

  private showError(error: unknown): void {
    const message = error instanceof Error ? error.message : 'Request failed.';
    this.snackBar.open(message, 'Close', { duration: 4000 });
  }
}
