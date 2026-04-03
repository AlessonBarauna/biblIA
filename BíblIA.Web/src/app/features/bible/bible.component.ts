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
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { ApiService, BibleBook, BibleVerse, BibleStudyNote, Bookmark, VerseNote } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { AiPanelComponent } from '../../shared/ai-panel/ai-panel.component';
import { OfflineSyncService } from '../../services/offline-sync.service';

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
    MatInputModule,
    MatFormFieldModule,
    AiPanelComponent
  ],
  templateUrl: './bible.component.html',
  styleUrls: ['./bible.component.css']
})
export class BibleComponent implements OnInit {
  private api        = inject(ApiService);
  private auth       = inject(AuthService);
  readonly offline   = inject(OfflineSyncService);
  private route      = inject(ActivatedRoute);
  private destroyRef = inject(DestroyRef);
  private platform   = inject(PLATFORM_ID);

  private readonly HISTORY_KEY     = 'bible_search_history';
  private readonly HISTORY_MAX     = 10;
  private readonly POSITION_KEY    = 'bible_last_position';
  private readonly HIGHLIGHTS_PREFIX = 'bible_hl_';

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

  // bookAnnotationMap: Map<bookId, Set<chapter>> — capítulos únicos com anotação por livro
  bookAnnotationMap = signal<Map<number, Set<number>>>(new Map());

  // noteMap: Map<verseNumber, VerseNote> — anotações do capítulo atual
  noteMap = signal<Map<number, VerseNote>>(new Map());
  // Verso com o painel de anotação aberto (null = nenhum)
  editingNoteVerse = signal<number | null>(null);
  // Texto do campo de edição
  editingNoteText  = signal('');

  // Modo leitura — overlay fullscreen sem distrações
  readingMode = signal(false);
  // Versículo selecionado no modo leitura (barra de ações)
  selectedReadingVerse = signal<number | null>(null);
  // Sheet de anotação inline no modo leitura (não sai do reader)
  readingNoteVerse = signal<number | null>(null);
  readingNoteText  = signal('');
  // Destaques: Map<verseNumber, hexColor> — persiste em localStorage
  readingHighlights = signal<Map<number, string>>(new Map());

  // Paleta de destaques — hex armazenado, bg semitransparente para overlay no texto
  readonly highlightColors = [
    { hex: '#FFD600', bg: 'rgba(255,214,0,0.38)',  name: 'Amarelo' },
    { hex: '#FF6D00', bg: 'rgba(255,109,0,0.32)',  name: 'Laranja' },
    { hex: '#00C853', bg: 'rgba(0,200,83,0.28)',   name: 'Verde'   },
    { hex: '#00B0FF', bg: 'rgba(0,176,255,0.30)',  name: 'Azul'    },
    { hex: '#AA00FF', bg: 'rgba(170,0,255,0.24)',  name: 'Roxo'    },
    { hex: '#FF4081', bg: 'rgba(255,64,129,0.30)', name: 'Rosa'    },
  ];

  // Sheets de seleção de livro / capítulo / versão
  showBookSheet     = signal(false);
  showChapterSheet  = signal(false);
  showVersionSheet  = signal(false);

  // Áudio — Web Speech API
  isPlaying = signal(false);

  // Modo comparação — exibe traduções selecionadas em colunas lado a lado
  compareMode         = signal(false);
  compareTranslations = signal<TranslationKey[]>(['kjv', 'acf']);
  // Índice do tamanho de fonte no array abaixo (padrão = 1 → 1rem)
  fontSizeIdx = signal(1);
  readonly fontSizes = [0.9, 1.0, 1.15, 1.3, 1.55];

  // Verso copiado — exibe "Copiado!" por 2s antes de limpar
  copiedVerse    = signal<number | null>(null);
  // Verso compartilhado — exibe "Compartilhado!" por 2s
  sharedVerse    = signal<number | null>(null);
  // Verso a destacar após navegação pela busca — limpo após 2s + animação CSS
  highlightedVerse = signal<number | null>(null);

  // ── Versículos relacionados (cross-references via IA) ─────────────────────
  relatedVersesVerse   = signal<number | null>(null);
  relatedVersesLoading = signal(false);
  relatedVersesList    = signal<{ ref: string; text: string }[]>([]);

