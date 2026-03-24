import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { SlicePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDividerModule } from '@angular/material/divider';
import { ApiService, ChurchHero, Revival } from '../../services/api.service';
import { AiPanelComponent } from '../../shared/ai-panel/ai-panel.component';

const CATEGORY_LABELS: Record<string, string> = {
  'Reformer':   'Reformador',
  'Missionary': 'Missionário',
  'Evangelist': 'Evangelista',
  'Revivalist': 'Avivalista',
  'Theologian': 'Teólogo',
  'Martyr':     'Mártir',
};

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatDividerModule,
    SlicePipe,
    AiPanelComponent,
  ],
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css']
})
export class HistoryComponent implements OnInit {
  private api = inject(ApiService);

  // ── Estado ────────────────────────────────────────────────────────────────
  loading = signal(false);
  error = signal<string | null>(null);

  heroes = signal<ChurchHero[]>([]);
  revivals = signal<Revival[]>([]);
  selectedCategory = signal<string | null>(null);
  selectedHero = signal<ChurchHero | null>(null);

  // Filtro de heróis por categoria
  filteredHeroes = computed(() => {
    const cat = this.selectedCategory();
    return cat ? this.heroes().filter(h => h.category === cat) : this.heroes();
  });

  // Categorias únicas presentes nos dados
  categories = computed(() =>
    [...new Set(this.heroes().map(h => h.category))].sort()
  );

  readonly categoryLabels = CATEGORY_LABELS;

  ngOnInit() {
    this.loading.set(true);
    let pending = 2;
    const done = () => { if (--pending === 0) this.loading.set(false); };

    this.api.getChurchHeroes().subscribe({
      next: (h) => { this.heroes.set(h); done(); },
      error: () => { this.error.set('Erro ao carregar heróis da fé.'); done(); }
    });

    this.api.getRevivals().subscribe({
      next: (r) => { this.revivals.set(r); done(); },
      error: () => { this.error.set('Erro ao carregar avivamentos.'); done(); }
    });
  }

  toggleCategory(cat: string) {
    this.selectedCategory.set(this.selectedCategory() === cat ? null : cat);
  }

  openHero(hero: ChurchHero) {
    this.selectedHero.set(hero);
  }

  closeHero() {
    this.selectedHero.set(null);
  }

  getCategoryLabel(cat: string): string {
    return CATEGORY_LABELS[cat] ?? cat;
  }

  getCategoryIcon(cat: string): string {
    const icons: Record<string, string> = {
      'Reformer':   'edit_document',
      'Missionary': 'flight_takeoff',
      'Evangelist': 'campaign',
      'Revivalist': 'local_fire_department',
      'Theologian': 'psychology',
      'Martyr':     'star',
    };
    return icons[cat] ?? 'person';
  }

  formatYears(hero: ChurchHero): string {
    if (!hero.birthYear && !hero.deathYear) return '';
    const birth = hero.birthYear ?? '?';
    const death = hero.deathYear ?? 'atual';
    return `(${birth}–${death})`;
  }
}
