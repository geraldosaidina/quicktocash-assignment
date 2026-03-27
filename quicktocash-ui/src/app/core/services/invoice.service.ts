import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { API_BASE_URL } from '../constants/api.constants';
import { ApiResponse } from '../../shared/models/api-response.model';
import { Invoice } from '../../shared/models/invoice.model';
import {
  CreateEarlyPaymentRequestPayload,
  EarlyPaymentEligibility,
  EarlyPaymentRequest
} from '../../shared/models/early-payment.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly invoicesBaseUrl = `${API_BASE_URL}/invoices`;

  constructor(private readonly http: HttpClient) {}

  getInvoicesBySupplier(supplierId: string): Observable<Invoice[]> {
    const params = new HttpParams().set('supplierId', supplierId);

    return this.http
      .get<ApiResponse<Invoice[]>>(this.invoicesBaseUrl, { params })
      .pipe(map((response) => response.data));
  }

  getInvoiceById(invoiceId: string): Observable<Invoice> {
    return this.http
      .get<ApiResponse<Invoice>>(`${this.invoicesBaseUrl}/${invoiceId}`)
      .pipe(map((response) => response.data));
  }

  getEarlyPaymentEligibility(invoiceId: string): Observable<EarlyPaymentEligibility> {
    return this.http
      .get<ApiResponse<EarlyPaymentEligibility>>(
        `${this.invoicesBaseUrl}/${invoiceId}/early-payment-eligibility`
      )
      .pipe(map((response) => response.data));
  }

  submitEarlyPaymentRequest(
    invoiceId: string,
    payload: CreateEarlyPaymentRequestPayload
  ): Observable<EarlyPaymentRequest> {
    return this.http
      .post<ApiResponse<EarlyPaymentRequest>>(
        `${this.invoicesBaseUrl}/${invoiceId}/early-payment-request`,
        payload
      )
      .pipe(map((response) => response.data));
  }
}
