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
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';
import { OfflineSyncService } from '../../services/offline-sync.service';

// ── Tipos do cronograma ───────────────────────────────────────────────────────

interface ChapterEntry { bookId: number; bookName: string; chapter: number; }
interface DayReading   { day: number; chapters: ChapterEntry[]; }

// ── Tipos do quiz ────────────────────────────────────────────────────────────

interface QuizQuestion { question: string; options: string[]; correct: number; }
interface QuizState {
  loading:        boolean;
  dayNumber:      number;
  chapterList:    string;   // "Gênesis 1, Gênesis 2" — exibido no header do quiz
  questions:      QuizQuestion[];
  currentQ:       number;
  selectedOption: number | null;
  score:          number;
  finished:       boolean;
}

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
    MatTooltipModule,
    MatSnackBarModule
  ],
  templateUrl: './reading.component.html',
  styleUrls: ['./reading.component.css']
})
export class ReadingComponent implements OnInit {
  private api    = inject(ApiService);
  private auth   = inject(AuthService);
  readonly offline = inject(OfflineSyncService);

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

  // Quiz pós-leitura
  quiz = signal<QuizState | null>(null);

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

  // ── Histórico de atividade ─────────────────────────────────────────────────
  //
  // Agrupa os logs por data (desc) e mapeia planId → nome do plano.
  // Exibe até 30 dias com atividade para não sobrecarregar o DOM.

  activityHistory = computed(() => {
    const logs  = this.allLogs();
    const plans = this.plans();
    const planMap = new Map(plans.map(p => [p.id, p]));

    // Agrupa por data
    const byDate = new Map<string, ReadingLog[]>();
    for (const log of logs) {
      const date = log.completedAt.substring(0, 10);
      if (!byDate.has(date)) byDate.set(date, []);
      byDate.get(date)!.push(log);
    }

    // Ordena datas desc e pega as 30 mais recentes
    return [...byDate.entries()]
      .sort(([a], [b]) => b.localeCompare(a))
      .slice(0, 30)
      .map(([date, entries]) => ({
        date,
        label: new Date(date + 'T12:00:00').toLocaleDateString('pt-BR', {
          weekday: 'short', day: 'numeric', month: 'short'
        }),
        entries: entries.map(e => ({
          planIcon: planMap.get(e.planId)?.icon ?? '📖',
          planName: planMap.get(e.planId)?.name ?? `Plano #${e.planId}`,
          day:      e.dayNumber
        }))
      }));
  });

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

  // ── Quiz pós-leitura ──────────────────────────────────────────────────────
  //
  // Após marcar um dia como lido, pede à IA 3 perguntas de múltipla escolha
  // sobre os capítulos do dia. A resposta é JSON puro — parseamos e exibimos
  // o quiz num overlay sem sair do plano de leitura.

  triggerQuiz(day: number): void {
    const chapters = this.schedule()[day - 1]?.chapters ?? [];
    if (chapters.length === 0) return;

    const chapterList = chapters.map(c => `${c.bookName} ${c.chapter}`).join(', ');

    this.quiz.set({
      loading: true, dayNumber: day, chapterList,
      questions: [], currentQ: 0, selectedOption: null, score: 0, finished: false
    });

    const prompt =
      `Crie exatamente 3 perguntas de múltipla escolha sobre os seguintes capítulos bíblicos: ${chapterList}. ` +
      `Responda APENAS com um array JSON válido, sem texto adicional, neste formato exato: ` +
      `[{"question":"...","options":["A","B","C","D"],"correct":0}] ` +
      `onde "correct" é o índice (0-3) da resposta correta. As perguntas devem testar compreensão do texto bíblico.`;

    this.api.askAi(prompt, 'bible').subscribe({
      next: res => {
        try {
          // Extrai o array JSON da resposta (pode ter texto ao redor)
          const match = res.answer.match(/\[[\s\S]*\]/);
          const questions: QuizQuestion[] = match ? JSON.parse(match[0]) : [];
          this.quiz.update(q => q ? { ...q, loading: false, questions } : q);
        } catch {
          this.quiz.set(null); // resposta não parseável — descarta silenciosamente
        }
      },
      error: () => this.quiz.set(null)
    });
  }

  selectAnswer(idx: number): void {
    this.quiz.update(q => q && q.selectedOption === null ? { ...q, selectedOption: idx } : q);
  }

  nextQuestion(): void {
    this.quiz.update(q => {
      if (!q) return q;
      const correct = q.questions[q.currentQ]?.correct ?? -1;
      const gained  = q.selectedOption === correct ? 1 : 0;
      const next    = q.currentQ + 1;
      if (next >= q.questions.length) {
        return { ...q, score: q.score + gained, selectedOption: null, finished: true };
      }
      return { ...q, score: q.score + gained, currentQ: next, selectedOption: null };
    });
  }

  closeQuiz(): void {
    this.quiz.set(null);
  }

  // ── Marcar / desmarcar ────────────────────────────────────────────────────

  toggleDay(day: number): void {
    const plan = this.activePlan();
    if (!plan) return;
    const key  = `${plan.id}:${day}`;
    const set  = new Set(this.doneSet());

    if (set.has(key)) {
      if (this.offline.isOnline()) {
        this.api.unmarkReadingDay(plan.id, day).subscribe();
      } else {
        this.offline.enqueue({ type: 'unmark_day', planId: plan.id, dayNumber: day });
      }
      set.delete(key);
    } else {
      if (this.offline.isOnline()) {
        this.api.markReadingDay(plan.id, day).subscribe({
          next: () => {
            this.api.getReadingLogs().subscribe(logs => {
              this.allLogs.set(logs);
              this.streak.set(this.computeStreak(logs));
            });
            this.triggerQuiz(day);
          }
        });
      } else {
        this.offline.enqueue({ type: 'mark_day', planId: plan.id, dayNumber: day });
      }
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
