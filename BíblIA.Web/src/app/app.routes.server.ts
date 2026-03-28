import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // /auth não tem guard — pode ser pré-renderizada no build para carregamento rápido
  { path: 'auth', renderMode: RenderMode.Prerender },

  // Todas as rotas protegidas usam Client-side rendering:
  // durante o pré-render no servidor não existe localStorage, então o authGuard
  // sempre redirecionaria para /auth, gerando HTML errado para as rotas logadas.
  // Com RenderMode.Client, o Angular hidrata no browser onde o token já existe.
  { path: '**',   renderMode: RenderMode.Client }
];
