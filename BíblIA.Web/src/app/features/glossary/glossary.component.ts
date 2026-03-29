import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ApiService } from '../../services/api.service';

export interface GlossaryTerm {
  term: string;
  category: string;
  shortDef: string;
  relatedTerms?: string[];
}

const TERMS: GlossaryTerm[] = [
  // Soteriologia
  { term: 'Justificação', category: 'Soteriologia', shortDef: 'Ato pelo qual Deus declara o pecador justo com base na obra de Cristo.', relatedTerms: ['Santificação', 'Imputação', 'Fé'] },
  { term: 'Santificação', category: 'Soteriologia', shortDef: 'Processo contínuo pelo qual o crente é transformado à imagem de Cristo.', relatedTerms: ['Justificação', 'Glorificação'] },
  { term: 'Regeneração', category: 'Soteriologia', shortDef: 'Novo nascimento espiritual operado pelo Espírito Santo no coração do eleito.', relatedTerms: ['Eleição', 'Arrependimento'] },
  { term: 'Eleição', category: 'Soteriologia', shortDef: 'Escolha soberana de Deus de salvar certos indivíduos antes da fundação do mundo.', relatedTerms: ['Predestinação', 'Graça'] },
  { term: 'Predestinação', category: 'Soteriologia', shortDef: 'Decreto eterno de Deus determinando o destino final dos seres humanos.', relatedTerms: ['Eleição', 'Providência'] },
  { term: 'Imputação', category: 'Soteriologia', shortDef: 'Atribuição da justiça de Cristo ao crente e do pecado do crente a Cristo.', relatedTerms: ['Justificação', 'Expiação'] },
  { term: 'Glorificação', category: 'Soteriologia', shortDef: 'Estado final e perfeito do crente na presença de Deus após a ressurreição.', relatedTerms: ['Santificação', 'Ressurreição'] },
  { term: 'Expiação', category: 'Soteriologia', shortDef: 'Obra de Cristo na cruz satisfazendo a justa ira de Deus pelo pecado.', relatedTerms: ['Propiciação', 'Redenção'] },
  { term: 'Propiciação', category: 'Soteriologia', shortDef: 'Satisfação da ira divina pelo sacrifício de Cristo.', relatedTerms: ['Expiação', 'Reconciliação'] },
  { term: 'Redenção', category: 'Soteriologia', shortDef: 'Libertação do poder e penalidade do pecado pelo sangue de Cristo.', relatedTerms: ['Expiação', 'Resgate'] },
  // Teologia Própria
  { term: 'Trindade', category: 'Teologia Própria', shortDef: 'Um Deus em três Pessoas distintas: Pai, Filho e Espírito Santo.', relatedTerms: ['Encarnação', 'Pneumatologia'] },
  { term: 'Aseidade', category: 'Teologia Própria', shortDef: 'Atributo de Deus de existir por si mesmo, sem dependência de nada externo.', relatedTerms: ['Onipotência', 'Imutabilidade'] },
  { term: 'Imutabilidade', category: 'Teologia Própria', shortDef: 'Perfeição divina pela qual Deus não muda em Ser, atributos ou propósitos.', relatedTerms: ['Aseidade', 'Eternidade'] },
  { term: 'Onisciência', category: 'Teologia Própria', shortDef: 'Conhecimento perfeito e total de Deus sobre todas as coisas passadas, presentes e futuras.' },
  { term: 'Providência', category: 'Teologia Própria', shortDef: 'Governo soberano de Deus sobre toda a criação para cumprir seus propósitos.', relatedTerms: ['Predestinação', 'Soberania'] },
  // Cristologia
  { term: 'Encarnação', category: 'Cristologia', shortDef: 'O eterno Filho de Deus assumindo natureza humana em Jesus Cristo.', relatedTerms: ['Trindade', 'Hipostática (União)'] },
  { term: 'União Hipostática', category: 'Cristologia', shortDef: 'Doutrina de que Cristo é uma Pessoa com duas naturezas — divina e humana —, sem confusão nem separação.', relatedTerms: ['Encarnação', 'Kenosis'] },
  { term: 'Kenosis', category: 'Cristologia', shortDef: 'Esvaziamento de Cristo ao assumir a forma de servo na encarnação (Fp 2:7).', relatedTerms: ['Encarnação', 'União Hipostática'] },
  { term: 'Ressurreição', category: 'Cristologia', shortDef: 'Retorno corporal de Jesus à vida no terceiro dia, fundamento da fé cristã.', relatedTerms: ['Glorificação', 'Parousia'] },
  { term: 'Parousia', category: 'Escatologia', shortDef: 'Segunda vinda pessoal, física e gloriosa de Cristo ao final dos tempos.' },
  // Pneumatologia
  { term: 'Pneumatologia', category: 'Pneumatologia', shortDef: 'Ramo da teologia que estuda a pessoa e obra do Espírito Santo.', relatedTerms: ['Regeneração', 'Santificação'] },
  { term: 'Cessacionismo', category: 'Pneumatologia', shortDef: 'Visão de que os dons miraculosos cessaram com o fechamento do cânon.', relatedTerms: ['Continuacionismo'] },
  { term: 'Continuacionismo', category: 'Pneumatologia', shortDef: 'Visão de que todos os dons do Espírito continuam ativos na Igreja.', relatedTerms: ['Cessacionismo'] },
  // Eclesiologia
  { term: 'Eclesiologia', category: 'Eclesiologia', shortDef: 'Estudo doutrinário da natureza, missão e governo da Igreja.', relatedTerms: ['Batismo', 'Ceia do Senhor'] },
  { term: 'Batismo', category: 'Eclesiologia', shortDef: 'Ordenança da imersão em água simbolizando morte e ressurreição com Cristo.', relatedTerms: ['Paedobatismo', 'Credobatismo'] },
  { term: 'Paedobatismo', category: 'Eclesiologia', shortDef: 'Prática do batismo de infantes como sinal da aliança de graça.', relatedTerms: ['Batismo', 'Credobatismo'] },
  { term: 'Credobatismo', category: 'Eclesiologia', shortDef: 'Posição que reserva o batismo apenas a crentes professantes.', relatedTerms: ['Batismo', 'Paedobatismo'] },
  // Hermenêutica
  { term: 'Inerrância', category: 'Bibliologia', shortDef: 'Doutrina de que as Escrituras originais são isentas de erro em tudo que afirmam.', relatedTerms: ['Infalibilidade', 'Inspiração'] },
  { term: 'Infalibilidade', category: 'Bibliologia', shortDef: 'A Bíblia não falha em seu propósito — revelar Deus e o caminho da salvação.', relatedTerms: ['Inerrância'] },
  { term: 'Inspiração', category: 'Bibliologia', shortDef: 'Ação sobrenatural do Espírito Santo sobre os autores humanos da Bíblia.', relatedTerms: ['Inerrância', 'Canon'] },
  { term: 'Hermenêutica', category: 'Bibliologia', shortDef: 'Ciência e arte da interpretação das Escrituras Sagradas.', relatedTerms: ['Exegese', 'Eisegese'] },
  { term: 'Exegese', category: 'Bibliologia', shortDef: 'Processo de extrair o significado original do texto bíblico no seu contexto.', relatedTerms: ['Hermenêutica'] },
  // Escatologia
  { term: 'Milênio', category: 'Escatologia', shortDef: 'Reinado de Cristo por mil anos mencionado em Apocalipse 20.', relatedTerms: ['Amilenismo', 'Pré-milenismo', 'Pós-milenismo'] },
  { term: 'Arrebatamento', category: 'Escatologia', shortDef: 'Evento escatológico em que os crentes vivos são trasladados para encontrar Cristo.', relatedTerms: ['Parousia', 'Tribulação'] },
  { term: 'Aliança', category: 'Teologia da Aliança', shortDef: 'Compromisso formal de Deus com seu povo, com promessas e condições.', relatedTerms: ['Nova Aliança', 'Aliança Abraâmica'] },
  { term: 'Nova Aliança', category: 'Teologia da Aliança', shortDef: 'Aliança anunciada por Jeremias e inaugurada pelo sangue de Cristo.', relatedTerms: ['Aliança', 'Justificação'] },
];

