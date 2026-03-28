import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ApiService, BibleBook, ReadingPlan, ReadingLog } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

// ── Tipos do cronograma ───────────────────────────────────────────────────────

interface ChapterEntry { bookId: number; bookName: string; chapter: number; }
interface DayReading   { day: number; chapters: ChapterEntry[]; }

// ── Tipos do heatmap ─────────────────────────────────────────────────────────
// Cada célula representa um dia calendario com contagem de leituras naquele dia.
// count 0 = sem leitura; isFuture = cinza; isToday = borda destacada

interface HeatDay {
  date: string;    // YYYY-MM-DD
  count: number;   // quantas entradas de log nesse dia
  isToday: boolean;
  isFuture: boolean;
  monthLabel: string | null; // nome curto do mês — só no 1º dia visível de cada mês
}

// ── Componente ────────────────────────────────────────────────────────────────

@Component({
  selector: 'app-reading',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './reading.component.html',
  styleUrls: ['./reading.component.css']
})
export class ReadingComponent implements OnInit {
  private api  = inject(ApiService);
  private auth = inject(AuthService);

  readonly isLoggedIn = this.auth.isLoggedIn;

  // Estado de carregamento
  loading = signal(true);

  // Dados do servidor
  plans    = signal<ReadingPlan[]>([]);
  books    = signal<BibleBook[]>([]);
  allLogs  = signal<ReadingLog[]>([]);
  // Set de "planId:dayNumber" para lookup O(1)
  doneSet  = signal<Set<string>>(new Set());
  // Dias consecutivos de leitura (qualquer plano) — base para o streak
  streak   = signal(0);

  // View de estatísticas
  showStats = signal(false);

  // Plano aberto para detalhe
  activePlan = signal<ReadingPlan | null>(null);

  // Cronograma gerado do plano ativo (calculado quando activePlan muda)
  schedule = signal<DayReading[]>([]);

  // Dia atual do plano ativo (próximo dia a ser lido)
  currentDay = computed(() => {
    const plan = this.activePlan();
    if (!plan) return 1;
    // Encontra o maior dia concluído neste plano e avança 1
    const done = this.doneSet();
    let max = 0;
    for (let d = 1; d <= plan.totalDays; d++) {
      if (done.has(`${plan.id}:${d}`)) max = d;
      else break; // Para no primeiro buraco — evita pular dias
    }
    return Math.min(max + 1, plan.totalDays);
  });

  // ── Heatmap ───────────────────────────────────────────────────────────────
  //
  // Retorna um array plano de 91 dias (13 semanas × 7) alinhado ao domingo da
  // semana mais antiga. O template usa `grid-auto-flow: column` com 7 linhas,
  // então os dias saem naturalmente dispostos em colunas-semana.

  heatmapDays = computed((): HeatDay[] => {
    const logs = this.allLogs();

    // Conta leituras por data calendar (UTC, para evitar drift de timezone)
    const dayCounts = new Map<string, number>();
    for (const log of logs) {
      const d = log.completedAt.substring(0, 10);
      dayCounts.set(d, (dayCounts.get(d) ?? 0) + 1);
    }

    const todayDate = new Date();
    todayDate.setHours(0, 0, 0, 0);
    const todayStr = todayDate.toISOString().substring(0, 10);

    // Inicio = domingo da semana 12 semanas atrás (para completar 13 semanas inteiras)
    const start = new Date(todayDate);
    start.setDate(start.getDate() - 90);            // ~13 semanas atrás
    start.setDate(start.getDate() - start.getDay()); // recua ao domingo

    const MONTHS = ['Jan','Fev','Mar','Abr','Mai','Jun','Jul','Ago','Set','Out','Nov','Dez'];
    const seenMonths = new Set<string>();

    const days: HeatDay[] = [];
    const cursor = new Date(start);

    while (days.length < 91) {
      const dateStr = cursor.toISOString().substring(0, 10);
      const monthKey = dateStr.substring(0, 7); // YYYY-MM

      // Label de mês: só na primeira aparição de cada mês (para o template renderizar)
      let monthLabel: string | null = null;
      if (!seenMonths.has(monthKey)) {
        seenMonths.add(monthKey);
        monthLabel = MONTHS[cursor.getMonth()];
      }

      days.push({
        date:       dateStr,
        count:      dayCounts.get(dateStr) ?? 0,
        isToday:    dateStr === todayStr,
        isFuture:   cursor > todayDate,
        monthLabel
      });

      cursor.setDate(cursor.getDate() + 1);
    }

    return days;
  });

  // Métricas derivadas dos logs
  totalUniqueDays = computed(() =>
    new Set(this.allLogs().map(l => l.completedAt.substring(0, 10))).size
  );

  totalReadings = computed(() => this.allLogs().length);

  ngOnInit(): void {
    const plans$    = this.api.getReadingPlans().pipe(catchError(() => of([] as ReadingPlan[])));
    const books$    = this.api.getBooks().pipe(catchError(() => of([] as BibleBook[])));
    const logs$     = this.isLoggedIn()
      ? this.api.getReadingLogs().pipe(catchError(() => of([] as ReadingLog[])))
      : of([] as ReadingLog[]);

    forkJoin({ plans: plans$, books: books$, logs: logs$ }).subscribe(({ plans, books, logs }) => {
      this.plans.set(plans);
      this.books.set(books);
      this.allLogs.set(logs);
      this.doneSet.set(new Set(logs.map(l => `${l.planId}:${l.dayNumber}`)));
      this.streak.set(this.computeStreak(logs));
      this.loading.set(false);
    });
  }

