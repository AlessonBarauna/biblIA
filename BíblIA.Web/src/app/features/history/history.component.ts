import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { SlicePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
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

export interface BibleEra {
  id: number;
  period: string;
  timespan: string;
  icon: string;
  color: string;
  summary: string;
  keyFigures: string[];
  narrative: string;
  keyVerses: { ref: string; text: string }[];
  theologianInsight: { theologian: string; quote: string };
  theologicalTheme: string;
}

export interface Theologian {
  name: string;
  years: string;
  tradition: string;
  icon: string;
  contribution: string;
  mainWorks: string[];
  keyQuote: string;
  theologicalEmphasis: string;
}

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
    MatExpansionModule,
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

  // ── Narrativa Bíblica (estática) ──────────────────────────────────────────
  readonly bibleEras: BibleEra[] = [
    {
      id: 1,
      period: 'Criação e Queda',
      timespan: 'Gênesis 1–3',
      icon: 'brightness_5',
      color: '#4CAF50',
      summary: 'Deus cria o universo do nada (ex nihilo), forma o homem à sua imagem (imago Dei) e institui o primeiro relacionamento de aliança. A queda introduz o pecado, a morte e a necessidade de redenção.',
      keyFigures: ['Adão', 'Eva', 'Satanás'],
      narrative: `A narrativa da criação em Gênesis 1-2 não é um manual científico, mas uma declaração teológica: Deus é soberano sobre toda criação. O ser humano é criado à imago Dei — não apenas como imagem física, mas como representante relacional e moral de Deus na terra. Adão recebe a tarefa cultural (Gênesis 1:28) de cuidar, governar e desenvolver a criação.\n\nA queda (Gênesis 3) não é apenas desobediência: é a quebra do relacionamento de confiança. Quando a serpente questiona "Será que Deus disse?", a tentação fundamental é que o ser humano se torne seu próprio juiz moral. O resultado é o que Calvino chamou de "corrupção total" — não que o ser humano seja tão mal quanto possível, mas que cada aspecto de sua natureza (intelecto, vontade, emoções) foi afetado pelo pecado.\n\nTeologicamente, Gênesis 3:15 (o protevangelho) já aponta para a redenção: a semente da mulher esmagará a cabeça da serpente. A história da redenção começa aqui.`,
      keyVerses: [
        { ref: 'Gênesis 1:27', text: 'Deus criou o homem à sua imagem, à imagem de Deus o criou; homem e mulher os criou.' },
        { ref: 'Gênesis 3:15', text: 'Porei inimizade entre ti e a mulher, entre a tua descendência e o descendente dela; este te ferirá a cabeça, e tu lhe ferirás o calcanhar.' },
      ],
      theologianInsight: {
        theologian: 'Agostinho de Hipona (354–430)',
        quote: '"Criaste-nos para Ti, e inquieto está o nosso coração enquanto não repousa em Ti." A queda explica a inquietude humana: fomos feitos para Deus e em nada mais encontramos descanso.',
      },
      theologicalTheme: 'Imago Dei · Aliança da Criação · Protevangelium · Corrupção Total',
    },
    {
      id: 2,
      period: 'Período Antediluviano',
      timespan: 'Gênesis 4–9',
      icon: 'water',
      color: '#2196F3',
      summary: 'O pecado se alastra da família de Adão às gerações seguintes. Deus julga a humanidade com o dilúvio, mas preserva Noé e sua família como sinal de graça e faz a primeira aliança cósmica.',
      keyFigures: ['Caim', 'Abel', 'Sete', 'Enoque', 'Noé'],
      narrative: `A progressão do pecado em Gênesis 4-6 é assustadora: de Caim matando Abel por inveja, ao canto de vingança de Lameque (Gênesis 4:23-24), até a corrupção total da humanidade (Gênesis 6:5 — "todo desígnio dos pensamentos do coração era sempre só o mal"). A Escritura não romantiza a condição humana.\n\nNoé representa a graça preservadora de Deus. O dilúvio é simultaneamente julgamento e salvação — um padrão que se repete na travessia do Mar Vermelho e no batismo (1 Pedro 3:20-21). Martinho Lutero viu no arca um tipo de Cristo: assim como Noé encontrou graça aos olhos do Senhor e foi preservado dentro da arca, o crente encontra salvação "em Cristo".\n\nA Aliança Noética (Gênesis 9) é a primeira aliança explícita com toda a humanidade: Deus não destruirá a terra com dilúvio novamente. O arco-íris é o sinal — uma arma de guerra virada para cima, apontando para o próprio Deus, como se Ele dissesse: "Que o julgamento caia sobre mim antes de cair sobre vocês". Alguns veem aqui um pré-anúncio da cruz.`,
      keyVerses: [
        { ref: 'Gênesis 6:8', text: 'Noé, porém, achou graça diante do Senhor.' },
        { ref: 'Gênesis 9:13', text: 'Ponho o meu arco nas nuvens, o qual será por sinal de aliança entre mim e a terra.' },
      ],
      theologianInsight: {
        theologian: 'Martinho Lutero (1483–1546)',
        quote: 'Lutero via o arca como tipo de Cristo: "Aquele que permanece fora da arca perece. Aquele que está dentro é salvo. Assim é com Cristo — fora Dele não há salvação."',
      },
      theologicalTheme: 'Julgamento e Graça · Aliança Noética · Dilúvio como Tipo Batismal',
    },
    {
      id: 3,
      period: 'Torre de Babel e Chamado de Abraão',
      timespan: 'Gênesis 10–25',
      icon: 'account_balance',
      color: '#FF9800',
      summary: 'A rebelião em Babel dispersa as nações. Deus chama Abraão da Mesopotâmia e inicia a história da redenção através de uma família específica, prometendo bênção para todos os povos da terra.',
      keyFigures: ['Noé', 'Sem', 'Abraão', 'Sara', 'Hagar', 'Isaque', 'Ismail'],
      narrative: `A Torre de Babel (Gênesis 11) é a tentativa humana de alcançar Deus pelos próprios esforços — "façamos um nome para nós". A resposta de Deus não é apenas punição, mas providência: a dispersão das nações prepara o palco para que Abraão seja chamado e, através dele, todas as famílias da terra sejam abençoadas (Gênesis 12:3).\n\nO chamado de Abraão (Gênesis 12) é o pivô central da história bíblica. Deus escolhe um homem idoso, sem filhos, de uma família que servia outros deuses (Josué 24:2), e faz promessas extraordinárias: terra, descendência numerosa e bênção universal. A fé de Abraão foi considerada justiça (Gênesis 15:6) — o versículo mais citado pelo apóstolo Paulo para explicar a justificação pela fé (Romanos 4; Gálatas 3).\n\nA oferta de Isaque (Gênesis 22) é o clímax da fé de Abraão: ele crê que Deus pode ressuscitar seu filho (Hebreus 11:19). O monte Moriá onde Isaque foi ofertado é identificado com o monte onde o templo seria construído — e onde Cristo seria crucificado. O carneiro preso pelos chifres é tipo do substituto que Deus provê.`,
      keyVerses: [
        { ref: 'Gênesis 12:3', text: 'Em ti serão benditas todas as famílias da terra.' },
        { ref: 'Gênesis 15:6', text: 'E creu ele no Senhor, e isso lhe foi imputado como justiça.' },
        { ref: 'Gênesis 22:14', text: 'Abraão chamou aquele lugar: O Senhor proverá.' },
      ],
      theologianInsight: {
        theologian: 'João Calvino (1509–1564)',
        quote: '"A fé de Abraão não era meramente intelectual — era uma confiança total na promessa de Deus mesmo contra toda evidência contrária. Isso é o que Paulo chama de \'esperar contra toda esperança\' (Romanos 4:18)."',
      },
      theologicalTheme: 'Aliança Abraâmica · Eleição · Justificação pela Fé · Tipologia Sacrificial',
    },
    {
      id: 4,
      period: 'Os Patriarcas: Isaque, Jacó e José',
      timespan: 'Gênesis 25–50',
      icon: 'family_restroom',
      color: '#9C27B0',
      summary: 'A aliança passa de Abraão para Isaque e Jacó (renomeado Israel). A história de José mostra a providência divina através da traição, escravidão e exaltação — apontando para o maior José, Jesus Cristo.',
      keyFigures: ['Isaque', 'Rebeca', 'Esaú', 'Jacó', 'Lia', 'Raquel', 'José', 'Judá'],
      narrative: `A história dos patriarcas revela que Deus trabalha através de pessoas falhas. Jacó é enganador (rouba a bênção de Esaú), mas Deus luta com ele (Gênesis 32) e o renomeia Israel ("aquele que luta com Deus"). A luta do patriarca é metáfora da vida de fé: não é de quem consegue por si só, mas de quem se agarra a Deus com tenacidade.\n\nA narrativa de José (Gênesis 37-50) é uma das mais literariamente refinadas da Bíblia. Vendido como escravo pelos irmãos, acusado falsamente, preso — cada descida parece definitiva. Mas em cada etapa, "o Senhor estava com José" (Gênesis 39:2, 21, 23). Sua exaltação ao segundo lugar no Egito não é um final feliz casual: é providência divina — "Não fostes vós que me enviastes para cá, mas Deus" (Gênesis 45:8).\n\nJosé é o mais claro tipo de Cristo no Antigo Testamento: rejeitado pelos irmãos, vendido por prata, desce ao poço/prisão (morte), exaltado ao lugar mais alto, e torna-se o provedor de vida para aqueles que o rejeitaram. Charles Spurgeon pregou extensamente sobre este paralelo.`,
      keyVerses: [
        { ref: 'Gênesis 32:28', text: 'Não te chamarás mais Jacó, mas Israel; porque lutaste com Deus e com os homens, e prevaleceste.' },
        { ref: 'Gênesis 50:20', text: 'Vós, na verdade, intentastes o mal contra mim; porém Deus o tornou em bem, para fazer o que hoje se vê: conservar a vida de muito povo.' },
      ],
      theologianInsight: {
        theologian: 'Charles Spurgeon (1834–1892)',
        quote: '"José é o espelho mais fiel de Cristo no Antigo Testamento. Em cada detalhe de seu sofrimento e exaltação, vejo as pegadas do meu Senhor. Deus registrou o futuro na vida do patriarca."',
      },
      theologicalTheme: 'Providência · Tipologia Cristológica · Eleição Incondicional · Reconciliação',
    },
    {
      id: 5,
      period: 'Êxodo, Sinai e Lei',
      timespan: 'Êxodo–Deuteronômio',
      icon: 'terrain',
      color: '#F44336',
      summary: 'Deus liberta Israel da escravidão no Egito por meio de Moisés com sinais e prodígios. No Sinai, Deus revela sua Lei e estabelece o sistema sacrificial que aponta para Cristo, o Cordeiro perfeito.',
      keyFigures: ['Moisés', 'Arão', 'Miriã', 'Faraó', 'Josué', 'Caleb'],
      narrative: `O Êxodo é o evento fundante do Antigo Testamento — o eixo em torno do qual toda a história de Israel gira. Quando Deus quer identificar a si mesmo, Ele diz: "Eu sou o Senhor teu Deus que te tirei da terra do Egito, da casa da servidão" (Êxodo 20:2). A redenção precede a Lei — Deus primeiro liberta, depois dá mandamentos. Isso é crucial: a Lei não é o caminho para ganhar favor, mas a resposta ao favor já concedido.\n\nA Páscoa (Êxodo 12) é o momento central: o sangue do cordeiro impecável pintado nas ombreiras das portas protege a família do julgamento. Paulo identifica explicitamente Cristo com esse cordeiro: "Cristo, nossa Páscoa, foi imolado" (1 Coríntios 5:7). O cordeiro devia ser sem defeito, os ossos não poderiam ser quebrados (Êxodo 12:46; João 19:36) — detalhes cumpridos na crucificação.\n\nNo Sinai, Deus estabelece o sistema sacrificial (Levítico) que seria a "pedagogia" para a Cruz. O autor de Hebreus explica: o sacerdócio levítico era imperfeito porque precisava repetir os sacrifícios. Cristo, como Sumo Sacerdote segundo a ordem de Melquisedeque, oferece um sacrifício único e permanente (Hebreus 9-10).`,
      keyVerses: [
        { ref: 'Êxodo 12:13', text: 'O sangue vos servirá de sinal nas casas onde estiverdes; vendo eu o sangue, passarei por cima de vós.' },
        { ref: 'Êxodo 20:2', text: 'Eu sou o Senhor teu Deus que te tirei da terra do Egito, da casa da servidão.' },
        { ref: 'Deuteronômio 18:15', text: 'O Senhor teu Deus te suscitará um profeta do meio de ti, de teus irmãos, semelhante a mim.' },
      ],
      theologianInsight: {
        theologian: 'Jonathan Edwards (1703–1758)',
        quote: '"Todo o sistema levítico era uma linguagem visual, uma série de sermões pregados em carne e sangue, todos dizendo: \'Sem derramamento de sangue não há remissão\'. Cristo é o cumprimento de cada tipo, cada sombra, cada sacrifício."',
      },
      theologicalTheme: 'Redenção e Lei · Tipologia Pascal · Sacerdócio Levítico · Aliança Sinaítica',
    },
    {
      id: 6,
      period: 'Conquista, Juízes e Monarquia',
      timespan: 'Josué – 2 Samuel',
      icon: 'castle',
      color: '#795548',
      summary: 'Israel entra na terra prometida, passa pelos ciclos de infidelidade dos Juízes e estabelece a monarquia. Davi recebe a promessa messiânica: seu trono será estabelecido para sempre.',
      keyFigures: ['Josué', 'Débora', 'Gideão', 'Sansão', 'Rute', 'Samuel', 'Saul', 'Davi'],
      narrative: `O livro de Josué mostra que Deus cumpre suas promessas — a terra prometida a Abraão é conquistada. Mas o padrão dos Juízes revela a condição humana: sem um rei que os governe, "cada um fazia o que parecia reto aos seus próprios olhos" (Juízes 21:25). O ciclo apostasia-opressão-clamor-livramento-apostasia se repete sete vezes, criando uma crescente descida moral.\n\nRute é um oásis no deserto dos Juízes: uma mulher moabita escolhe o Deus de Israel, mostrando que a redenção sempre foi universal em seu alcance. O conceito do "resgatador" (go'el) em Rute aponta para Cristo como o Redentor que paga o preço para restaurar o que foi perdido.\n\nDavi representa o ápice da monarquia israelita, mas também sua maior falha (2 Samuel 11-12). O Salmo 51, escrito após o pecado com Bate-Seba, é uma das mais profundas expressões de arrependimento na literatura mundial. A Aliança Davídica (2 Samuel 7) é crucial: Deus promete que o trono de Davi será eterno. O Novo Testamento abre com "livro da geração de Jesus Cristo, filho de Davi" (Mateus 1:1) — cumprindo essa promessa.`,
      keyVerses: [
        { ref: '2 Samuel 7:12-13', text: 'Levantarei depois de ti um descendente teu... e estabelecerei o trono do seu reino para sempre.' },
        { ref: 'Salmos 51:10', text: 'Cria em mim, ó Deus, um coração puro e renova dentro de mim um espírito inabalável.' },
      ],
      theologianInsight: {
        theologian: 'Dietrich Bonhoeffer (1906–1945)',
        quote: '"O Salmo 51 é a oração do ser humano que finalmente parou de se justificar e se jogou na misericórdia de Deus. Toda teologia genuína começa aqui: \'Contra ti, só contra ti, pequei.\'"',
      },
      theologicalTheme: 'Aliança Davídica · Messianismo · Arrependimento · Go\'el (Redentor)',
    },
    {
      id: 7,
      period: 'Profetas e Exílio',
      timespan: 'Isaías – Malaquias',
      icon: 'record_voice_over',
      color: '#607D8B',
      summary: 'Os profetas chamam Israel ao arrependimento, anunciam o julgamento do exílio e prometem uma nova aliança. Isaías pinta o retrato mais claro do Messias sofredor no Antigo Testamento.',
      keyFigures: ['Isaías', 'Jeremias', 'Ezequiel', 'Daniel', 'Oséias', 'Amós', 'Miquéias'],
      narrative: `O período profético é rico em tensão: julgamento e esperança, destruição e restauração. Os profetas não eram apenas "predictores" do futuro — eram pregadores apaixonados que chamavam a geração presente ao arrependimento.\n\nIsaías 53, escrito 700 anos antes de Cristo, descreve o Servo Sofredor com precisão cirúrgica: desprezado e rejeitado, varão de dores, ferido por nossas transgressões, levado como cordeiro ao matadouro. O rabino Alan Dershowitz admitiu que, se lesse esse texto sem saber a fonte, concluiria que descrevia um indivíduo específico que sofreu substitucionalmente. A Igreja sempre viu aqui Jesus.\n\nJeremias anuncia a Nova Aliança (Jeremias 31:31-34) — diferente da de Sinai, não em tábuas de pedra mas no coração. Jesus cita essa aliança na Última Ceia: "Esta taça é a nova aliança no meu sangue" (Lucas 22:20). Ezequiel completa o quadro: Deus dará um novo coração e porá seu Espírito no interior do povo (Ezequiel 36:26-27) — cumprido em Pentecostes.\n\nDaniel no exílio mostra que a fidelidade a Deus é possível mesmo na cultura que tenta assimilar e corromper. As visões de Daniel sobre o "Filho do Homem" (Daniel 7:13-14) são a base do auto-título favorito de Jesus.`,
      keyVerses: [
        { ref: 'Isaías 53:5', text: 'Mas ele foi traspassado por causa das nossas transgressões e moído por causa das nossas iniquidades; o castigo que nos traz a paz estava sobre ele, e pelas suas pisaduras fomos sarados.' },
        { ref: 'Jeremias 31:33', text: 'Porei a minha lei no seu interior e a escreverei no seu coração; eu serei o seu Deus, e eles serão o meu povo.' },
        { ref: 'Ezequiel 36:26', text: 'Dar-vos-ei coração novo e porei dentro de vós espírito novo; tirarei da vossa carne o coração de pedra e vos darei coração de carne.' },
      ],
      theologianInsight: {
        theologian: 'John Stott (1921–2011)',
        quote: '"Isaías 53 é o coração do Antigo Testamento. Se você quer entender a cruz, leia este capítulo. Cada versículo é um sermão sobre a substituição: Ele suportou o que merecíamos, para que recebamos o que Ele merecia."',
      },
      theologicalTheme: 'Expiação Substitucional · Nova Aliança · Servo Sofredor · Messias Profético',
    },
    {
      id: 8,
      period: 'Encarnação e Ministério de Jesus',
      timespan: 'Mateus – João',
      icon: 'star',
      color: '#FFC107',
      summary: 'O Verbo eterno se faz carne. Jesus nasce, é batizado, tenta, prega, faz milagres, escolhe apóstolos, e entrega sua vida na cruz como o cumprimento de toda a história bíblica anterior.',
      keyFigures: ['Jesus Cristo', 'Maria', 'José', 'João Batista', 'Pedro', 'João', 'Tiago', 'Maria Madalena'],
      narrative: `A encarnação é o evento mais extraordinário da história: o eterno Filho de Deus se torna humano sem deixar de ser divino. Os teólogos usam o termo "hipostática" (a união de duas naturezas em uma pessoa). João 1:14 — "o Verbo se fez carne" — resume o paradoxo: o criador do universo entrou em sua própria criação.\n\nO ministério de Jesus subverte todas as expectativas. Ele toca leprosos (os intocáveis), come com pecadores (os indesejáveis), elogia a fé de romanos e samaritanos (os inimigos), e chama crianças ao centro (os invisíveis). Cada milagre é um sinal do Reino: quando Jesus cura um cego, está mostrando o que a era messiânica traz; quando ressuscita Lázaro, está anunciando o que fará consigo mesmo e com todos os crentes.\n\nA crucificação deve ser lida à luz de todo o sistema sacrificial do AT. Jesus não morre como mártir ou exemplo moral apenas — Ele morre como o substituto, o Cordeiro de Deus (João 1:29) que tira o pecado do mundo. As palavras da cruz condensam a teologia: "Eli, Eli, lema sabactâni?" (abandono suportado pelo inocente em lugar do culpado) e "Está consumado!" (tetelestai = pago integralmente — o mesmo termo usado em recibos de dívidas no grego koinê).`,
      keyVerses: [
        { ref: 'João 1:14', text: 'O Verbo se fez carne e habitou entre nós, cheio de graça e de verdade.' },
        { ref: 'Marcos 10:45', text: 'O Filho do Homem não veio para ser servido, mas para servir e dar a sua vida em resgate por muitos.' },
        { ref: 'João 19:30', text: 'Quando Jesus tomou o vinagre, disse: Está consumado! E, inclinando a cabeça, entregou o espírito.' },
      ],
      theologianInsight: {
        theologian: 'Anselmo de Cantuária (1033–1109)',
        quote: '"Cur Deus Homo? (Por que Deus se tornou homem?) Porque somente um ser humano devia a satisfação, mas somente Deus podia pagá-la. Portanto, era necessário que Deus se tornasse homem."',
      },
      theologicalTheme: 'Encarnação · Hipostática · Expiação · Teologia da Cruz',
    },
    {
      id: 9,
      period: 'Ressurreição, Pentecostes e Igreja Primitiva',
      timespan: 'Atos 1–28 · Epístolas',
      icon: 'local_fire_department',
      color: '#E91E63',
      summary: 'Cristo ressuscita, a morte é derrotada. O Espírito Santo é derramado em Pentecostes e a Igreja nasce como comunidade do Reino. Paulo leva o evangelho ao mundo mediterrâneo com um corpus teológico sem igual.',
      keyFigures: ['Pedro', 'Paulo', 'Estêvão', 'Barnabé', 'Silas', 'Timóteo', 'Lídia', 'Priscila e Áquila'],
      narrative: `A ressurreição é o fundamento do cristianismo. Paulo é direto: "Se Cristo não ressuscitou, é vã a nossa fé" (1 Coríntios 15:17). Não é uma metáfora espiritual — é a reivindicação de um evento histórico, físico, verificável. As aparições pós-ressurreição (mais de 500 testemunhas, 1 Coríntios 15:6) e o túmulo vazio são os dois pilares históricos que os estudiosos precisam explicar.\n\nPentecostes (Atos 2) é o cumprimento da promessa de Joel 2:28-32 e de Jesus (João 14-16). O Espírito Santo não é uma força, mas a terceira Pessoa da Trindade que agora habita cada crente permanentemente. Pedro, que negou Cristo três vezes horas antes, pregou com tal poder que 3.000 pessoas se converteram em um dia. A transformação só pode ser explicada pelo Espírito.\n\nPaulo é o maior teólogo da história cristã. Suas cartas elaboram a doutrina da justificação pela fé (Romanos, Gálatas), a eclesiologia (Efésios, Colossenses), a escatologia (1-2 Tessalonicenses), e a ética cristã. A carta aos Romanos foi o catalisador das conversões de Agostinho, Lutero e Wesley — três marcos da história cristã separados por séculos.`,
      keyVerses: [
        { ref: '1 Coríntios 15:3-4', text: 'Cristo morreu pelos nossos pecados, segundo as Escrituras, foi sepultado e ressuscitou ao terceiro dia, segundo as Escrituras.' },
        { ref: 'Atos 2:17', text: 'Nos últimos dias, diz Deus, derramarei do meu Espírito sobre toda carne.' },
        { ref: 'Romanos 1:16-17', text: 'Não me envergonho do evangelho, pois é o poder de Deus para a salvação de todo aquele que crê... nele se revela a justiça de Deus.' },
      ],
      theologianInsight: {
        theologian: 'N. T. Wright (1948–)',
        quote: '"A ressurreição não é um evento dentro da história: é o início da nova criação irromependo dentro da história. Ela não diz que a vida espiritual vai bem — diz que a história tomou um novo rumo irreversível."',
      },
      theologicalTheme: 'Ressurreição Corporal · Pentecostes · Justificação · Igreja como Corpo',
    },
    {
      id: 10,
      period: 'Escatologia: A Consumação',
      timespan: 'Apocalipse 1–22',
      icon: 'auto_awesome',
      color: '#673AB7',
      summary: 'Apocalipse não é manual de previsões políticas — é a revelação de Jesus Cristo como Senhor soberano da história. A narrativa termina com a nova criação, a derrota definitiva do mal e o casamento do Cordeiro com sua noiva.',
      keyFigures: ['Jesus Cristo (o Cordeiro)', 'João', 'Os Sete Anjos', 'A Besta', 'O Falso Profeta'],
      narrative: `O Apocalipse foi escrito para cristãos perseguidos sob Domiciano (c. 95 d.C.). A mensagem central não é um código secreto sobre o fim dos tempos — é: "O Cordeiro já venceu. Portanto, perseverem." A linguagem simbólica (besta, dragão, números) era compreensível para leitores do século I versados em literatura apocalíptica judaica.\n\nA estrutura do livro revela a soberania de Deus sobre a história: as sete igrejas (capítulos 2-3) mostram a realidade da Igreja militante; os selos, trombetas e taças mostram julgamentos que purificam e preparam; os capítulos 12-14 revelam o conflito espiritual subjacente à perseguição; e os capítulos 19-22 descrevem o desfecho.\n\nA "nova criação" (Apocalipse 21-22) é importante: não é a aniquilação do mundo físico, mas sua renovação e glorificação. A Nova Jerusalém desce DO CÉU para a TERRA — o movimento é de cima para baixo, não de baixo para cima. Deus não resgata almas da criação; Ele resgata a criação. N.T. Wright ressalta: "O destino dos crentes não é ir para o céu e ficar lá para sempre, mas ressuscitar corporalmente em uma nova terra renovada."\n\nA última palavra da Bíblia (Apocalipse 22:20) é uma oração: "Amen! Vem, Senhor Jesus!" — a mesma oração da Igreja primitiva em aramaico: Maranatha. Toda a Bíblia aponta para esta esperança: o retorno do Rei para consumar o que a criação, a queda, os patriarcas, a lei, os profetas, a cruz e a ressurreição prepararam.`,
      keyVerses: [
        { ref: 'Apocalipse 5:9', text: 'Digno és de tomar o livro e abrir os seus selos; porque foste morto e com o teu sangue compraste para Deus homens de toda tribo, língua, povo e nação.' },
        { ref: 'Apocalipse 21:3-4', text: 'O tabernáculo de Deus está com os homens... e ele enxugará de seus olhos toda lágrima, e a morte já não existirá, já não haverá luto nem choro nem dor.' },
        { ref: 'Apocalipse 22:20', text: 'Amen! Vem, Senhor Jesus!' },
      ],
      theologianInsight: {
        theologian: 'G. K. Beale (1949–)',
        quote: '"Apocalipse é o clímax de toda a tipologia bíblica. Cada promessa do AT, cada sombra, cada tipo — tudo encontra seu cumprimento final no Cordeiro que está de pé, como se tivesse sido morto, no meio do trono."',
      },
      theologicalTheme: 'Nova Criação · Escatologia Bíblica · Soberania de Deus · Consumação',
    },
  ];

  // ── Grandes Teólogos (estáticos) ──────────────────────────────────────────
  readonly greatTheologians: Theologian[] = [
    {
      name: 'Agostinho de Hipona',
      years: '354–430 d.C.',
      tradition: 'Patrística / Catolicismo',
      icon: 'psychology',
      contribution: 'Doutrinas da graça, predestinação, pecado original e a cidade de Deus. Moldou toda a teologia ocidental subsequente.',
      mainWorks: ['Confissões', 'A Cidade de Deus', 'A Trindade', 'Sobre a Graça e o Livre-Arbítrio'],
      keyQuote: '"Criaste-nos para Ti, e inquieto está o nosso coração enquanto não repousa em Ti." — Confissões 1.1',
      theologicalEmphasis: 'Graça soberana · Pecado original · Predestinação · Dois reinos',
    },
    {
      name: 'Tomás de Aquino',
      years: '1225–1274 d.C.',
      tradition: 'Escolástica / Catolicismo',
      icon: 'menu_book',
      contribution: 'Síntese da fé cristã com a filosofia aristotélica. As "cinco vias" para provar a existência de Deus. Fundamento do catolicismo intelectual.',
      mainWorks: ['Suma Teológica', 'Suma Contra os Gentios', 'Comentário às Sentenças'],
      keyQuote: '"A graça não destrói a natureza, mas a pressupõe e a aperfeiçoa." — Suma Teológica I, q.1, a.8',
      theologicalEmphasis: 'Razão e fé · Teologia natural · Escolástica · Lei natural',
    },
    {
      name: 'Martinho Lutero',
      years: '1483–1546',
      tradition: 'Reforma Protestante / Luteranismo',
      icon: 'edit_document',
      contribution: 'Redescoberta da justificação pela fé somente (sola fide). As 95 Teses iniciaram a Reforma Protestante. Traduziu a Bíblia para o alemão, moldando o idioma e tornando a Palavra acessível ao povo.',
      mainWorks: ['As 95 Teses', 'A Liberdade do Cristão', 'O Cativeiro Babilônico', 'Catecismo Maior e Menor'],
      keyQuote: '"Aqui estou. Não posso fazer de outra forma. Deus me ajude. Amém." — Dieta de Worms, 1521',
      theologicalEmphasis: 'Sola Fide · Sola Scriptura · Sacerdócio de todos os crentes · Teologia da Cruz',
    },
    {
      name: 'João Calvino',
      years: '1509–1564',
      tradition: 'Reforma Protestante / Calvinismo/Presbiterianismo',
      icon: 'school',
      contribution: 'Sistematizou a teologia reformada. As Institutas se tornaram o tratado teológico mais influente do protestantismo. Sua visão da soberania de Deus e da predestinação moldou igrejas reformadas ao redor do mundo.',
      mainWorks: ['Institutas da Religião Cristã', 'Comentários Bíblicos (NT e AT completos)', 'Catecismo de Genebra'],
      keyQuote: '"Nosso coração é uma fábrica perpétua de ídolos." — Institutas I.11.8',
      theologicalEmphasis: 'Soberania de Deus · TULIP · Aliança · Soli Deo Gloria',
    },
    {
      name: 'Jonathan Edwards',
      years: '1703–1758',
      tradition: 'Puritanismo / Congregacionalismo',
      icon: 'auto_awesome',
      contribution: 'O maior teólogo americano. Combinou profundidade filosófica, fervor bíblico e experiência espiritual. Pregador do Grande Avivamento. Sua obra sobre a vontade humana é considerada a maior obra filosófica produzida na América.',
      mainWorks: ['A Liberdade da Vontade', 'A Natureza da Virtude Verdadeira', 'Marcas da Obra do Espírito', 'Pecadores nas Mãos de um Deus Irado'],
      keyQuote: '"Deus é o ser mais glorioso e a contemplação de Sua glória é o prazer mais elevado possível a uma criatura." — A Natureza da Virtude Verdadeira',
      theologicalEmphasis: 'Glória de Deus · Avivamento · Filosofia da religião · Afetos religiosos',
    },
    {
      name: 'Charles Spurgeon',
      years: '1834–1892',
      tradition: 'Batista / Calvinismo',
      icon: 'campaign',
      contribution: 'O "Príncipe dos Pregadores". Pregou para 10 milhões de pessoas ao longo da vida. Seus sermões (3.600 publicados) são os mais lidos após a Bíblia. Fundou o Colégio de Pastores e o Orfanato de Stockwell.',
      mainWorks: ['The Metropolitan Tabernacle Pulpit (63 volumes)', 'Tesouro de Davi', 'Lectures to My Students', 'Gleanings Among the Sheaves'],
      keyQuote: '"Uma Bíblia que está se desfazendo pertence a uma pessoa que não está." — Sermão sobre 2 Timóteo 3:16',
      theologicalEmphasis: 'Pregação expositiva · Graça soberana · Doutrina reformada · Evangelismo',
    },
    {
      name: 'Dietrich Bonhoeffer',
      years: '1906–1945',
      tradition: 'Luteranismo / Confessante',
      icon: 'star',
      contribution: 'Teólogo e pastor que resistiu ao nazismo com o custo da vida. Sua obra sobre a "graça barata vs graça cara" e o discipulado são leituras essenciais para a ética cristã. Executado a 39 anos.',
      mainWorks: ['O Custo do Discipulado', 'Vida em Comunhão', 'Ética', 'Cartas e Devocional do Cárcere'],
      keyQuote: '"A graça barata é o pregão do perdão sem o arrependimento, o batismo sem a disciplina da Igreja, a Comunhão sem a confissão... A graça cara custa tudo ao homem, mas ela é a única graça verdadeira." — O Custo do Discipulado',
      theologicalEmphasis: 'Discipulado · Ética cristã · Resistência ao totalitarismo · Graça e custo',
    },
    {
      name: 'C. S. Lewis',
      years: '1898–1963',
      tradition: 'Anglicanismo',
      icon: 'local_library',
      contribution: 'O maior apologista cristão do século XX. Convertido de ateu convicto ao anglicanism, usou a ficção (Nárnia, Perelandra) e a argumentação filosófica para comunicar o evangelho a gerações de leitores.',
      mainWorks: ['Cristianismo Puro e Simples', 'Cartas do Inferno', 'Deus no Banco dos Réus', 'As Crônicas de Nárnia', 'O Problema do Sofrimento'],
      keyQuote: '"Ou este homem era, e é, o Filho de Deus — ou era um louco ou algo pior. Você pode tapá-lo por tolo, pode cuspir nele e matá-lo como um demônio, ou pode cair a seus pés e chamá-lo de Senhor e Deus. Mas não venha com a conversa de que era apenas um grande mestre humano." — Cristianismo Puro e Simples',
      theologicalEmphasis: 'Apologética · Trilemma de Lewis · Alegoria redentora · Razão e fé',
    },
  ];

  selectedEra = signal<BibleEra | null>(null);
  selectedTheologian = signal<Theologian | null>(null);

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

  openHero(hero: ChurchHero) { this.selectedHero.set(hero); }
  closeHero() { this.selectedHero.set(null); }
  openEra(era: BibleEra) { this.selectedEra.set(era); }
  closeEra() { this.selectedEra.set(null); }
  openTheologian(t: Theologian) { this.selectedTheologian.set(t); }
  closeTheologian() { this.selectedTheologian.set(null); }

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
