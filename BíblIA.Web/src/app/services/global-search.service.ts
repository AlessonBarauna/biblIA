import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, forkJoin, of } from 'rxjs';
import { debounceTime, switchMap, catchError } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ApiService, TheologyCourse, ChurchHero } from './api.service';

export type SearchResultType = 'verse' | 'course' | 'hero';

export interface SearchResult {
  type:        SearchResultType;
  icon:        string;
  title:       string;
  subtitle:    string;
  route:       string;
  queryParams?: Record<string, unknown>;
}

export interface SearchGroup {
  label:   string;
  results: SearchResult[];
}

@Injectable({ providedIn: 'root' })
export class GlobalSearchService {
  private api    = inject(ApiService);
  private router = inject(Router);

  query    = signal('');
  groups   = signal<SearchGroup[]>([]);
  loading  = signal(false);
  open     = signal(false);

  // Cache de cursos e heróis — carregados uma só vez na primeira busca
  private courses: TheologyCourse[] = [];
  private heroes:  ChurchHero[]     = [];
  private cacheLoaded = false;

  private subject = new Subject<string>();

  constructor() {
    this.subject.pipe(
      debounceTime(300),
      switchMap(q => {
        if (q.length < 3) {
          this.groups.set([]);
          this.loading.set(false);
          return of(null);
        }
        this.loading.set(true);

        // Carrega cache de cursos/heróis na primeira busca
        const cache$ = this.cacheLoaded
          ? of({ courses: this.courses, heroes: this.heroes })
          : forkJoin({
              courses: this.api.getTheologyCourses().pipe(catchError(() => of([] as TheologyCourse[]))),
              heroes:  this.api.getChurchHeroes().pipe(catchError(() => of([] as ChurchHero[])))
            });

        const verses$ = this.api.searchBibleVerses(q, 5).pipe(catchError(() => of([])));

        return forkJoin({ cache: cache$, verses: verses$ });
      }),
      // takeUntilDestroyed works without DestroyRef when called in constructor
      takeUntilDestroyed()
    ).subscribe(res => {
      if (!res) return;

      const { cache, verses } = res as any;

      if (!this.cacheLoaded) {
        this.courses     = cache.courses;
        this.heroes      = cache.heroes;
        this.cacheLoaded = true;
      }

      const q      = this.query().toLowerCase();
      const groups: SearchGroup[] = [];

      // ── Versículos ──────────────────────────────────────────────────────
      if (verses.length > 0) {
        groups.push({
          label: 'Versículos',
          results: verses.map((v: any) => ({
            type:        'verse' as SearchResultType,
            icon:        'book',
            title:       `${v.bookName} ${v.chapter}:${v.verse}`,
            subtitle:    (v.textACF || v.textKJV || '').slice(0, 80),
            route:       '/bible',
            queryParams: { bookId: v.bookId, chapter: v.chapter }
          }))
        });
      }

      // ── Cursos de teologia ───────────────────────────────────────────────
      const matchCourses = this.courses.filter(c =>
        c.title.toLowerCase().includes(q) ||
        c.description.toLowerCase().includes(q) ||
        c.category.toLowerCase().includes(q)
      ).slice(0, 4);

      if (matchCourses.length > 0) {
        groups.push({
          label: 'Cursos de Teologia',
          results: matchCourses.map(c => ({
            type:     'course' as SearchResultType,
            icon:     'school',
            title:    c.title,
            subtitle: c.category,
            route:    '/theology'
          }))
        });
      }

      // ── Heróis da história ───────────────────────────────────────────────
      const matchHeroes = this.heroes.filter(h =>
        h.name.toLowerCase().includes(q) ||
        h.biography.toLowerCase().includes(q) ||
        h.nationality.toLowerCase().includes(q)
      ).slice(0, 4);

      if (matchHeroes.length > 0) {
        groups.push({
          label: 'Heróis da Fé',
          results: matchHeroes.map(h => ({
            type:     'hero' as SearchResultType,
            icon:     'person',
            title:    h.name,
            subtitle: `${h.nationality} · ${h.period}`,
            route:    '/history'
          }))
        });
      }

      this.groups.set(groups);
      this.loading.set(false);
      this.open.set(groups.length > 0 || this.query().length >= 3);
    });
  }

  search(q: string): void {
    this.query.set(q);
    if (q.length < 3) {
      this.groups.set([]);
      this.open.set(false);
    }
    this.subject.next(q.trim());
  }

  navigate(result: SearchResult): void {
    this.clear();
    this.router.navigate([result.route], { queryParams: result.queryParams });
  }

  clear(): void {
    this.query.set('');
    this.groups.set([]);
    this.open.set(false);
    this.loading.set(false);
  }
}
