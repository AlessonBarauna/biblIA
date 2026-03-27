import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterModule } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiService, BibleVerse } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { ChatStateService } from '../../services/chat-state.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule, MatProgressSpinnerModule, RouterModule],
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

  bookmarkCount       = signal(0);
  completedModuleCount = signal(0);

  readonly isLoggedIn  = this.auth.isLoggedIn;
  readonly activeChatId = this.chatState.activeChatId;

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

  // forkJoin dispara as duas requests em paralelo — resultado só chega quando ambas concluem.
  // catchError em cada um garante que uma falha não derruba o outro.
  private loadStats(): void {
    this.loadingStats.set(true);
    forkJoin({
      bookmarks: this.api.getBookmarks().pipe(catchError(() => of([]))),
      progress:  this.api.getProgress().pipe(catchError(() => of([])))
    }).subscribe(({ bookmarks, progress }) => {
      this.bookmarkCount.set(bookmarks.length);
      this.completedModuleCount.set(progress.length);
      this.loadingStats.set(false);
    });
  }

  get todayLabel(): string {
    return new Date().toLocaleDateString('pt-BR', { weekday: 'long', day: 'numeric', month: 'long' });
  }
}
