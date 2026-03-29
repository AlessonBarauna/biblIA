import { Injectable, PLATFORM_ID, inject, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

// ── Tipos de ação enfileirável ───────────────────────────────────────────────
//
// Cada ação representa uma chamada de API que falhou por falta de conexão.
// O discriminador `type` permite replay tipado no flush().

export type SyncAction =
  | { type: 'mark_day';    planId: number; dayNumber: number }
  | { type: 'unmark_day';  planId: number; dayNumber: number }
  | { type: 'upsert_note'; bookId: number; chapter: number; verse: number; note: string }
  | { type: 'delete_note'; bookId: number; chapter: number; verse: number };

interface QueueEntry {
  id:       string;   // UUID simples para deduplicação
  action:   SyncAction;
  queuedAt: string;   // ISO date
}

@Injectable({ providedIn: 'root' })
export class OfflineSyncService {
  private api      = inject(ApiService);
  private platform = inject(PLATFORM_ID);

  private readonly QUEUE_KEY = 'offline_sync_queue';

  isOnline  = signal(true);
  flushing  = signal(false);

  private queue = signal<QueueEntry[]>(this.loadQueue());

  pendingCount = computed(() => this.queue().length);

  constructor() {
    if (!isPlatformBrowser(this.platform)) return;

    // Sincroniza com o estado real da rede
    this.isOnline.set(navigator.onLine);

    window.addEventListener('online',  () => {
      this.isOnline.set(true);
      this.flush();
    });
    window.addEventListener('offline', () => this.isOnline.set(false));
  }

  // ── API pública ───────────────────────────────────────────────────────────

  enqueue(action: SyncAction): void {
    // Deduplicação: substitui ação do mesmo tipo + mesma chave primária
    const key  = this.actionKey(action);
    const prev = this.queue().filter(e => this.actionKey(e.action) !== key);

    const entry: QueueEntry = {
      id:       crypto.randomUUID ? crypto.randomUUID() : Math.random().toString(36).slice(2),
      action,
      queuedAt: new Date().toISOString()
    };

    const next = [...prev, entry];
    this.queue.set(next);
    this.persistQueue(next);
  }

  // ── Replay da fila ────────────────────────────────────────────────────────
  //
  // Processa as ações em ordem FIFO. Em caso de falha de uma ação, para o
  // processamento para não perder ordenação (ex: mark depois de unmark).

  flush(): void {
    const pending = this.queue();
    if (pending.length === 0 || this.flushing()) return;

    this.flushing.set(true);
    this.replayNext([...pending]);
  }

  private replayNext(remaining: QueueEntry[]): void {
    if (remaining.length === 0) {
      this.flushing.set(false);
      return;
    }

    const [head, ...tail] = remaining;
    const obs = this.observableFor(head.action);

    obs.subscribe({
      next: () => {
        // Remove da fila só após confirmação do servidor
        const next = this.queue().filter(e => e.id !== head.id);
        this.queue.set(next);
        this.persistQueue(next);
        this.replayNext(tail);
      },
      error: () => {
        // Rede voltou mas servidor retornou erro — para e tenta novamente mais tarde
        this.flushing.set(false);
      }
    });
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  // Chave única por ação — usada para deduplicação no enqueue
  private actionKey(action: SyncAction): string {
    switch (action.type) {
      case 'mark_day':
      case 'unmark_day':    return `${action.type}:${action.planId}:${action.dayNumber}`;
      case 'upsert_note':
      case 'delete_note':   return `note:${action.bookId}:${action.chapter}:${action.verse}`;
    }
  }

  private observableFor(action: SyncAction): Observable<unknown> {
    switch (action.type) {
      case 'mark_day':    return this.api.markReadingDay(action.planId, action.dayNumber);
      case 'unmark_day':  return this.api.unmarkReadingDay(action.planId, action.dayNumber);
      case 'upsert_note': return this.api.upsertVerseNote(action.bookId, action.chapter, action.verse, action.note);
      case 'delete_note': return this.api.deleteVerseNote(action.bookId, action.chapter, action.verse);
    }
  }

  private loadQueue(): QueueEntry[] {
    if (!isPlatformBrowser(this.platform)) return [];
    try {
      return JSON.parse(localStorage.getItem(this.QUEUE_KEY) ?? '[]');
    } catch {
      return [];
    }
  }

  private persistQueue(entries: QueueEntry[]): void {
    if (!isPlatformBrowser(this.platform)) return;
    localStorage.setItem(this.QUEUE_KEY, JSON.stringify(entries));
  }
}
