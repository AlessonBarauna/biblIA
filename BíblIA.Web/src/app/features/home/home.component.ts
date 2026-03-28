import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiService, BibleVerse, ReadingPlan, ReadingLog } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { ChatStateService } from '../../services/chat-state.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    RouterModule
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private api       = inject(ApiService);
  private auth      = inject(AuthService);
  private chatState = inject(ChatStateService);

  verseOfDay    = signal<BibleVerse | null>(null);
  loadingVerse  = signal(true);
  loadingStats  = signal(false);

  bookmarkCount        = signal(0);
  completedModuleCount = signal(0);
  streak               = signal(0);
  plans                = signal<ReadingPlan[]>([]);

  readonly isLoggedIn   = this.auth.isLoggedIn;
  readonly activeChatId = this.chatState.activeChatId;

  // Planos com progresso > 0, ordenados por % mais avançado
  activePlans = computed(() =>
    this.plans()
      .filter(p => p.completedDays > 0)
      .sort((a, b) => (b.completedDays / b.totalDays) - (a.completedDays / a.totalDays))
      .slice(0, 2) // máximo 2 no dashboard
  );

  progressPct(plan: ReadingPlan): number {
    return Math.round((plan.completedDays / plan.totalDays) * 100);
  }

  features = [
    { title: 'Chat com IA',   description: 'Converse com uma IA especializada em teologia bíblica', route: '/chat',        icon: '💬' },
    { title: 'Bíblia',        description: 'Acesse versículos bíblicos de diferentes versões',      route: '/bible',       icon: '📖' },
    { title: 'Teologia',      description: 'Explore conceitos teológicos profundos',                route: '/theology',    icon: '⛪' },
    { title: 'Escatologia',   description: 'Entenda os últimos tempos e profecias',                 route: '/eschatology', icon: '🔮' },
    { title: 'História',      description: 'Contexto histórico dos livros bíblicos',               route: '/history',     icon: '📚' }
  ];

  ngOnInit(): void {
    this.api.getVerseOfDay().subscribe({
      next:  v  => { this.verseOfDay.set(v); this.loadingVerse.set(false); },
      error: () => this.loadingVerse.set(false)
    });

    if (this.auth.isLoggedIn()) {
      this.loadStats();
    }
  }

  private loadStats(): void {
    this.loadingStats.set(true);
    forkJoin({
      bookmarks: this.api.getBookmarks().pipe(catchError(() => of([]))),
      progress:  this.api.getProgress().pipe(catchError(() => of([]))),
      plans:     this.api.getReadingPlans().pipe(catchError(() => of([] as ReadingPlan[]))),
      logs:      this.api.getReadingLogs().pipe(catchError(() => of([] as ReadingLog[])))
    }).subscribe(({ bookmarks, progress, plans, logs }) => {
      this.bookmarkCount.set(bookmarks.length);
      this.completedModuleCount.set(progress.length);
      this.plans.set(plans);
      this.streak.set(this.computeStreak(logs));
      this.loadingStats.set(false);
    });
  }

  // Replica a lógica do reading component — conta dias consecutivos terminando hoje ou ontem
  private computeStreak(logs: ReadingLog[]): number {
    if (logs.length === 0) return 0;
    const uniqueDates = [...new Set(logs.map(l => l.completedAt.substring(0, 10)))];
    uniqueDates.sort().reverse();
    const toDay = (offset: number) => {
      const d = new Date(); d.setDate(d.getDate() + offset);
      return d.toISOString().substring(0, 10);
    };
    if (uniqueDates[0] !== toDay(0) && uniqueDates[0] !== toDay(-1)) return 0;
    let streak = 1;
    for (let i = 1; i < uniqueDates.length; i++) {
      const diff = Math.round((new Date(uniqueDates[i-1]).getTime() - new Date(uniqueDates[i]).getTime()) / 86_400_000);
      if (diff === 1) streak++; else break;
    }
    return streak;
  }

  get todayLabel(): string {
    return new Date().toLocaleDateString('pt-BR', { weekday: 'long', day: 'numeric', month: 'long' });
  }
}
