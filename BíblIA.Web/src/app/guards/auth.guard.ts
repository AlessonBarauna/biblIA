import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Guard funcional (Angular 17+): retorna true para prosseguir ou UrlTree para redirecionar.
// Durante SSR (servidor não tem localStorage), deixa passar sempre — a verificação
// real acontece no browser após a hidratação, onde o token pode ser lido.
export const authGuard: CanActivateFn = () => {
  const platform    = inject(PLATFORM_ID);
  const authService = inject(AuthService);
  const router      = inject(Router);

  if (!isPlatformBrowser(platform)) return true;

  if (authService.isLoggedIn()) return true;

  return router.createUrlTree(['/auth']);
};
