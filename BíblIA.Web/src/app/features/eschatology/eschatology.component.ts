import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { ApiService, EschatologyView } from '../../services/api.service';
import { AiPanelComponent } from '../../shared/ai-panel/ai-panel.component';

// Linha da tabela comparativa — cada campo vira uma linha com valor de cada visão
interface CompareRow { label: string; icon: string; key: keyof EschatologyView; highlight: boolean; }

@Component({
  selector: 'app-eschatology',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatSelectModule,
    MatFormFieldModule,
    MatChipsModule,
    AiPanelComponent,
  ],
  templateUrl: './eschatology.component.html',
  styleUrls: ['./eschatology.component.css']
})
export class EschatologyComponent implements OnInit {
  private api = inject(ApiService);

  // ── Estado ────────────────────────────────────────────────────────────────
  loading = signal(false);
  error = signal<string | null>(null);

  views = signal<EschatologyView[]>([]);
  selectedView = signal<EschatologyView | null>(null);

  // Modo comparação
  compareMode = signal(false);
  compareId1 = signal<number | null>(null);
  compareId2 = signal<number | null>(null);
  comparison = signal<{ view1: EschatologyView; view2: EschatologyView; comparison: any } | null>(null);
  comparingLoading = signal(false);

  canCompare = computed(() => this.compareId1() !== null && this.compareId2() !== null && this.compareId1() !== this.compareId2());

  // Definição da tabela — ordem e rótulos de cada atributo comparado
  readonly compareRows: CompareRow[] = [
    { label: 'Resumo',          icon: 'info',          key: 'summary',              highlight: false },
    { label: 'Milênio',         icon: 'hourglass_bottom', key: 'millenniumView',    highlight: true  },
    { label: 'Arrebatamento',   icon: 'flight_takeoff', key: 'raptureView',         highlight: true  },
    { label: 'Tribulação',      icon: 'warning',       key: 'tribulationView',      highlight: true  },
    { label: 'Teólogos',        icon: 'person',        key: 'mainTheologians',      highlight: false },
    { label: 'Escrituras-chave',icon: 'menu_book',     key: 'keyScriptures',        highlight: false },
    { label: 'Pontos fortes',   icon: 'thumb_up',      key: 'strengths',            highlight: false },
    { label: 'Pontos fracos',   icon: 'thumb_down',    key: 'weaknesses',           highlight: false },
  ];

  ngOnInit() {
    this.loading.set(true);
    this.api.getEschatologyViews().subscribe({
      next: (v) => { this.views.set(v); this.loading.set(false); },
      error: () => { this.error.set('Erro ao carregar visões escatológicas.'); this.loading.set(false); }
    });
  }

  openView(view: EschatologyView) {
    this.selectedView.set(view);
  }

  closeView() {
    this.selectedView.set(null);
  }

  enterCompareMode() {
    this.compareMode.set(true);
    this.comparison.set(null);
    this.compareId1.set(null);
    this.compareId2.set(null);
  }

  exitCompareMode() {
    this.compareMode.set(false);
    this.comparison.set(null);
  }

  runComparison() {
    const id1 = this.compareId1();
    const id2 = this.compareId2();
    if (!id1 || !id2) return;

    this.comparingLoading.set(true);
    this.api.compareEschatologyViews(id1, id2).subscribe({
      next: (result) => { this.comparison.set(result); this.comparingLoading.set(false); },
      error: () => { this.error.set('Erro ao comparar visões.'); this.comparingLoading.set(false); }
    });
  }
}
