import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const requestWithAuth = req.clone({
    setHeaders: {
      Authorization: 'Bearer mock-supplier-dashboard-token'
    }
  });

  return next(requestWithAuth);
};