  openPlan(plan: ReadingPlan): void {
    this.activePlan.set(plan);
    this.schedule.set(this.buildSchedule(plan));
    this.showStats.set(false);
  }

  closePlan(): void {
    this.activePlan.set(null);
    this.schedule.set([]);
  }

  openStats(): void {
    this.activePlan.set(null);
    this.schedule.set([]);
    this.showStats.set(true);
  }

  closeStats(): void {
    this.showStats.set(false);
  }

  // Intensidade da célula do heatmap: 0 (sem leitura) a 3 (muitas leituras)
  heatIntensity(count: number): number {
    if (count === 0) return 0;
    if (count <= 1)  return 1;
    if (count <= 3)  return 2;
    return 3;
  }

  // ── Marcar / desmarcar ────────────────────────────────────────────────────

  toggleDay(day: number): void {
    const plan = this.activePlan();
    if (!plan) return;
    const key  = `${plan.id}:${day}`;
    const set  = new Set(this.doneSet());

    if (set.has(key)) {
      this.api.unmarkReadingDay(plan.id, day).subscribe();
      set.delete(key);
    } else {
      this.api.markReadingDay(plan.id, day).subscribe({
        next: () => {
          // Após confirmação do servidor, busca logs frescos para recalcular streak e heatmap
          this.api.getReadingLogs().subscribe(logs => {
            this.allLogs.set(logs);
            this.streak.set(this.computeStreak(logs));
          });
        }
      });
      set.add(key);
    }
    this.doneSet.set(set);

    // Atualiza completedDays no plano local
    this.plans.update(list =>
      list.map(p => p.id === plan.id
        ? { ...p, completedDays: [...set].filter(k => k.startsWith(`${p.id}:`)).length }
        : p)
    );
    this.activePlan.update(p => p
      ? { ...p, completedDays: [...set].filter(k => k.startsWith(`${p!.id}:`)).length }
      : p);
  }

  isDone(day: number): boolean {
    const plan = this.activePlan();
    return !!plan && this.doneSet().has(`${plan.id}:${day}`);
  }

  progressPct(plan: ReadingPlan): number {
    return Math.round((plan.completedDays / plan.totalDays) * 100);
  }

  // ── Deep link para a Bíblia ────────────────────────────────────────────────

  bibleParams(chapter: ChapterEntry): Record<string, number> {
    return { bookId: chapter.bookId, chapter: chapter.chapter };
  }

  // ── Streak ────────────────────────────────────────────────────────────────
  //
  // Conta quantos dias calendário CONSECUTIVOS terminando em hoje (ou ontem)
  // o usuário leu em qualquer plano. Usa apenas as datas de completedAt.
  //
  // Por quê aceitar ontem? Evita que o streak caia a zero logo de manhã,
  // antes de o usuário ter tido tempo de fazer a leitura do dia.

  private computeStreak(logs: ReadingLog[]): number {
    if (logs.length === 0) return 0;

    // Datas únicas no formato 'YYYY-MM-DD' (UTC, para evitar drift de timezone)
    const uniqueDates = [...new Set(logs.map(l => l.completedAt.substring(0, 10)))];
    uniqueDates.sort().reverse(); // mais recente primeiro

    const toDay = (offset: number) => {
      const d = new Date();
      d.setDate(d.getDate() + offset);
      return d.toISOString().substring(0, 10);
    };

    const today     = toDay(0);
    const yesterday = toDay(-1);

    // Streak só existe se a leitura mais recente foi hoje ou ontem
    if (uniqueDates[0] !== today && uniqueDates[0] !== yesterday) return 0;

    let streak = 1;
    for (let i = 1; i < uniqueDates.length; i++) {
      const prev = new Date(uniqueDates[i - 1]);
      const curr = new Date(uniqueDates[i]);
      const diff = Math.round((prev.getTime() - curr.getTime()) / 86_400_000);
      if (diff === 1) streak++;
      else break;
    }
    return streak;
  }

  // ── Geração do cronograma ─────────────────────────────────────────────────
  //
  // O algoritmo distribui os capítulos da lista de forma linear pelos dias.
  // Math.round nos índices garante que nenhum capítulo seja pulado ou duplicado.

  private buildSchedule(plan: ReadingPlan): DayReading[] {
    const books    = this.filterBooks(plan.strategy);
    const chapters = this.flatChapters(books);
    const total    = chapters.length;
    const days:    DayReading[] = [];

    for (let d = 1; d <= plan.totalDays; d++) {
      const start = Math.round(((d - 1) / plan.totalDays) * total);
      const end   = Math.round((d       / plan.totalDays) * total);
      days.push({ day: d, chapters: chapters.slice(start, end) });
    }
    return days;
  }

  private filterBooks(strategy: string): BibleBook[] {
    const all = [...this.books()].sort((a, b) => a.orderIndex - b.orderIndex);
    if (strategy === 'new_testament') return all.filter(b => b.testament === 'NT');
    if (strategy === 'gospels')       return all.filter(b => b.orderIndex >= 40 && b.orderIndex <= 43);
    return all; // 'full_bible'
  }

  private flatChapters(books: BibleBook[]): ChapterEntry[] {
    const result: ChapterEntry[] = [];
    for (const b of books) {
      for (let c = 1; c <= b.chapterCount; c++) {
        result.push({ bookId: b.id, bookName: b.name, chapter: c });
      }
    }
    return result;
  }
}
