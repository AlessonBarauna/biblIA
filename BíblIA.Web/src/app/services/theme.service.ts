import { Injectable, signal, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type Theme = 'dark' | 'light';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private platform = inject(PLATFORM_ID);
  private readonly STORAGE_KEY = 'biblia_theme';

  // Estado reativo — lido pelo componente para alternar ícone no toolbar
  readonly theme = signal<Theme>(this.loadSaved());

  constructor() {
    // Aplica o tema salvo assim que o serviço é criado (browser only)
    if (isPlatformBrowser(this.platform)) {
      this.applyTheme(this.theme());
    }
  }

  toggle(): void {
    const next: Theme = this.theme() === 'dark' ? 'light' : 'dark';
    this.theme.set(next);
    if (isPlatformBrowser(this.platform)) {
      localStorage.setItem(this.STORAGE_KEY, next);
      this.applyTheme(next);
    }
  }

  private applyTheme(theme: Theme): void {
    // A classe 'theme-light' no <body> ativa as variáveis CSS do tema claro.
    // Sem a classe, as variáveis padrão do dark theme são usadas.
    document.body.classList.toggle('theme-light', theme === 'light');
  }

  private loadSaved(): Theme {
    if (!isPlatformBrowser(this.platform)) return 'dark';
    return (localStorage.getItem(this.STORAGE_KEY) as Theme | null) ?? 'dark';
  }
}
