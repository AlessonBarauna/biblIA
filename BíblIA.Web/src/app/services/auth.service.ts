import { Injectable, signal, computed, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { ApiService } from './api.service';

export interface AuthUser {
  id: number;
  name: string;
  email: string;
}

const TOKEN_KEY = 'jwt_token';
const USER_KEY  = 'current_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api      = inject(ApiService);
  private router   = inject(Router);
  private platform = inject(PLATFORM_ID);

  // ── Estado reativo via signals ───────────────────────────────────────────
  // Signal privado (escrita interna) → exposto como readonly para o template
  private _user  = signal<AuthUser | null>(null);
  private _token = signal<string | null>(null);

  readonly currentUser  = this._user.asReadonly();
  readonly isLoggedIn   = computed(() => this._token() !== null);

  // Mantido para compatibilidade com código existente que usa currentUser$
  get currentUser$() {
    // Retorna um Observable que emite o valor atual — não é reativo a mudanças
    // futuras, mas evita quebrar código legado durante a migração
    return new Observable<AuthUser | null>(obs => {
      obs.next(this._user());
      obs.complete();
    });
  }

  constructor() {
    this.restoreSession();
  }

  // ── Restaura sessão do localStorage ao inicializar ───────────────────────
  private restoreSession(): void {
    if (!isPlatformBrowser(this.platform)) return;

    const token = localStorage.getItem(TOKEN_KEY);
    const userJson = localStorage.getItem(USER_KEY);

    if (token && userJson) {
      this._token.set(token);
      this._user.set(JSON.parse(userJson));
    }
  }

  // ── Ações públicas ───────────────────────────────────────────────────────

  login(email: string, password: string): Observable<any> {
    console.log('[AuthService] Iniciando login:', { email });
    return this.api.login({ email, password }).pipe(
      tap(response => {
        console.log('[AuthService] Response recebida:', response);
        this.handleAuthResponse(response);
      })
    );
  }

  register(email: string, name: string, password: string): Observable<any> {
    console.log('[AuthService] Iniciando registro:', { email, name });
    return this.api.register({ email, name, password, passwordConfirm: password }).pipe(
      tap(response => {
        console.log('[AuthService] Response recebida:', response);
        this.handleAuthResponse(response);
      })
    );
  }

  logout(): void {
    this._user.set(null);
    this._token.set(null);

    if (isPlatformBrowser(this.platform)) {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(USER_KEY);
    }

    this.router.navigate(['/auth']);
  }

  getToken(): string | null {
    return this._token();
  }

  // Compatibilidade: código antigo que chamava getCurrentUser()
  getCurrentUser(): AuthUser | null {
    return this._user();
  }

  // ── Privado ──────────────────────────────────────────────────────────────

  private handleAuthResponse(response: { success: boolean; token?: string; user?: AuthUser }): void {
    if (!response.success || !response.token || !response.user) return;

    this._token.set(response.token);
    this._user.set(response.user);

    if (isPlatformBrowser(this.platform)) {
      localStorage.setItem(TOKEN_KEY, response.token);
      localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    }
  }
}
