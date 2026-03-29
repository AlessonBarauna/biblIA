import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';

export interface BibleEra {
  id: number;
  name: string;
  period: string;        // ex: "~2000–1800 a.C."
  category: 'creation' | 'patriarchs' | 'exodus' | 'conquest' | 'judges' | 'kingdom' | 'exile' | 'return' | 'intertestamental' | 'nt';
  icon: string;
  summary: string;
  keyFigures: string[];
  keyBooks: string[];
  highlight: string;     // versículo ou frase marcante
}

const ERAS: BibleEra[] = [
  {
    id: 1, name: 'Criação', period: 'Indeterminado',
    category: 'creation', icon: '🌍',
    summary: 'Deus cria o universo, a vida e o ser humano em seis dias. Adão e Eva vivem no Éden até a Queda.',
    keyFigures: ['Adão', 'Eva', 'Serpente'],
    keyBooks: ['Gênesis 1–3'],
    highlight: '"No princípio, Deus criou os céus e a terra." — Gn 1:1'
  },
  {
    id: 2, name: 'Queda e Primeiros Humanos', period: '~4000 a.C. (simbólico)',
    category: 'creation', icon: '🍎',
    summary: 'A desobediência traz o pecado e a morte. Caim mata Abel. A humanidade se multiplica e se corrompe.',
    keyFigures: ['Adão', 'Eva', 'Caim', 'Abel', 'Sete', 'Enoque'],
    keyBooks: ['Gênesis 3–5'],
    highlight: '"Onde está Abel, teu irmão?" — Gn 4:9'
  },
  {
    id: 3, name: 'O Dilúvio', period: '~3000 a.C. (simbólico)',
    category: 'creation', icon: '🌊',
    summary: 'Nada encontra graça diante de Deus. O dilúvio purifica a terra; a aliança do arco-íris renova a criação.',
    keyFigures: ['Noé', 'Sem', 'Cam', 'Jafé'],
    keyBooks: ['Gênesis 6–9'],
    highlight: '"Estabeleço minha aliança convosco." — Gn 9:11'
  },
  {
    id: 4, name: 'Torre de Babel e Dispersão', period: '~2500–2000 a.C.',
    category: 'creation', icon: '🗼',
    summary: 'Os descendentes de Noé constroem uma torre para alcançar os céus. Deus confunde as línguas e dispersa os povos.',
    keyFigures: ['Ninrode'],
    keyBooks: ['Gênesis 10–11'],
    highlight: '"Vamos edificar uma cidade e uma torre." — Gn 11:4'
  },
  {
    id: 5, name: 'Abraão — Pai da Fé', period: '~2000–1900 a.C.',
    category: 'patriarchs', icon: '⭐',
    summary: 'Deus chama Abraão de Ur e faz a aliança da promessa: terra, descendência e bênção para todas as nações.',
    keyFigures: ['Abraão', 'Sara', 'Ló', 'Hagar', 'Ismael'],
    keyBooks: ['Gênesis 12–20'],
    highlight: '"Sai da tua terra… e eu te farei uma grande nação." — Gn 12:1–2'
  },
  {
    id: 6, name: 'Isaque e Jacó', period: '~1900–1800 a.C.',
    category: 'patriarchs', icon: '🪜',
    summary: 'Isaque é o filho da promessa. Jacó luta com Deus e recebe o nome Israel; seus 12 filhos tornam-se as tribos.',
    keyFigures: ['Isaque', 'Rebeca', 'Jacó', 'Esaú', 'Lia', 'Raquel'],
    keyBooks: ['Gênesis 21–36'],
    highlight: '"Lutaste com Deus e com os homens e prevaleceste." — Gn 32:28'
  },
  {
    id: 7, name: 'José no Egito', period: '~1800–1700 a.C.',
    category: 'patriarchs', icon: '👑',
    summary: 'José, vendido como escravo, torna-se governador do Egito. A providência divina transforma o mal em bem.',
    keyFigures: ['José', 'Potifar', 'Faraó', 'Benjamim'],
    keyBooks: ['Gênesis 37–50'],
    highlight: '"Vós planejastes o mal contra mim, mas Deus o tornou em bem." — Gn 50:20'
  },
  {
    id: 8, name: 'Escravidão no Egito', period: '~1700–1446 a.C.',
    category: 'exodus', icon: '⛓️',
    summary: 'Israel cresce no Egito e é escravizado. O faraó ordena o assassinato dos bebês hebreus.',
    keyFigures: ['Moisés (nascimento)', 'Miriã', 'Faraó'],
    keyBooks: ['Êxodo 1–2'],
    highlight: '"Quanto mais os oprimiam, mais eles se multiplicavam." — Êx 1:12'
  },
  {
    id: 9, name: 'O Êxodo', period: '~1446 a.C.',
    category: 'exodus', icon: '🔥',
    summary: 'Moisés lidera Israel para fora do Egito após dez pragas. A travessia do Mar Vermelho sela a libertação.',
    keyFigures: ['Moisés', 'Arão', 'Miriã', 'Faraó Ramsés'],
    keyBooks: ['Êxodo 3–15'],
    highlight: '"Eu sou o que Sou." — Êx 3:14'
  },
  {
    id: 10, name: 'Sinai e a Lei', period: '~1446–1406 a.C.',
    category: 'exodus', icon: '📜',
    summary: 'No Monte Sinai Deus entrega os Dez Mandamentos e a Torah. Israel vaga 40 anos no deserto.',
    keyFigures: ['Moisés', 'Josué', 'Calebe', 'Aarão'],
    keyBooks: ['Êxodo 16–40', 'Levítico', 'Números', 'Deuteronômio'],
    highlight: '"Amarás ao Senhor teu Deus de todo o teu coração." — Dt 6:5'
  },
  {
    id: 11, name: 'Conquista de Canaã', period: '~1406–1380 a.C.',
    category: 'conquest', icon: '⚔️',
    summary: 'Josué lidera Israel na travessia do Jordão. Jericó cai, e as tribos recebem suas porções de terra.',
    keyFigures: ['Josué', 'Raabe', 'Calebe', 'Acã'],
    keyBooks: ['Josué'],
    highlight: '"Sede fortes e corajosos; não temais." — Js 1:9'
  },
  {
    id: 12, name: 'O Período dos Juízes', period: '~1380–1050 a.C.',
    category: 'judges', icon: '🔄',
    summary: 'Um ciclo de apostasia, opressão, clamor e libertação se repete. Juízes carismáticos lideram Israel.',
    keyFigures: ['Débora', 'Gideão', 'Sansão', 'Jefté', 'Rute', 'Boaz'],
    keyBooks: ['Juízes', 'Rute'],
    highlight: '"Naqueles dias não havia rei em Israel; cada um fazia o que parecia bem a seus olhos." — Jz 21:25'
  },
  {
    id: 13, name: 'Samuel e a Transição', period: '~1100–1050 a.C.',
    category: 'judges', icon: '🕯️',
    summary: 'Samuel, último dos juízes e primeiro dos profetas, une a era dos juízes ao reino monárquico.',
    keyFigures: ['Samuel', 'Eli', 'Ana', 'Saul'],
    keyBooks: ['1 Samuel 1–10'],
    highlight: '"Fala, pois o teu servo ouve." — 1 Sm 3:10'
  },
  {
    id: 14, name: 'O Reino Unido — Saul', period: '~1050–1010 a.C.',
    category: 'kingdom', icon: '🗡️',
    summary: 'Saul é ungido primeiro rei de Israel. Sua desobediência leva à rejeição divina e à ascensão de Davi.',
    keyFigures: ['Saul', 'Jônatas', 'Davi', 'Golias'],
    keyBooks: ['1 Samuel 11–31'],
    highlight: '"Obedecer é melhor do que sacrificar." — 1 Sm 15:22'
  },
  {
    id: 15, name: 'O Reino de Davi', period: '~1010–970 a.C.',
    category: 'kingdom', icon: '🎵',
    summary: 'Davi unifica Israel, conquista Jerusalém e recebe a promessa de um reino eterno para sua linhagem.',
    keyFigures: ['Davi', 'Bate-Seba', 'Natan', 'Absalão', 'Joabe'],
    keyBooks: ['2 Samuel', 'Salmos'],
    highlight: '"Tua casa e teu reino serão estabelecidos para sempre." — 2 Sm 7:16'
  },
  {
    id: 16, name: 'O Reino de Salomão', period: '~970–930 a.C.',
    category: 'kingdom', icon: '🏛️',
    summary: 'Salomão constrói o Templo de Jerusalém e reina no auge do poder israelita, mas sua idolatria causa a divisão.',
    keyFigures: ['Salomão', 'Rainha de Sabá', 'Reoboão'],
    keyBooks: ['1 Reis 1–11', 'Provérbios', 'Eclesiastes', 'Cantares'],
    highlight: '"A sabedoria clama nas ruas, faz ouvir a sua voz nas praças." — Pv 1:20'
  },
  {
    id: 17, name: 'Reino Dividido — Norte e Sul', period: '~930–722 a.C.',
    category: 'kingdom', icon: '⚡',
    summary: 'Israel (norte) e Judá (sul) coexistem em conflito. Profetas como Elias, Eliseu e Amós pregam arrependimento.',
    keyFigures: ['Elias', 'Eliseu', 'Acabe', 'Jezabel', 'Jeroboão'],
    keyBooks: ['1 Reis 12–22', '2 Reis 1–17', 'Amós', 'Oseias'],
    highlight: '"Até quando claudicareis entre dois pensamentos?" — 1 Rs 18:21'
  },
  {
    id: 18, name: 'Queda de Samaria — Exílio Assírio', period: '~722 a.C.',
    category: 'exile', icon: '💔',
    summary: 'A Assíria conquista o Reino do Norte. As dez tribos são deportadas e dispersas — as "tribos perdidas".',
    keyFigures: ['Salmaneser V', 'Sargão II', 'Oseias (rei)'],
    keyBooks: ['2 Reis 17', 'Oseias'],
    highlight: '"Isso aconteceu porque os filhos de Israel pecaram contra o Senhor." — 2 Rs 17:7'
  },
  {
    id: 19, name: 'Judá Sozinha — Profetas Clássicos', period: '~722–586 a.C.',
    category: 'exile', icon: '📢',
    summary: 'Judá sobrevive ao assírio mas declina. Isaías, Jeremias e Miquéias anunciam julgamento e promessa de restauração.',
    keyFigures: ['Ezequias', 'Josias', 'Isaías', 'Jeremias', 'Miquéias'],
    keyBooks: ['2 Reis 18–25', 'Isaías', 'Jeremias', 'Miquéias'],
    highlight: '"Eis que a virgem conceberá e dará à luz um filho." — Is 7:14'
  },
  {
    id: 20, name: 'Queda de Jerusalém — Exílio Babilônico', period: '~586 a.C.',
    category: 'exile', icon: '🔥',
    summary: 'Nabucodonosor destrói o Templo e deporta Judá para a Babilônia. A monarquia davídica se interrompe.',
    keyFigures: ['Nabucodonosor', 'Sedequias', 'Jeremias', 'Daniel', 'Ezequiel'],
    keyBooks: ['2 Reis 25', 'Lamentações', 'Ezequiel', 'Daniel'],
    highlight: '"Como é que jaz solitária a cidade outrora populosa!" — Lm 1:1'
  },
  {
    id: 21, name: 'Exílio na Babilônia', period: '~586–538 a.C.',
    category: 'exile', icon: '🌙',
    summary: 'Israel reflete, ora e redige as Escrituras no exílio. Daniel e Ezequiel ministram na diáspora.',
    keyFigures: ['Daniel', 'Ezequiel', 'Sadraque', 'Mesaque', 'Abede-Nego'],
    keyBooks: ['Daniel', 'Ezequiel', 'Salmos do exílio'],
    highlight: '"Como cantaremos o canto do Senhor em terra estranha?" — Sl 137:4'
  },
  {
    id: 22, name: 'Retorno do Exílio — Édito de Ciro', period: '~538–515 a.C.',
    category: 'return', icon: '🏠',
    summary: 'Ciro, rei persa, decreta o retorno dos judeus. Zorobabel lidera a primeira leva; o Templo é reconstruído.',
    keyFigures: ['Ciro', 'Zorobabel', 'Josué (sumo sacerdote)', 'Esdras'],
    keyBooks: ['Esdras 1–6', 'Ageu', 'Zacarias'],
    highlight: '"Quem é dentre vós de todo o seu povo? Suba a Jerusalém." — Ed 1:3'
  },
  {
    id: 23, name: 'Esdras, Neemias e Restauração', period: '~458–420 a.C.',
    category: 'return', icon: '🧱',
    summary: 'Esdras reforma a lei; Neemias reconstrói os muros de Jerusalém em 52 dias. O povo renova a aliança.',
    keyFigures: ['Esdras', 'Neemias', 'Sanbalate', 'Tobias', 'Ester'],
    keyBooks: ['Esdras 7–10', 'Neemias', 'Ester', 'Malaquias'],
    highlight: '"O povo tinha ânimo para trabalhar." — Ne 4:6'
  },
  {
    id: 24, name: 'Período Intertestamental', period: '~420–4 a.C.',
    category: 'intertestamental', icon: '⏳',
    summary: '400 anos de silêncio profético. Domínio persa, grego (Alexandre e hellenismo) e romano. Surgimento das seitas judaicas.',
    keyFigures: ['Alexandre Magno', 'Antíoco Epífanes', 'Judas Macabeu', 'Herodes'],
    keyBooks: ['(Apócrifos)', 'Macabeus'],
    highlight: 'O silêncio entre Malaquias e Mateus prepara o mundo para o Messias.'
  },
  {
    id: 25, name: 'Jesus Cristo — Encarnação e Igreja', period: '~4 a.C. – 100 d.C.',
    category: 'nt', icon: '✝️',
    summary: 'Jesus nasce, ministra, morre e ressuscita. O Espírito Santo é derramado em Pentecostes. Paulo espalha o Evangelho.',
    keyFigures: ['Jesus', 'Maria', 'Pedro', 'Paulo', 'João'],
    keyBooks: ['Mateus', 'Marcos', 'Lucas', 'João', 'Atos', 'Cartas', 'Apocalipse'],
    highlight: '"Porque Deus amou o mundo de tal maneira que deu o seu Filho unigênito." — Jo 3:16'
  }
];

