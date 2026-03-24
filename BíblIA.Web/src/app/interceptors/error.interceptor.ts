import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

// Interceptor de erros: loga detalhes no console e re-lança para os subscribers
// Posição: depois do authInterceptor para capturar erros de autenticação também
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.error('[HTTP Error]', {
        status: error.status,
        url: error.url,
        message: error.message,
        body: error.error
      });
      return throwError(() => error);
    })
  );
};
