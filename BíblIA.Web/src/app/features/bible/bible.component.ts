import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ApiService, BibleBook, BibleVerse } from '../../services/api.service';
import { AiPanelComponent } from '../../shared/ai-panel/ai-panel.component';

// O componente funciona como uma máquina de estados com 3 "vistas":
//   'books'   → lista de livros do AT/NT
//   'chapters' → grid de capítulos do livro selecionado
//   'verses'   → versículos do capítulo selecionado
type View = 'books' | 'chapters' | 'verses';
type TranslationKey = 'kjv' | 'aa' | 'acf' | 'nvi';
interface Translation { key: TranslationKey; label: string; name: string; }

@Component({
  selector: 'app-bible',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule,
    AiPanelComponent
  ],
  templateUrl: './bible.component.html',
  styleUrls: ['./bible.component.css']
})
export class BibleComponent implements OnInit {
  private api = inject(ApiService);

  // ── Estado ──────────────────────────────────────────────────────────────
  view = signal<View>('books');
  loading = signal(false);

  books = signal<BibleBook[]>([]);
  selectedBook = signal<BibleBook | null>(null);
  selectedChapter = signal<number | null>(null);
  verses = signal<BibleVerse[]>([]);

  // Traduções disponíveis — adicionadas na ordem de preferência de exibição
  readonly translations: Translation[] = [
    { key: 'kjv', label: 'KJV', name: 'King James Version'       },
    { key: 'aa',  label: 'AA',  name: 'Almeida Revisada'          },
    { key: 'acf', label: 'ACF', name: 'Almeida Corrigida e Fiel'  },
    { key: 'nvi', label: 'NVI', name: 'Nova Versão Internacional' },
  ];

  activeTranslation = signal<TranslationKey>('kjv');

  // ── Computed ─────────────────────────────────────────────────────────────
  otBooks = computed(() => this.books().filter(b => b.testament === 'OT'));
  ntBooks = computed(() => this.books().filter(b => b.testament === 'NT'));

  chapterNumbers = computed(() => {
    const book = this.selectedBook();
    if (!book) return [];
    return Array.from({ length: book.chapterCount }, (_, i) => i + 1);
  });

  versionLabel = computed(() =>
    this.translations.find(t => t.key === this.activeTranslation())?.label ?? 'KJV'
  );

  ngOnInit(): void {
    this.loadBooks();
  }

  // ── Ações ────────────────────────────────────────────────────────────────

  loadBooks(): void {
    this.loading.set(true);
    this.api.getBooks().subscribe({
      next: books => {
        this.books.set(books);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  selectBook(book: BibleBook): void {
    this.selectedBook.set(book);
    this.selectedChapter.set(null);
    this.verses.set([]);
    this.view.set('chapters');
  }

  selectChapter(chapter: number): void {
    const book = this.selectedBook();
    if (!book) return;

    this.selectedChapter.set(chapter);
    this.loading.set(true);
    this.view.set('verses');

    this.api.getChapter(book.id, chapter).subscribe({
      next: verses => {
        this.verses.set(verses);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  goBack(): void {
    if (this.view() === 'verses') {
      this.view.set('chapters');
    } else if (this.view() === 'chapters') {
      this.view.set('books');
    }
  }

  // Avança para a próxima tradução ciclicamente
  toggleVersion(): void {
    const keys = this.translations.map(t => t.key);
    const current = keys.indexOf(this.activeTranslation());
    const next = (current + 1) % keys.length;
    this.activeTranslation.set(keys[next]);
  }

  verseText(v: BibleVerse): string {
    // Retorna o texto na tradução ativa; se vazio, cai no KJV como fallback
    const map: Record<string, string> = {
      kjv: v.textKJV,
      aa:  v.textAA,
      acf: v.textACF,
      nvi: v.textNVI,
    };
    return map[this.activeTranslation()] || v.textKJV;
  }
}