const CATEGORY_LABELS: Record<BibleEra['category'], string> = {
  creation:        'Criação e Primeiros Humanos',
  patriarchs:      'Patriarcas',
  exodus:          'Êxodo e Deserto',
  conquest:        'Conquista',
  judges:          'Juízes',
  kingdom:         'Reino',
  exile:           'Exílio',
  return:          'Retorno',
  intertestamental:'Período Intertestamental',
  nt:              'Novo Testamento'
};

const CATEGORY_COLORS: Record<BibleEra['category'], string> = {
  creation:        '#8B5CF6',
  patriarchs:      '#D97706',
  exodus:          '#EF4444',
  conquest:        '#059669',
  judges:          '#0EA5E9',
  kingdom:         '#F59E0B',
  exile:           '#6B7280',
  return:          '#10B981',
  intertestamental:'#EC4899',
  nt:              '#3B82F6'
};

@Component({
  selector: 'app-timeline',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, MatIconModule, MatChipsModule, MatTooltipModule],
  templateUrl: './timeline.component.html',
  styleUrls: ['./timeline.component.css']
})
export class TimelineComponent {
  readonly eras = ERAS;
  readonly categoryLabels = CATEGORY_LABELS;
  readonly categoryColors = CATEGORY_COLORS;

  selectedEra   = signal<BibleEra | null>(null);
  activeFilter  = signal<BibleEra['category'] | 'all'>('all');
  searchQuery   = signal('');

  readonly categories = [...new Set(ERAS.map(e => e.category))];

  readonly filteredEras = computed(() => {
    const filter = this.activeFilter();
    const q      = this.searchQuery().toLowerCase();
    return ERAS.filter(era => {
      const matchCat  = filter === 'all' || era.category === filter;
      const matchText = !q || era.name.toLowerCase().includes(q) ||
                        era.summary.toLowerCase().includes(q) ||
                        era.keyFigures.some(f => f.toLowerCase().includes(q));
      return matchCat && matchText;
    });
  });

  selectEra(era: BibleEra): void {
    this.selectedEra.set(this.selectedEra()?.id === era.id ? null : era);
  }

  setFilter(cat: BibleEra['category'] | 'all'): void {
    this.activeFilter.set(cat);
    this.selectedEra.set(null);
  }

  colorFor(cat: BibleEra['category']): string {
    return CATEGORY_COLORS[cat];
  }
}