const CATEGORIES = [...new Set(TERMS.map(t => t.category))];

@Component({
  selector: 'app-glossary',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatTooltipModule],
  templateUrl: './glossary.component.html',
  styleUrls: ['./glossary.component.css']
})
export class GlossaryComponent {
  private api = inject(ApiService);

  readonly terms      = TERMS;
  readonly categories = CATEGORIES;

  activeFilter  = signal<string>('all');
  searchQuery   = signal('');
  selectedTerm  = signal<GlossaryTerm | null>(null);

  // AI explanation cache: term → explanation
  private aiCache = new Map<string, string>();
  aiLoading = signal(false);
  aiAnswer  = signal<string | null>(null);

  readonly filteredTerms = computed(() => {
    const cat = this.activeFilter();
    const q   = this.searchQuery().toLowerCase();
    return TERMS.filter(t => {
      const matchCat  = cat === 'all' || t.category === cat;
      const matchText = !q || t.term.toLowerCase().includes(q) ||
                        t.shortDef.toLowerCase().includes(q);
      return matchCat && matchText;
    }).sort((a, b) => a.term.localeCompare(b.term, 'pt-BR'));
  });

  selectTerm(term: GlossaryTerm): void {
    if (this.selectedTerm()?.term === term.term) {
      this.selectedTerm.set(null);
      this.aiAnswer.set(null);
      return;
    }
    this.selectedTerm.set(term);
    this.aiAnswer.set(this.aiCache.get(term.term) ?? null);
  }

  explainWithAi(term: GlossaryTerm): void {
    if (this.aiLoading()) return;
    const cached = this.aiCache.get(term.term);
    if (cached) { this.aiAnswer.set(cached); return; }

    this.aiLoading.set(true);
    this.aiAnswer.set(null);

    const prompt = `Explique o termo teológico "${term.term}" de forma clara e acessível para um estudante de teologia iniciante. Inclua: definição precisa, base bíblica principal (um ou dois versículos), importância doutrinária e como esse conceito se relaciona com a salvação ou a vida cristã. Seja conciso (máximo 180 palavras).`;

    this.api.askAi(prompt, 'theology').subscribe({
      next: res => {
        this.aiCache.set(term.term, res.answer);
        this.aiAnswer.set(res.answer);
        this.aiLoading.set(false);
      },
      error: () => {
        this.aiAnswer.set('Não foi possível gerar a explicação. Tente novamente.');
        this.aiLoading.set(false);
      }
    });
  }

  jumpToRelated(termName: string): void {
    const found = TERMS.find(t => t.term.toLowerCase() === termName.toLowerCase());
    if (found) {
      this.selectedTerm.set(found);
      this.aiAnswer.set(this.aiCache.get(found.term) ?? null);
    }
  }
}
