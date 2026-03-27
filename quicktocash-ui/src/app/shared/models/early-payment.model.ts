export interface EarlyPaymentEligibility {
  invoiceId: string;
  isEligible: boolean;
  fee: number;
  disbursementAmount: number;
  earlyByDays: number;
  reason: string;
}

export interface CreateEarlyPaymentRequestPayload {
  disbursementAmount: number;
}

export interface EarlyPaymentRequest {
  requestId: string;
  invoiceId: string;
  requestedDate: string;
  disbursementAmount: number;
  fee: number;
  status: string;
}
