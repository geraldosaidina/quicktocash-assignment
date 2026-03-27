import { ErrorHandler, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorHandler implements ErrorHandler {
  handleError(error: unknown): void {
    // Keep global handling simple and interview-friendly.
    console.error('Global error handler:', error);
  }
}