  // ── Ask AI por versículo ───────────────────────────────────────────────────
  askAiVerse   = signal<number | null>(null);
  askAiQuery   = signal('');
  askAiAnswer  = signal<string | null>(null);
  askAiLoading = signal(false);

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
      this.loadBookAnnotations();
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

  // ── Modo leitura ──────────────────────────────────────────────────────────

  enterReadingMode(): void  { this.readingMode.set(true);  }
  exitReadingMode(): void   { this.readingMode.set(false); this.selectedReadingVerse.set(null); }

  // ── Ask AI por versículo ──────────────────────────────────────────────────
  //
  // Abre um painel inline abaixo do versículo com campo de pergunta.
  // O contexto do versículo é enviado junto com a pergunta para a IA.

  openAskAi(v: BibleVerse): void {
    if (this.askAiVerse() === v.verse) {
      this.closeAskAi();
      return;
    }
    this.askAiVerse.set(v.verse);
    this.askAiQuery.set('');
    this.askAiAnswer.set(null);
  }

  closeAskAi(): void {
    this.askAiVerse.set(null);
    this.askAiQuery.set('');
    this.askAiAnswer.set(null);
  }

  submitAskAi(v: BibleVerse): void {
    const question = this.askAiQuery().trim();
    if (!question || this.askAiLoading()) return;

    const book    = this.selectedBook()!;
    const ref     = `${book.name} ${this.selectedChapter()}:${v.verse}`;
    const text    = this.verseText(v);
    const context = `Versículo: ${ref} — "${text}"`;

    this.askAiLoading.set(true);
    this.askAiAnswer.set(null);

    this.api.askAi(question, 'bible', context).subscribe({
      next: res => {
        this.askAiAnswer.set(res.answer);
        this.askAiLoading.set(false);
      },
      error: () => {
        this.askAiAnswer.set('Erro ao consultar a IA. Tente novamente.');
        this.askAiLoading.set(false);
      }
    });
  }

  // ── Modo comparação ───────────────────────────────────────────────────────

  toggleCompareMode(): void {
    this.compareMode.update(v => !v);
  }

  // Liga/desliga uma tradução no modo comparação.
  // Regra: mínimo 2 traduções sempre selecionadas.
  toggleCompareTranslation(key: TranslationKey): void {
    const current = this.compareTranslations();
    if (current.includes(key)) {
      if (current.length <= 2) return; // não deixa desmarcar abaixo de 2
      this.compareTranslations.set(current.filter(k => k !== key));
    } else {
      this.compareTranslations.set([...current, key]);
    }
  }

  // Texto de um verso para uma tradução específica (usado no modo comparação)
  verseTextFor(v: BibleVerse, key: TranslationKey): string {
    const map: Record<TranslationKey, string> = {
      kjv: v.textKJV, aa: v.textAA, acf: v.textACF, nvi: v.textNVI
    };
    return map[key] || v.textKJV;
  }

  increaseFontSize(): void {
    if (this.fontSizeIdx() < this.fontSizes.length - 1)
      this.fontSizeIdx.update(i => i + 1);
  }

  decreaseFontSize(): void {
    if (this.fontSizeIdx() > 0)
      this.fontSizeIdx.update(i => i - 1);
  }

