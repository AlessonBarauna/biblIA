import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ApiService, EschatologyView } from '../../services/api.service';
import { AiPanelComponent } from '../../shared/ai-panel/ai-panel.component';

@Component({
  selector: 'app-eschatology',
  standalone: true,
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatSelectModule,
    MatFormFieldModule,
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
