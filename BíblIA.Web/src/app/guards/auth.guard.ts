import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Guard funcional (Angular 17+): retorna true para prosseguir ou UrlTree para redirecionar
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  }

  // Salva a URL que o usuário tentou acessar para redirecionar após o login
  return router.createUrlTree(['/auth']);
};
