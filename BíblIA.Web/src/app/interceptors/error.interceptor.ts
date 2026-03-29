import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

// Interceptor de erros: loga detalhes no console e re-lança para os subscribers.
// 401 → limpa sessão e redireciona para /auth (token expirado ou inválido).
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const auth   = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.error('[HTTP Error]', {
        status:  error.status,
        url:     error.url,
        message: error.message,
        body:    error.error
      });

      if (error.status === 401) {
        auth.logout();
        router.navigate(['/auth']);
      }

      return throwError(() => error);
    })
  );
};
