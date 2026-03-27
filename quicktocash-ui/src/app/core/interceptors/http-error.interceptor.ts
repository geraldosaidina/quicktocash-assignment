import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message =
        error.error?.message ??
        (error.error?.errors?.length ? error.error.errors.join(', ') : null) ??
        'Unexpected error while calling API.';

      return throwError(() => new Error(message));
    })
  );
};