  get currentFontSize(): number {
    return this.fontSizes[this.fontSizeIdx()];
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

  // ── Versículos relacionados ───────────────────────────────────────────────
  //
  // Pede à IA 4 referências cruzadas temáticas para o versículo clicado.
  // Clicar no mesmo verso novamente fecha o painel (toggle).

  loadRelatedVerses(v: BibleVerse): void {
    // Toggle: fecha se clicar no mesmo verso
    if (this.relatedVersesVerse() === v.verse) {
      this.relatedVersesVerse.set(null);
      this.relatedVersesList.set([]);
      return;
    }

    const book = this.selectedBook()!;
    const ref  = `${book.name} ${this.selectedChapter()}:${v.verse}`;
    const text = this.verseText(v).slice(0, 120);

    this.relatedVersesVerse.set(v.verse);
    this.relatedVersesLoading.set(true);
    this.relatedVersesList.set([]);

    const prompt =
      `Liste 4 versículos bíblicos que são referências cruzadas ou tematicamente relacionados a ${ref}: "${text}". ` +
      `Responda APENAS com JSON válido, sem texto adicional: ` +
      `[{"ref":"Livro Capítulo:Versículo","text":"texto resumido do versículo em português"}]`;

    this.api.askAi(prompt, 'bible').subscribe({
      next: res => {
        try {
          const match = res.answer.match(/\[[\s\S]*\]/);
          this.relatedVersesList.set(match ? JSON.parse(match[0]) : []);
        } catch {
          this.relatedVersesList.set([]);
        }
        this.relatedVersesLoading.set(false);
      },
      error: () => this.relatedVersesLoading.set(false)
    });
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
    if (e.key === 'Escape' && this.readingMode()) { this.exitReadingMode(); return; }
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
      error: () => {}
    });
  }

  loadBookAnnotations(): void {
    this.api.getAllVerseNotes().subscribe({
      next: notes => {
        const map = new Map<number, Set<number>>();
        for (const n of notes) {
          if (!map.has(n.bookId)) map.set(n.bookId, new Set());
          map.get(n.bookId)!.add(n.chapter);
        }
        this.bookAnnotationMap.set(map);
      },
      error: () => {}
    });
  }

  // Retorna quantos capítulos únicos do livro têm anotação
  annotatedChapterCount(bookId: number): number {
    return this.bookAnnotationMap().get(bookId)?.size ?? 0;
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

  // ── Anotações pessoais ────────────────────────────────────────────────────

  hasNote(verseNumber: number): boolean {
    return this.noteMap().has(verseNumber);
  }

  openNoteEditor(v: BibleVerse): void {
    const existing = this.noteMap().get(v.verse);
    this.editingNoteText.set(existing?.note ?? '');
    this.editingNoteVerse.set(v.verse);
  }

  cancelNoteEditor(): void {
    this.editingNoteVerse.set(null);
    this.editingNoteText.set('');
  }

  saveNote(v: BibleVerse): void {
    const book  = this.selectedBook()!;
    const text  = this.editingNoteText().trim();

    const ch = this.selectedChapter()!;

    if (!text) {
      if (this.noteMap().has(v.verse)) {
        // Atualiza UI imediatamente
        const map = new Map(this.noteMap());
        map.delete(v.verse);
        this.noteMap.set(map);
        this.loadBookAnnotations();

        if (this.offline.isOnline()) {
          this.api.deleteVerseNote(book.id, ch, v.verse).subscribe();
        } else {
          this.offline.enqueue({ type: 'delete_note', bookId: book.id, chapter: ch, verse: v.verse });
        }
      }
    } else {
      // Insere localmente com um objeto temporário (sem id real — substituído no flush)
      const tempNote: VerseNote = { id: -1, bookId: book.id, bookName: book.name, chapter: ch, verse: v.verse, note: text, updatedAt: new Date().toISOString() };
      const map = new Map(this.noteMap());
      map.set(v.verse, tempNote);
      this.noteMap.set(map);
      this.loadBookAnnotations();

      if (this.offline.isOnline()) {
        this.api.upsertVerseNote(book.id, ch, v.verse, text).subscribe({
          next: note => {
            const m = new Map(this.noteMap());
            m.set(v.verse, note); // substitui temporário pelo real
            this.noteMap.set(m);
          }
        });
      } else {
        this.offline.enqueue({ type: 'upsert_note', bookId: book.id, chapter: ch, verse: v.verse, note: text });
      }
    }
    this.cancelNoteEditor();
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

        if (deepBookId && deepChapter) {
          // Deep link vindo de favorito ou plano de leitura
          const book = books.find(b => b.id === deepBookId);
          if (book) { this.selectedBook.set(book); this.selectChapter(deepChapter); }
        } else {
          // Sem deep link: tenta restaurar última posição lida
          this.restorePosition(books);
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
    this.stopAudio();
    this.loading.set(true);
    this.view.set('verses');
    this.studyNote.set(null);
    this.noteExpanded.set(false);
    this.highlightedVerse.set(null);
    this.editingNoteVerse.set(null);
    this.noteMap.set(new Map());
    this.relatedVersesVerse.set(null);
    this.relatedVersesList.set([]);
    this.selectedReadingVerse.set(null);
    this.closeAskAi();
    this.savePosition();
    this.loadHighlights();

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

    // Carrega anotações do capítulo (silencioso — usuário não logado recebe 401)
    if (this.auth.isLoggedIn()) {
      this.api.getVerseNotes(book.id, chapter).subscribe({
        next: notes => {
          const map = new Map<number, VerseNote>();
          notes.forEach(n => map.set(n.verse, n));
          this.noteMap.set(map);
        },
        error: () => {}
      });
    }

    // Fire-and-forget: 404 é silenciado pois nem todo capítulo tem nota
    this.api.getChapterNote(book.id, chapter).subscribe({
      next: note => this.studyNote.set(note),
      error: () => {} // 404 esperado para capítulos sem nota
    });
  }

  // ── Posição (última leitura) ──────────────────────────────────────────────

  savePosition(): void {
    if (!isPlatformBrowser(this.platform)) return;
    const book = this.selectedBook();
    const ch   = this.selectedChapter();
    if (!book || !ch) return;
    localStorage.setItem(this.POSITION_KEY, JSON.stringify({ bookId: book.id, chapter: ch }));
  }

  private restorePosition(books: BibleBook[]): void {
    if (!isPlatformBrowser(this.platform)) return;
    try {
      const saved = JSON.parse(localStorage.getItem(this.POSITION_KEY) ?? 'null');
      if (saved?.bookId && saved?.chapter) {
        const book = books.find(b => b.id === saved.bookId);
        if (book) { this.selectedBook.set(book); this.selectChapter(saved.chapter); }
      }
    } catch {}
  }

  // ── Sheets de seleção ─────────────────────────────────────────────────────

  openBookSheet(): void  { this.showBookSheet.set(true); }
  closeBookSheet(): void { this.showBookSheet.set(false); }

  selectBookFromSheet(book: BibleBook): void {
    this.selectedBook.set(book);
    this.showBookSheet.set(false);
    this.showChapterSheet.set(true);
  }

  openChapterSheet(): void  { this.showChapterSheet.set(true); }
  closeChapterSheet(): void { this.showChapterSheet.set(false); }

  selectChapterFromSheet(ch: number): void {
    this.showChapterSheet.set(false);
    this.selectChapter(ch);
  }

  // ── Áudio — Web Speech API ────────────────────────────────────────────────
  //
  // Usa SpeechSynthesis nativo do browser — sem backend, sem API key.
  // Lê todos os versículos do capítulo atual na língua da tradução ativa.

  toggleAudio(): void {
    if (!isPlatformBrowser(this.platform)) return;
    if (this.isPlaying()) { this.stopAudio(); return; }

    const verses = this.verses();
    if (!verses.length) return;

    const lang = this.activeTranslation() === 'kjv' ? 'en-US' : 'pt-BR';
    const text = verses.map(v => `${v.verse}. ${this.verseText(v)}`).join(' ');

    const utterance     = new SpeechSynthesisUtterance(text);
    utterance.lang      = lang;
    utterance.rate      = 0.88;
    utterance.onend     = () => this.isPlaying.set(false);
    utterance.onerror   = () => this.isPlaying.set(false);

    window.speechSynthesis.speak(utterance);
    this.isPlaying.set(true);
  }

  stopAudio(): void {
    if (!isPlatformBrowser(this.platform)) return;
    window.speechSynthesis?.cancel();
    this.isPlaying.set(false);
  }

  goBack(): void {
    if (this.view() === 'verses') {
      this.stopAudio();
      this.view.set('chapters');
      this.studyNote.set(null);
      this.noteExpanded.set(false);
      this.compareMode.set(false);
    } else if (this.view() === 'chapters') {
      this.view.set('books');
    }
  }

  // ── Sheet de seleção de versão ────────────────────────────────────────────

  openVersionSheet(): void  { this.showVersionSheet.set(true); }
  closeVersionSheet(): void { this.showVersionSheet.set(false); }

  selectVersion(key: TranslationKey): void {
    this.activeTranslation.set(key);
    this.showVersionSheet.set(false);
  }

  // ── Destaques de versículo ────────────────────────────────────────────────
  //
  // Cada capítulo tem sua própria chave no localStorage.
  // Armazena Map<verseNumber, hexColor>.

  private highlightsKey(): string {
    return `${this.HIGHLIGHTS_PREFIX}${this.selectedBook()?.id}_${this.selectedChapter()}`;
  }

  loadHighlights(): void {
    if (!isPlatformBrowser(this.platform)) return;
    try {
      const raw = localStorage.getItem(this.highlightsKey());
      if (!raw) { this.readingHighlights.set(new Map()); return; }
      const obj = JSON.parse(raw) as Record<string, string>;
      this.readingHighlights.set(new Map(Object.entries(obj).map(([k, v]) => [Number(k), v])));
    } catch {
      this.readingHighlights.set(new Map());
    }
  }

  private saveHighlights(): void {
    if (!isPlatformBrowser(this.platform)) return;
    const obj = Object.fromEntries(this.readingHighlights());
    if (Object.keys(obj).length === 0) {
      localStorage.removeItem(this.highlightsKey());
    } else {
      localStorage.setItem(this.highlightsKey(), JSON.stringify(obj));
    }
  }

  // Retorna a cor de fundo semitransparente do destaque (para o inline highlight)
  verseHighlightBg(verse: number): string {
    const hex = this.readingHighlights().get(verse);
    if (!hex) return '';
    return this.highlightColors.find(c => c.hex === hex)?.bg ?? '';
  }

  // Retorna o hex sólido (para borda na view normal)
  verseHighlightHex(verse: number): string {
    return this.readingHighlights().get(verse) ?? '';
  }

  setHighlight(verse: number, hex: string): void {
    const map = new Map(this.readingHighlights());
    // Clicar na mesma cor remove o destaque (toggle)
    if (map.get(verse) === hex) {
      map.delete(verse);
    } else {
      map.set(verse, hex);
    }
    this.readingHighlights.set(map);
    this.saveHighlights();
  }

  clearHighlight(verse: number): void {
    const map = new Map(this.readingHighlights());
    map.delete(verse);
    this.readingHighlights.set(map);
    this.saveHighlights();
  }

  // ── Seleção de versículo no modo leitura ──────────────────────────────────

  selectReadingVerse(verse: number): void {
    // Toque no mesmo verso fecha a barra de ações
    this.selectedReadingVerse.set(this.selectedReadingVerse() === verse ? null : verse);
  }

  dismissReadingSelection(): void {
    this.selectedReadingVerse.set(null);
  }

  copyReadingVerse(): void {
    const verse = this.selectedReadingVerse();
    if (verse === null) return;
    const v = this.verses().find(v => v.verse === verse);
    if (v) { this.copyVerse(v); this.selectedReadingVerse.set(null); }
  }

  shareReadingVerse(): void {
    const verse = this.selectedReadingVerse();
    if (verse === null) return;
    const v = this.verses().find(v => v.verse === verse);
    if (v) { this.shareVerse(v); this.selectedReadingVerse.set(null); }
  }

  openNoteFromReading(): void {
    const verse = this.selectedReadingVerse();
    if (verse === null) return;
    const v = this.verses().find(v => v.verse === verse);
    if (!v) return;
    this.exitReadingMode();
    // Pequeno delay para o overlay fechar antes de abrir o editor
    setTimeout(() => this.openNoteEditor(v), 150);
  }

  openReadingNoteSheet(verse: number): void {
    const existing = this.noteMap().get(verse);
    this.readingNoteText.set(existing?.note ?? '');
    this.readingNoteVerse.set(verse);
  }

  closeReadingNoteSheet(): void {
    this.readingNoteVerse.set(null);
    this.readingNoteText.set('');
  }

  saveReadingNote(): void {
    const verse = this.readingNoteVerse();
    if (verse === null) return;
    const v = this.verses().find(v => v.verse === verse);
    if (!v) return;
    this.editingNoteText.set(this.readingNoteText());
    this.editingNoteVerse.set(verse);
    this.saveNote(v);
    this.closeReadingNoteSheet();
  }

  askAiFromReading(): void {
    const verse = this.selectedReadingVerse();
    if (verse === null) return;
    const v = this.verses().find(v => v.verse === verse);
    if (!v) return;
    this.exitReadingMode();
    setTimeout(() => this.openAskAi(v), 150);
  }

  toggleBookmarkFromReading(): void {
    const verse = this.selectedReadingVerse();
    if (verse === null) return;
    const v = this.verses().find(v => v.verse === verse);
    if (v) this.toggleBookmark(v);
  }

  isBookmarkedByVerse(verse: number): boolean {
    return this.bookmarkMap().has(verse);
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
