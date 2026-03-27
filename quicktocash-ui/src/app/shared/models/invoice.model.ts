export interface Invoice {
  invoiceId: string;
  invoiceNumber: string;
  supplierName: string;
  supplierId: string;
  amount: number;
  submittedDate: string;
  dueDate: string;
  status: string;
}
