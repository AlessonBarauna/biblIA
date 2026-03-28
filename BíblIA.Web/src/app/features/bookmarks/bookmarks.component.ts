import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ApiService, Bookmark } from '../../services/api.service';

@Component({
  selector: 'app-bookmarks',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './bookmarks.component.html',
  styleUrls: ['./bookmarks.component.css']
})
export class BookmarksComponent implements OnInit {
  private api = inject(ApiService);

  bookmarks = signal<Bookmark[]>([]);
  loading   = signal(true);
  // Set de IDs sendo removidos — evita duplo-clique e mostra feedback imediato
  removing  = signal<Set<number>>(new Set());

  ngOnInit(): void {
    this.api.getBookmarks().subscribe({
      next:  b  => { this.bookmarks.set(b); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  remove(bookmark: Bookmark): void {
    // Otimistic UI: mostra estado "removendo" antes da resposta do servidor
    this.removing.set(new Set([...this.removing(), bookmark.id]));

    this.api.removeBookmark(bookmark.id).subscribe({
      next: () => {
        this.bookmarks.set(this.bookmarks().filter(b => b.id !== bookmark.id));
        const s = new Set(this.removing());
        s.delete(bookmark.id);
        this.removing.set(s);
      },
      error: () => {
        // Reverte o estado visual em caso de erro
        const s = new Set(this.removing());
        s.delete(bookmark.id);
        this.removing.set(s);
      }
    });
  }

  isRemoving(id: number): boolean {
    return this.removing().has(id);
  }

  // Query params para deep link no BibleComponent
  bibleParams(b: Bookmark): Record<string, number> {
    return { bookId: b.bookId, chapter: b.chapter };
  }

  formatDate(iso: string): string {
    return new Date(iso).toLocaleDateString('pt-BR', { day: 'numeric', month: 'short', year: 'numeric' });
  }
}
