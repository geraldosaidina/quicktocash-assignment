import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { InvoiceService } from '../../../core/services/invoice.service';
import { EarlyPaymentEligibility, Invoice } from '../../../shared/models';

type InvoiceStatusFilter = 'All' | 'Pending' | 'Approved' | 'Funded' | 'Rejected';

@Component({
  selector: 'app-supplier-dashboard-page',
  standalone: false,
  templateUrl: './supplier-dashboard-page.component.html',
  styleUrl: './supplier-dashboard-page.component.scss'
})
export class SupplierDashboardPageComponent implements OnInit {
  readonly displayedColumns = [
    'invoiceNumber',
    'supplierName',
    'amount',
    'submittedDate',
    'dueDate',
    'status',
    'actions'
  ];
  readonly statusFilters: InvoiceStatusFilter[] = ['All', 'Pending', 'Approved', 'Funded', 'Rejected'];
  readonly statusClassMap: Record<string, string> = {
    Pending: 'badge-pending',
    Approved: 'badge-approved',
    Funded: 'badge-funded',
    Rejected: 'badge-rejected'
  };
  readonly defaultSupplierId = 'SUP-100';

  readonly filtersForm;
  readonly requestForm;

  allInvoices: Invoice[] = [];
  filteredInvoices: Invoice[] = [];
  summaryCounts: Record<InvoiceStatusFilter, number> = {
    All: 0,
    Pending: 0,
    Approved: 0,
    Funded: 0,
    Rejected: 0
  };
  selectedStatus: InvoiceStatusFilter = 'All';
  searchTerm = '';

  selectedInvoice: Invoice | null = null;
  eligibility: EarlyPaymentEligibility | null = null;
  detailPanelOpen = false;
  eligibilityLoading = false;
  detailErrorMessage = '';
  selectedInvoiceAlreadyRequested = false;
  confirmingRequest = false;

  private readonly requestedInvoiceIds = new Set<string>();

  loading = false;
  submitting = false;
  errorMessage = '';

  constructor(
    private readonly fb: FormBuilder,
    private readonly invoiceService: InvoiceService,
    private readonly snackBar: MatSnackBar
  ) {
    this.filtersForm = this.fb.group({
      supplierId: [this.defaultSupplierId, Validators.required]
    });

    this.requestForm = this.fb.group({
      disbursementAmount: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    // Delay initial request to next macrotask to avoid first-pass hydration expression checks.
    setTimeout(() => this.loadInvoices());
  }

  loadInvoices(): void {
    if (this.filtersForm.invalid) {
      return;
    }

    const supplierId = this.filtersForm.controls.supplierId.value?.trim() ?? '';
    if (!supplierId) {
      return;
    }

    this.errorMessage = '';
    this.loading = true;
    this.invoiceService
      .getInvoicesBySupplier(supplierId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (invoices) => {
          this.allInvoices = invoices;
          this.selectedStatus = 'All';
          this.searchTerm = '';
          this.recomputeDerivedState();
          this.closeDetailPanel();
        },
        error: (error) => {
          this.allInvoices = [];
          this.filteredInvoices = [];
          this.recomputeDerivedState();
          this.closeDetailPanel();
          this.showError(error);
        }
      });
  }

  selectStatus(status: string): void {
    this.selectedStatus = this.statusFilters.includes(status as InvoiceStatusFilter)
      ? (status as InvoiceStatusFilter)
      : 'All';
    this.recomputeDerivedState();
  }

  updateSearchTerm(value: string): void {
    this.searchTerm = value;
    this.recomputeDerivedState();
  }

  openInvoiceDetails(invoiceId: string): void {
    if (!invoiceId) {
      return;
    }

    this.errorMessage = '';
    this.detailErrorMessage = '';
    this.loading = true;
    this.invoiceService
      .getInvoiceById(invoiceId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (invoice) => {
          this.selectedInvoice = invoice;
          this.detailPanelOpen = true;
          this.selectedInvoiceAlreadyRequested = this.requestedInvoiceIds.has(invoice.invoiceId);
          this.confirmingRequest = false;
          this.fetchEligibility(invoice.invoiceId);
        },
        error: (error) => this.showError(error)
      });
  }

