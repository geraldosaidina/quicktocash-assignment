import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        return throwError(
          () =>
            new Error(
              'Cannot reach API. Ensure QuickToCash.Api is running at http://localhost:5237.'
            )
        );
      }

      const message =
        error.error?.message ??
        (error.error?.errors?.length ? error.error.errors.join(', ') : null) ??
        'Unexpected error while calling API.';

      return throwError(() => new Error(message));
    })
  );
};
