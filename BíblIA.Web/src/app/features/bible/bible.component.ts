import { Component, signal, computed, inject, OnInit, DestroyRef, HostListener, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, EMPTY, of } from 'rxjs';
import { debounceTime, switchMap, catchError } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';
import { FormsModule } from '@angular/forms';
import { ApiService, BibleBook, BibleVerse, BibleStudyNote, Bookmark } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
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
    FormsModule,
    MatTabsModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule,
    MatExpansionModule,
    AiPanelComponent
  ],
  templateUrl: './bible.component.html',
  styleUrls: ['./bible.component.css']
})
export class BibleComponent implements OnInit {
  private api        = inject(ApiService);
  private auth       = inject(AuthService);
  private route      = inject(ActivatedRoute);
  private destroyRef = inject(DestroyRef);
  private platform   = inject(PLATFORM_ID);

  private readonly HISTORY_KEY = 'bible_search_history';
  private readonly HISTORY_MAX = 10;

  private searchSubject = new Subject<string>();

  // ── Estado ──────────────────────────────────────────────────────────────
  view = signal<View>('books');
  loading = signal(false);

  books = signal<BibleBook[]>([]);
  selectedBook = signal<BibleBook | null>(null);
  selectedChapter = signal<number | null>(null);
  verses = signal<BibleVerse[]>([]);
  studyNote = signal<BibleStudyNote | null>(null);
  noteExpanded = signal(false);

  // ── Busca ────────────────────────────────────────────────────────────────
  searchQuery     = signal('');
  searchResults   = signal<BibleVerse[]>([]);
  searching       = signal(false);
  // Filtros avançados: testamento ('' = todos) e livro específico (null = todos)
  searchTestament = signal<string>('');
  searchBookId    = signal<number | null>(null);
  // Histórico das últimas 10 buscas — salvo no localStorage
  searchHistory   = signal<string[]>(this.loadHistory());

  // bookmarkMap: Map<verseNumber, bookmarkId> — permite checar e remover em O(1)
  bookmarkMap = signal<Map<number, number>>(new Map());

  // Verso copiado — exibe "Copiado!" por 2s antes de limpar
  copiedVerse    = signal<number | null>(null);
  // Verso compartilhado — exibe "Compartilhado!" por 2s
  sharedVerse    = signal<number | null>(null);
  // Verso a destacar após navegação pela busca — limpo após 2s + animação CSS
  highlightedVerse = signal<number | null>(null);

  readonly isLoggedIn = this.auth.isLoggedIn;

  // Computed para navegação entre capítulos
  hasPrevChapter = computed(() => (this.selectedChapter() ?? 1) > 1);
  hasNextChapter = computed(() => {
    const book = this.selectedBook();
    return book ? (this.selectedChapter() ?? 0) < book.chapterCount : false;
  });

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
    if (this.auth.isLoggedIn()) {
      this.loadBookmarks();
    }