  fetchEligibility(invoiceId: string): void {
    this.detailErrorMessage = '';
    this.eligibility = null;
    this.eligibilityLoading = true;
    this.confirmingRequest = false;

    this.invoiceService
      .getEarlyPaymentEligibility(invoiceId)
      .pipe(finalize(() => (this.eligibilityLoading = false)))
      .subscribe({
        next: (eligibility) => {
          this.eligibility = eligibility;
          this.requestForm.patchValue({
            disbursementAmount: eligibility.disbursementAmount
          });
        },
        error: (error) => {
          this.detailErrorMessage = this.getErrorMessage(error);
          this.showError(error);
        }
      });
  }

  startRequestConfirmation(): void {
    if (!this.selectedInvoice || !this.eligibility?.isEligible || this.selectedInvoiceAlreadyRequested) {
      return;
    }

    this.confirmingRequest = true;
  }

  cancelRequestConfirmation(): void {
    this.confirmingRequest = false;
  }

  submitEarlyPaymentRequest(): void {
    if (
      !this.selectedInvoice ||
      !this.eligibility?.isEligible ||
      this.selectedInvoiceAlreadyRequested ||
      this.requestForm.invalid
    ) {
      return;
    }

    this.errorMessage = '';
    this.detailErrorMessage = '';
    this.submitting = true;
    const invoiceId = this.selectedInvoice.invoiceId;

    this.invoiceService
      .submitEarlyPaymentRequest(invoiceId, {
        disbursementAmount: this.requestForm.controls.disbursementAmount.value ?? 0
      })
      .pipe(finalize(() => (this.submitting = false)))
      .subscribe({
        next: () => {
          this.snackBar.open('Early payment request submitted.', 'Close', { duration: 3000 });
          this.requestedInvoiceIds.add(invoiceId);
          this.selectedInvoiceAlreadyRequested = true;
          this.confirmingRequest = false;
          this.fetchEligibility(invoiceId);
        },
        error: (error) => {
          const message = this.getErrorMessage(error);
          this.detailErrorMessage = message;

          if (
            message.toLowerCase().includes('duplicate') ||
            message.toLowerCase().includes('pending early payment request')
          ) {
            this.requestedInvoiceIds.add(invoiceId);
            this.selectedInvoiceAlreadyRequested = true;
          }

          this.showError(error);
        }
      });
  }

  closeDetailPanel(): void {
    this.detailPanelOpen = false;
    this.selectedInvoice = null;
    this.eligibility = null;
    this.detailErrorMessage = '';
    this.selectedInvoiceAlreadyRequested = false;
    this.confirmingRequest = false;
  }

  private recomputeDerivedState(): void {
    this.summaryCounts = this.buildSummaryCounts(this.allInvoices);
    this.filteredInvoices = this.filterInvoices(this.allInvoices, this.selectedStatus, this.searchTerm);
  }

  private buildSummaryCounts(invoices: Invoice[]): Record<InvoiceStatusFilter, number> {
    const counts: Record<InvoiceStatusFilter, number> = {
      All: invoices.length,
      Pending: 0,
      Approved: 0,
      Funded: 0,
      Rejected: 0
    };

    for (const invoice of invoices) {
      if (invoice.status === 'Pending') {
        counts.Pending++;
      } else if (invoice.status === 'Approved') {
        counts.Approved++;
      } else if (invoice.status === 'Funded') {
        counts.Funded++;
      } else if (invoice.status === 'Rejected') {
        counts.Rejected++;
      }
    }

    return counts;
  }

  private filterInvoices(
    invoices: Invoice[],
    status: InvoiceStatusFilter,
    searchTerm: string
  ): Invoice[] {
    const normalizedSearchTerm = searchTerm.trim().toLowerCase();

    return invoices.filter((invoice) => {
      const statusMatches = status === 'All' || invoice.status === status;
      const searchMatches =
        !normalizedSearchTerm ||
        invoice.invoiceNumber.toLowerCase().includes(normalizedSearchTerm) ||
        invoice.supplierName.toLowerCase().includes(normalizedSearchTerm);

      return statusMatches && searchMatches;
    });
  }

  private showError(error: unknown): void {
    const message = this.getErrorMessage(error);
    this.errorMessage = message;
    this.snackBar.open(message, 'Close', { duration: 4000 });
  }

  private getErrorMessage(error: unknown): string {
    return error instanceof Error ? error.message : 'Request failed.';
  }
}
