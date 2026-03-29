import { Injectable, PLATFORM_ID, inject, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private platform = inject(PLATFORM_ID);

  private readonly KEY_ENABLED  = 'notify_enabled';
  private readonly KEY_TIME     = 'notify_time';
  private readonly KEY_LAST     = 'notify_last_date';

  // Suporte à API no browser atual
  readonly isSupported = isPlatformBrowser(this.platform) && 'Notification' in window;

  // Permissão atual ('default' | 'granted' | 'denied')
  permission = signal<NotificationPermission>(
    this.isSupported ? Notification.permission : 'denied'
  );

  // Configurações salvas pelo usuário
  reminderEnabled = signal<boolean>(this.loadBool(this.KEY_ENABLED));
  reminderTime    = signal<string>(this.loadStr(this.KEY_TIME, '08:00'));

  // ── Permissão ─────────────────────────────────────────────────────────────

  async requestPermission(): Promise<void> {
    if (!this.isSupported) return;
    const result = await Notification.requestPermission();
    this.permission.set(result);
  }

  // ── Configuração ──────────────────────────────────────────────────────────

  saveSettings(enabled: boolean, time: string): void {
    this.reminderEnabled.set(enabled);
    this.reminderTime.set(time);
    if (!isPlatformBrowser(this.platform)) return;
    localStorage.setItem(this.KEY_ENABLED, String(enabled));
    localStorage.setItem(this.KEY_TIME, time);
  }

  // ── Verificação e disparo ─────────────────────────────────────────────────
  //
  // Chamado ao inicializar o app. Lógica:
  //   1. Notificações habilitadas + permissão concedida?
  //   2. Horário atual >= horário configurado?
  //   3. Ainda não notificou hoje?
  //   → Dispara notificação e marca a data de hoje.

  checkAndNotify(): void {
    if (!this.isSupported) return;
    if (!this.reminderEnabled()) return;
    if (this.permission() !== 'granted') return;

    const now      = new Date();
    const todayStr = now.toISOString().substring(0, 10);        // YYYY-MM-DD
    const hhmm     = now.toTimeString().substring(0, 5);        // HH:MM

    if (hhmm < this.reminderTime()) return;                      // ainda não chegou a hora
    if (localStorage.getItem(this.KEY_LAST) === todayStr) return; // já notificou hoje

    localStorage.setItem(this.KEY_LAST, todayStr);
    this.show(
      '📖 Hora da leitura bíblica!',
      'Você ainda não leu hoje. Abra o app e continue seu plano de leitura.',
      '/reading'
    );
  }

  // Notificação de teste — usada pelo botão na tela de perfil
  testNotification(): void {
    if (!this.isSupported || this.permission() !== 'granted') return;
    this.show('📖 Notificação de teste', 'Seus lembretes de leitura estão ativos!', '/');
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  private show(title: string, body: string, path: string): void {
    // Prefere usar o Service Worker registration para que a notificação
    // apareça mesmo com a aba em segundo plano (quando SW estiver ativo).
    if ('serviceWorker' in navigator && navigator.serviceWorker.controller) {
      navigator.serviceWorker.ready.then(reg => {
        reg.showNotification(title, {
          body,
          icon:  '/icons/icon-192x192.png',
          badge: '/icons/icon-72x72.png',
          data:  { url: path }
        });
      }).catch(() => new Notification(title, { body }));
    } else {
      new Notification(title, { body });
    }
  }

  private loadBool(key: string): boolean {
    if (!isPlatformBrowser(this.platform)) return false;
    return localStorage.getItem(key) === 'true';
  }

  private loadStr(key: string, fallback: string): string {
    if (!isPlatformBrowser(this.platform)) return fallback;
    return localStorage.getItem(key) ?? fallback;
  }
}