    // Debounce: só dispara request depois de 300ms sem digitar.
    // switchMap cancela a request anterior se o usuário continuar digitando — evita race condition.
    this.searchSubject.pipe(
      debounceTime(300),
      switchMap(query => {
        if (query.length < 3) {
          this.searchResults.set([]);
          this.searching.set(false);
          return EMPTY;
        }
        this.searching.set(true);
        const testament = this.searchTestament() || undefined;
        const bookId    = this.searchBookId()    ?? undefined;
        return this.api.searchBibleVerses(query, 20, testament, bookId).pipe(
          catchError(() => of([] as BibleVerse[]))
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(results => {
      this.searchResults.set(results);
      this.searching.set(false);
      // Salva no histórico quando a busca retornou algum resultado
      if (results.length > 0) {
        this.saveToHistory(this.searchQuery().trim());
      }
    });
  }

  // ── Ações ────────────────────────────────────────────────────────────────

  onSearchInput(query: string): void {
    this.searchQuery.set(query);
    this.searchSubject.next(query.trim());
  }

  clearSearch(): void {
    this.searchQuery.set('');
    this.searchResults.set([]);
    this.searching.set(false);
  }

  // Muda o filtro de testamento e re-dispara a busca se já há query ativa
  setTestamentFilter(testament: string): void {
    this.searchTestament.set(testament);
    // Ao mudar testamento, limpa o filtro de livro (pode ser de outro testamento)
    this.searchBookId.set(null);
    if (this.searchQuery().length >= 3) {
      this.searchSubject.next(this.searchQuery().trim());
    }
  }

  setBookFilter(bookId: number | null): void {
    this.searchBookId.set(bookId);
    if (this.searchQuery().length >= 3) {
      this.searchSubject.next(this.searchQuery().trim());
    }
  }

  // Computed: livros disponíveis para o filtro de livro (depende do filtro de testamento)
  get filteredBooksForSearch(): BibleBook[] {
    const testament = this.searchTestament();
    if (!testament) return this.books();
    return this.books().filter(b => b.testament === testament);
  }

  // ── Histórico de buscas ───────────────────────────────────────────────────
  //
  // Salva no localStorage as últimas HISTORY_MAX buscas distintas.
  // SSR-safe: lê/escreve apenas em ambiente browser.

  private loadHistory(): string[] {
    if (!isPlatformBrowser(this.platform)) return [];
    try {
      return JSON.parse(localStorage.getItem(this.HISTORY_KEY) ?? '[]');
    } catch {
      return [];
    }
  }

  private saveToHistory(query: string): void {
    if (!isPlatformBrowser(this.platform) || query.length < 3) return;
    const prev    = this.searchHistory().filter(q => q !== query); // remove duplicata
    const updated = [query, ...prev].slice(0, this.HISTORY_MAX);
    this.searchHistory.set(updated);
    localStorage.setItem(this.HISTORY_KEY, JSON.stringify(updated));
  }

  removeHistoryEntry(query: string): void {
    const updated = this.searchHistory().filter(q => q !== query);
    this.searchHistory.set(updated);
    localStorage.setItem(this.HISTORY_KEY, JSON.stringify(updated));
  }

  clearHistory(): void {
    this.searchHistory.set([]);
    if (isPlatformBrowser(this.platform)) {
      localStorage.removeItem(this.HISTORY_KEY);
    }
  }

  // Preenche o campo de busca com um item do histórico e dispara a busca
  applyHistory(query: string): void {
    this.searchQuery.set(query);
    this.searchSubject.next(query);
  }

  // Navega diretamente para o capítulo do versículo encontrado na busca.
  // Passa o número do versículo para que selectChapter o destaque após o carregamento.
  goToVerse(v: BibleVerse): void {
    this.clearSearch();
    const navigate = (book: BibleBook) => {
      this.selectedBook.set(book);
      this.selectChapter(v.chapter, v.verse);
    };
    const book = this.books().find(b => b.id === v.bookId);
    if (book) navigate(book);
    else this.api.getBook(v.bookId).subscribe(b => navigate(b));
  }

  // ── Cópia de versículo ────────────────────────────────────────────────────

  // Copia o versículo no formato padrão de citação bíblica e dá feedback visual por 2s.
  copyVerse(v: BibleVerse): void {
    const ref  = `${this.selectedBook()!.name} ${this.selectedChapter()}:${v.verse}`;
    const text = `"${this.verseText(v)}" — ${ref}`;
    navigator.clipboard.writeText(text).then(() => {
      this.copiedVerse.set(v.verse);
      setTimeout(() => this.copiedVerse.set(null), 2000);
    });
  }

  // ── Compartilhar versículo ────────────────────────────────────────────────
  //
  // Usa a Web Share API (navigator.share) quando disponível — abre a sheet nativa
  // do SO com opções como WhatsApp, e-mail, copiar etc.
  // Fallback: copia o texto + URL para a área de transferência.

  shareVerse(v: BibleVerse): void {
    const book  = this.selectedBook()!;
    const ref   = `${book.name} ${this.selectedChapter()}:${v.verse}`;
    const text  = `"${this.verseText(v)}" — ${ref}`;
    const url   = `${window.location.origin}/bible?bookId=${book.id}&chapter=${this.selectedChapter()}`;

    const markShared = () => {
      this.sharedVerse.set(v.verse);
      setTimeout(() => this.sharedVerse.set(null), 2000);
    };

    if (navigator.share) {
      // navigator.share retorna Promise — catch silencia o cancelamento pelo usuário
      navigator.share({ title: ref, text, url }).then(markShared).catch(() => {});
    } else {
      // Fallback: copia texto + link para área de transferência
      navigator.clipboard.writeText(`${text}\n${url}`).then(markShared);
    }
  }

  // ── Navegação entre capítulos ─────────────────────────────────────────────

  prevChapter(): void {
    const ch = this.selectedChapter();
    if (ch && ch > 1) this.selectChapter(ch - 1);
  }

  nextChapter(): void {
    const ch   = this.selectedChapter();
    const book = this.selectedBook();
    if (ch && book && ch < book.chapterCount) this.selectChapter(ch + 1);
  }

  // Captura setas do teclado apenas quando na view de versículos e fora de inputs.
  @HostListener('window:keydown', ['$event'])
  onKeydown(e: KeyboardEvent): void {
    if (this.view() !== 'verses') return;
    const tag = (e.target as HTMLElement)?.tagName?.toLowerCase();
    if (tag === 'input' || tag === 'textarea') return;
    if (e.key === 'ArrowLeft')  { e.preventDefault(); this.prevChapter(); }
    if (e.key === 'ArrowRight') { e.preventDefault(); this.nextChapter(); }
  }

  // Extrai um trecho do texto em volta do termo buscado para exibir como preview.
  searchPreview(text: string): string {
    const query = this.searchQuery().trim().toLowerCase();
    if (!query || !text) return text.slice(0, 100);
    const idx = text.toLowerCase().indexOf(query);
    if (idx === -1) return text.slice(0, 100);
    const start = Math.max(0, idx - 40);
    const end   = Math.min(text.length, idx + query.length + 60);
    return (start > 0 ? '…' : '') + text.slice(start, end) + (end < text.length ? '…' : '');
  }

  loadBookmarks(): void {
    this.api.getBookmarks().subscribe({
      next: bookmarks => {
        const map = new Map<number, number>();
        bookmarks.forEach(b => map.set(b.verse, b.id));
        this.bookmarkMap.set(map);
      },
      error: () => {} // silencioso — usuário não-logado recebe 401, esperado
    });
  }

  isBookmarked(v: BibleVerse): boolean {
    return this.bookmarkMap().has(v.verse);
  }

  toggleBookmark(v: BibleVerse): void {
    const existingId = this.bookmarkMap().get(v.verse);

    if (existingId !== undefined) {
      // Remove: atualiza o Map localmente antes da resposta para feedback imediato
      this.api.removeBookmark(existingId).subscribe({
        next: () => {
          const map = new Map(this.bookmarkMap());
          map.delete(v.verse);
          this.bookmarkMap.set(map);
        }
      });
    } else {
      const book = this.selectedBook()!;
      this.api.addBookmark({
        bookId: book.id,
        chapter: this.selectedChapter()!,
        verse: v.verse,
        verseText: this.verseText(v),
      }).subscribe({
        next: bookmark => {
          const map = new Map(this.bookmarkMap());
          map.set(bookmark.verse, bookmark.id);
          this.bookmarkMap.set(map);
        }
      });
    }
  }

  loadBooks(): void {
    // Lê query params uma vez — usados para deep link vindo da página de favoritos
    const params      = this.route.snapshot.queryParamMap;
    const deepBookId  = params.get('bookId')  ? Number(params.get('bookId'))  : null;
    const deepChapter = params.get('chapter') ? Number(params.get('chapter')) : null;

    this.loading.set(true);
    this.api.getBooks().subscribe({
      next: books => {
        this.books.set(books);
        this.loading.set(false);

        // Auto-navega quando o componente foi aberto via link de favorito
        if (deepBookId && deepChapter) {
          const book = books.find(b => b.id === deepBookId);
          if (book) {
            this.selectedBook.set(book);
            this.selectChapter(deepChapter);
          }
        }
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

  selectChapter(chapter: number, highlightVerse?: number): void {
    const book = this.selectedBook();
    if (!book) return;

    this.selectedChapter.set(chapter);
    this.loading.set(true);
    this.view.set('verses');
    this.studyNote.set(null);
    this.noteExpanded.set(false);
    this.highlightedVerse.set(null);

    this.api.getChapter(book.id, chapter).subscribe({
      next: verses => {
        this.verses.set(verses);
        this.loading.set(false);
        // Destaca o versículo alvo por 2.5s após carregamento
        if (highlightVerse) {
          this.highlightedVerse.set(highlightVerse);
          setTimeout(() => this.highlightedVerse.set(null), 2500);
        }
      },
      error: () => this.loading.set(false)
    });

    // Fire-and-forget: 404 é silenciado pois nem todo capítulo tem nota
    this.api.getChapterNote(book.id, chapter).subscribe({
      next: note => this.studyNote.set(note),
      error: () => {} // 404 esperado para capítulos sem nota
    });
  }

  goBack(): void {
    if (this.view() === 'verses') {
      this.view.set('chapters');
      this.studyNote.set(null);
      this.noteExpanded.set(false);
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
