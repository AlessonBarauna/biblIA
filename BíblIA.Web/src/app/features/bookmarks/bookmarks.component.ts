import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
    FormsModule,
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
  removing  = signal<Set<number>>(new Set());

  // ── Filtro por tag ────────────────────────────────────────────────────────
  activeTag = signal<string | null>(null);

  allTags = computed(() => {
    const set = new Set<string>();
    this.bookmarks().forEach(b => b.tags?.forEach(t => set.add(t)));
    return [...set].sort();
  });

  filtered = computed(() => {
    const tag = this.activeTag();
    if (!tag) return this.bookmarks();
    return this.bookmarks().filter(b => b.tags?.includes(tag));
  });

  // ── Editor de tags inline ─────────────────────────────────────────────────
  editingTagsId  = signal<number | null>(null);
  editingTagsText = signal(''); // campo de texto livre: "fé, graça, salvação"

  ngOnInit(): void {
    this.api.getBookmarks().subscribe({
      next:  b  => { this.bookmarks.set(b); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  remove(bookmark: Bookmark): void {
    this.removing.set(new Set([...this.removing(), bookmark.id]));
    this.api.removeBookmark(bookmark.id).subscribe({
      next: () => {
        this.bookmarks.set(this.bookmarks().filter(b => b.id !== bookmark.id));
        const s = new Set(this.removing());
        s.delete(bookmark.id);
        this.removing.set(s);
        // Limpa filtro se a tag ativa já não existe mais
        if (this.activeTag() && !this.allTags().includes(this.activeTag()!)) {
          this.activeTag.set(null);
        }
      },
      error: () => {
        const s = new Set(this.removing());
        s.delete(bookmark.id);
        this.removing.set(s);
      }
    });
  }

  isRemoving(id: number): boolean { return this.removing().has(id); }

  // ── Tags ──────────────────────────────────────────────────────────────────

  openTagEditor(b: Bookmark): void {
    this.editingTagsText.set((b.tags ?? []).join(', '));
    this.editingTagsId.set(b.id);
  }

  cancelTagEditor(): void {
    this.editingTagsId.set(null);
    this.editingTagsText.set('');
  }

  saveTags(b: Bookmark): void {
    const tags = this.editingTagsText()
      .split(',')
      .map(t => t.trim().toLowerCase())
      .filter(t => t.length > 0);

    this.api.updateBookmarkTags(b.id, tags).subscribe({
      next: updated => {
        this.bookmarks.update(list =>
          list.map(bk => bk.id === updated.id ? { ...bk, tags: updated.tags } : bk)
        );
        this.cancelTagEditor();
        // Limpa filtro se a tag ativa foi removida deste favorito
        if (this.activeTag() && !this.allTags().includes(this.activeTag()!)) {
          this.activeTag.set(null);
        }
      }
    });
  }

  removeTag(b: Bookmark, tag: string): void {
    const tags = (b.tags ?? []).filter(t => t !== tag);
    this.api.updateBookmarkTags(b.id, tags).subscribe({
      next: updated =>
        this.bookmarks.update(list =>
          list.map(bk => bk.id === updated.id ? { ...bk, tags: updated.tags } : bk)
        )
    });
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  bibleParams(b: Bookmark): Record<string, number> {
    return { bookId: b.bookId, chapter: b.chapter };
  }

  formatDate(iso: string): string {
    return new Date(iso).toLocaleDateString('pt-BR', { day: 'numeric', month: 'short', year: 'numeric' });
  }
}
