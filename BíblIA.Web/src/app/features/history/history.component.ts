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
    {
      id: 11,
      period: 'Sabedoria: Jó, Salmos e Provérbios',
      timespan: 'Jó · Salmos · Provérbios · Eclesiastes · Cantares',
      icon: 'menu_book',
      color: '#3F51B5',
      summary: 'A literatura sapiencial de Israel aborda as grandes perguntas da existência: o sofrimento do justo, a adoração como resposta ao cosmos, a sabedoria prática para a vida e o sentido da existência.',
      keyFigures: ['Jó', 'Davi', 'Salomão', 'Asafe', 'Coré'],
      narrative: `Jó é o mais antigo livro da Bíblia e enfrenta a questão mais urgente: por que o justo sofre? A resposta de Jó recusa os clichês religiosos de seus amigos (que equacionam sofrimento com pecado) e wrestling com Deus até chegar a um encontro face a face (Jó 38-42). A conclusão não é uma explicação filosófica, mas um encontro: "Os meus ouvidos tinham ouvido falar de ti, mas agora os meus olhos te veem" (Jó 42:5). O sofrimento pode ser o caminho para conhecer Deus mais profundamente.\n\nOs Salmos são o livro de oração e adoração de Israel — e de Cristo. Jesus orou os Salmos na cruz (Salmo 22:1 = "Meu Deus, por que me abandonaste?"). Os Salmos Messiânicos (2, 22, 45, 72, 110) são citados mais de 40 vezes no NT como cumpridos em Cristo. Dietrich Bonhoeffer ensinou que os Salmos são a oração de Jesus em nós, pelo Espírito.\n\nProvérbios ensina que a sabedoria não é conhecimento técnico, mas orientação moral: o temor do Senhor é o começo da sabedoria (Provérbios 1:7). A Sabedoria personificada em Provérbios 8 (presente na criação) é identificada por Paulo com Cristo: "em quem estão escondidos todos os tesouros da sabedoria" (Colossenses 2:3).`,
      keyVerses: [
        { ref: 'Jó 42:5', text: 'Os meus ouvidos tinham ouvido falar de ti, mas agora os meus olhos te veem.' },
        { ref: 'Salmos 1:1-2', text: 'Bem-aventurado o homem que não anda no conselho dos ímpios... antes tem o seu prazer na lei do Senhor.' },
        { ref: 'Provérbios 1:7', text: 'O temor do Senhor é o princípio da sabedoria.' },
      ],
      theologianInsight: {
        theologian: 'Dietrich Bonhoeffer (1906–1945)',
        quote: '"Os Salmos são as orações de Davi, e, ao mesmo tempo, as orações de Cristo. Cristo ora em nós e nós oramos em Cristo. Por isso os Salmos são para sempre a oração da Igreja."',
      },
      theologicalTheme: 'Teodiceia · Adoração · Sabedoria Prática · Cristo a Sabedoria de Deus',
    },
    {
      id: 12,
      period: 'Salomão e a Construção do Templo',
      timespan: '1 Reis 1–11',
      icon: 'account_balance',
      color: '#FF5722',
      summary: 'Salomão constrói o Templo de Jerusalém — o símbolo da presença de Deus com Israel. Sua sabedoria proverbial e sua queda trágica ensinam que a verdadeira sabedoria exige mais que inteligência: exige um coração fiel.',
      keyFigures: ['Salomão', 'Davi', 'Hirão de Tiro', 'A Rainha de Sabá'],
      narrative: `A construção do Templo (1 Reis 6-8) é o clímax do reinado de Salomão. O Templo não é apenas uma estrutura religiosa — é a manifestação visível da habitação de Deus entre seu povo. Quando a glória de Deus (shekinah) encheu o templo de tal forma que os sacerdotes não podiam entrar (1 Reis 8:10-11), Israel experimentou o cume de sua história.\n\nA oração de dedicação de Salomão (1 Reis 8:22-53) é um modelo de intercessão que reconhece tanto a santidade de Deus ("os céus dos céus não te podem conter") quanto sua misericórdia. Jesus usaria o Templo como metáfora de seu próprio corpo (João 2:21), e João vê na nova criação que "não havia templo" porque Deus mesmo e o Cordeiro são o Templo (Apocalipse 21:22).\n\nA queda de Salomão (1 Reis 11) é trágica precisamente porque era evitável: ele sabia a verdade. Sua apostasia foi gradual — primeiro tolerou os deuses das esposas estrangeiras, depois os adorou. Isso ilustra que o conhecimento sem obediência é mais perigoso que a ignorância, pois engendra hipocrisia.`,
      keyVerses: [
        { ref: '1 Reis 8:27', text: 'Mas, na verdade, habitará Deus na terra? Eis que os céus e os céus dos céus não te podem conter.' },
        { ref: '1 Reis 8:30', text: 'Ouve, pois, a súplica do teu servo e do teu povo Israel, quando orarem em direção a este lugar.' },
      ],
      theologianInsight: {
        theologian: 'Matthew Henry (1662–1714)',
        quote: '"A queda de Salomão é um aviso eterno: nenhum dom, nenhuma sabedoria, nenhuma experiência passada protege quem abandona a vigilância do coração. O maior sábio do mundo foi derrubado pela cobiça e pela sensualidade."',
      },
      theologicalTheme: 'Templo como Habitação Divina · Shekinah · Queda pela Apostasia Gradual',
    },
    {
      id: 13,
      period: 'O Reino Dividido: Israel e Judá',
      timespan: '1 Reis 12 – 2 Reis 17',
      icon: 'call_split',
      color: '#795548',
      summary: 'Após Salomão, o reino se divide em dois: Israel (norte, 10 tribos) e Judá (sul, 2 tribos). Israel mergulha em apostasia sob Jeroboão, enquanto Judá oscila entre reis fiéis e infiéis. Ambos experimentarão o julgamento do exílio.',
      keyFigures: ['Roboão', 'Jeroboão', 'Acabe', 'Jezabel', 'Elias', 'Josafá', 'Ezequias', 'Josias'],
      narrative: `A divisão do reino (1 Reis 12) ocorre por um ato de hubris de Roboão, cumprindo a palavra profética (1 Reis 11:29-39). Jeroboão, para evitar que o povo vá a Jerusalém adorar, cria dois bezerros de ouro em Betel e Dã — uma apostasia deliberada que ecoa o episódio do bezerro de ouro no Sinai. A avaliação de todos os reis de Israel é "fez o mal aos olhos do Senhor".\n\nA saga de Elias e Jezabel (1 Reis 18-21) é um dos mais dramáticos confrontos entre verdade e poder na Bíblia. No Monte Carmelo, Elias confronta 450 profetas de Baal num desafio que demonstra que o Deus de Israel responde por fogo. Mas horas depois, ameaçado por uma mulher, Elias foge ao deserto em depressão profunda — um retrato honesto da vulnerabilidade mesmo dos maiores profetas.\n\nOs reis bons de Judá — Ezequias (reforma, oração, libertação de Senaqueribe) e Josias (redescoberta da Lei, maior reforma religiosa) — mostram que a renovação é possível. Mas não é suficiente: o pecado acumulado de gerações inteiras exige julgamento. O exílio não é capricho divino, mas consequência justa e anunciada.`,
      keyVerses: [
        { ref: '1 Reis 18:21', text: 'Elias se chegou a todo o povo e disse: Até quando coxeareis entre dois pensamentos? Se o Senhor é Deus, segui-o; porém se Baal, segui a este.' },
        { ref: '2 Reis 17:23', text: 'Até que o Senhor removeu Israel de diante da sua face... e Israel foi levado cativo da sua terra para a Assíria.' },
      ],
      theologianInsight: {
        theologian: 'C. S. Lewis (1898–1963)',
        quote: '"A história dos reis de Israel é a história de toda alma humana: a tentação de servir dois senhores, de manter uma religião conveniente que não exige nada. Mas Deus não aceita ser um entre muitos — Ele é o Senhor, ou não é nada."',
      },
      theologicalTheme: 'Fidelidade da Aliança · Apostasia Nacional · Julgamento Profético · Providência nos Reis',
    },
    {
      id: 14,
      period: 'Daniel: Fidelidade e Visões no Exílio',
      timespan: 'Daniel 1–12',
      icon: 'visibility',
      color: '#1A237E',
      summary: 'Daniel e seus companheiros demonstram que é possível ser fiel a Deus em meio a uma cultura hostil. As visões apocalípticas de Daniel sobre o Filho do Homem e os reinos do mundo são a base do linguajar apocalíptico de Jesus e do Apocalipse.',
      keyFigures: ['Daniel', 'Hananias', 'Misael', 'Azarias', 'Nabucodonosor', 'Belsazar', 'Dario'],
      narrative: `Daniel é o manual de vida para cristãos em culturas pós-cristãs. A pressão para se conformar é sistemática: mudar o nome, comer a comida do rei, adorar a estátua de ouro. A resistência de Daniel e seus amigos não é fanatismo — é fidelidade calculada: "o nosso Deus pode livrar-nos... e, mesmo que não nos livre, fica sabendo que não serviremos aos teus deuses" (Daniel 3:17-18). O "mesmo que não" é a fé mais madura: que confia independente do resultado.\n\nAs visões de Daniel (capítulos 7-12) são o material mais denso e debatido do AT. A visão do "Filho do Homem" (Daniel 7:13-14) recebendo domínio eterno de Deus é o texto que Jesus mais citou para si mesmo. Ao ser interrogado pelo Sumo Sacerdote, Jesus disse: "vocês verão o Filho do Homem sentado à direita do Poder e vindo nas nuvens do céu" (Marcos 14:62) — uma afirmação de divindade tão clara que o Sumo Sacerdote rasgou suas vestes.\n\nA "abominação da desolação" (Daniel 9:27; 11:31) foi parcialmente cumprida por Antíoco IV Epifânio (167 a.C.) e é citada por Jesus como modelo para o fim dos tempos (Mateus 24:15). Daniel ensina que há um padrão providencial na história: impérios sobem e caem, mas o reino de Deus é eterno.`,
      keyVerses: [
        { ref: 'Daniel 3:17-18', text: 'O nosso Deus, a quem servimos, pode livrar-nos... e, mesmo que não nos livre, fica sabendo, ó rei, que não serviremos aos teus deuses.' },
        { ref: 'Daniel 7:13-14', text: 'Vi em visões da noite que vinha com as nuvens do céu um como filho de homem... e foi-lhe dado domínio, e glória, e um reino, para que todos os povos o servissem.' },
      ],
      theologianInsight: {
        theologian: 'R. C. Sproul (1939–2017)',
        quote: '"Daniel 7 é a passagem mais importante do AT para entender quem Jesus afirma ser. Quando Jesus aplica este título a si mesmo, os ouvintes judeus entendiam perfeitamente: ele estava reivindicando ser o agente divino de Deus que recebe domínio eterno."',
      },
      theologicalTheme: 'Fidelidade Cultural · Apocalíptica · Soberania sobre Impérios · Filho do Homem',
    },
    {
      id: 15,
      period: 'A Restauração: Esdras, Neemias e Ester',
      timespan: 'Esdras · Neemias · Ester',
      icon: 'home_repair_service',
      color: '#00695C',
      summary: 'Deus cumpre sua promessa por meio de Ciro da Pérsia. Os exilados retornam, o Templo é reconstruído, os muros de Jerusalém são restaurados e Ester salva o povo de um genocídio — tudo movido pela providência divina.',
      keyFigures: ['Ciro da Pérsia', 'Esdras', 'Neemias', 'Ester', 'Mordecai', 'Hamã', 'Zorobabel', 'Josué'],
      narrative: `O decreto de Ciro (Esdras 1:1-4) é historicamente verificado — o Cilindro de Ciro no British Museum confirma sua política de repatriar povos deportados. Isaías havia profetizado o nome de Ciro como libertador 150 anos antes (Isaías 44:28-45:1) — um dos mais notáveis exemplos de profecia específica nas Escrituras.\n\nNeemias é um modelo de liderança. Ao ouvir que os muros de Jerusalém estavam destruídos, ele primeiro chorou e jejuou (Neemias 1), depois planejou estrategicamente (Neemias 2), organizou o trabalho apesar da oposição feroz (Neemias 4: "com uma mão faziam a obra e com a outra seguravam a espada"), e completou os muros em 52 dias (Neemias 6:15) — o que seus inimigos atribuíram a Deus.\n\nEster é o único livro do AT onde o nome de Deus não aparece explicitamente — mas a providência divina permeia cada cena. O "e se eu perecer, que pereça" de Ester (Ester 4:16) é um dos momentos de maior coragem na Bíblia. O festival de Purim (sortes) mostra que aquilo que parecia acaso (o sorteio de Hamã) estava sob o controle soberano de Deus.`,
      keyVerses: [
        { ref: 'Esdras 1:2', text: 'O Senhor Deus dos céus me deu todos os reinos da terra e me encarregou de edificar-lhe uma casa em Jerusalém, que está em Judá.' },
        { ref: 'Neemias 4:17', text: 'Com uma das mãos faziam a obra, e com a outra seguravam a espada.' },
        { ref: 'Ester 4:14', text: 'Quem sabe se para tal tempo como este é que chegaste ao reino?' },
      ],
      theologianInsight: {
        theologian: 'R. C. Sproul (1939–2017)',
        quote: '"Ester nos lembra que a providência de Deus opera nos bastidores da história através de decisões humanas aparentemente banais. O acaso do sorteio de Hamã estava nas mãos do Soberano que não dorme."',
      },
      theologicalTheme: 'Providência Divina · Fidelidade de Deus às Promessas · Liderança Piedosa · Coragem',
    },
    {
      id: 16,
      period: 'Os 400 Anos de Silêncio',
      timespan: '430 a.C. – 4 a.C.',
      icon: 'hourglass_empty',
      color: '#546E7A',
      summary: 'Entre Malaquias e o nascimento de Cristo, Deus parece silencioso por 400 anos. Mas nesse período, a Providência preparou o mundo para o evangelho: língua grega comum, direito romano, esperança messiânica e dispersão judaica.',
      keyFigures: ['Alexandre Magno', 'Antíoco IV Epifânio', 'Os Macabeus', 'Herodes o Grande', 'João Hiriano'],
      narrative: `Malaquias termina com uma promessa: "Eis que vos enviarei o profeta Elias antes que venha o grande e terrível dia do Senhor" (Malaquias 4:5). E então — silêncio. Quatro séculos sem voz profética. Esse silêncio foi tão pesado que quando João Batista apareceu, a primeira pergunta foi: "Você é Elias?" O silêncio acentuou a expectativa.\n\nMas o silêncio de Deus não é ausência de providência. Alexandre Magno (356-323 a.C.) conquistou o mundo mediterrâneo e espalhou a língua grega (koinê) — que se tornaria o idioma do Novo Testamento e da evangelização inicial. Roma construiu estradas que Paulo usaria em suas viagens missionárias. A perseguição de Antíoco IV Epifânio (167 a.C.) e a revolta macabeia aprofundaram a esperança messiânica.\n\nA dispersão judaica (Diáspora) plantou sinagogas pelo mundo mediterrâneo — pontos de contato que Paulo sempre usou como porta de entrada em cada cidade. O monoteísmo ético do judaísmo havia preparado "prosélitos" e "tementes a Deus" que estavam prontos para receber o cumprimento das promessas. Quando Jesus nasceu, "a plenitude do tempo" (Gálatas 4:4) era precisamente esse mundo providencialmente preparado.`,
      keyVerses: [
        { ref: 'Malaquias 4:5', text: 'Eis que vos enviarei o profeta Elias antes que venha o grande e terrível dia do Senhor.' },
        { ref: 'Gálatas 4:4', text: 'Mas, quando veio a plenitude do tempo, Deus enviou seu Filho, nascido de mulher.' },
      ],
      theologianInsight: {
        theologian: 'F. F. Bruce (1910–1990)',
        quote: '"Os 400 anos de silêncio não foram de inatividade divina. Foram o período de gestação do mundo que receberia o evangelho. Deus preparou o útero da história para o nascimento de Cristo."',
      },
      theologicalTheme: 'Providência na História Secular · Plenitude do Tempo · Preparação para o Evangelho',
    },
    {
      id: 17,
      period: 'O Sermão da Montanha',
      timespan: 'Mateus 5–7',
      icon: 'terrain',
      color: '#00BCD4',
      summary: 'O Sermão da Montanha é a magna carta do Reino de Deus. Jesus não abole a Lei — ele a cumpre e a aprofunda, revelando que o problema não é apenas o comportamento externo, mas o coração. As Bem-aventuranças invertem todos os valores humanos.',
      keyFigures: ['Jesus', 'Os Doze Apóstolos', 'As multidões'],
      narrative: `O Sermão da Montanha começa com as Bem-aventuranças (Mateus 5:3-12) — paradoxos do Reino que subvertem toda sabedoria mundana. Felizes os pobres de espírito, os que choram, os mansos, os perseguidos. Não é ascetismo ou masoquismo: é a descrição de quem encontrou que os recursos humanos não são suficientes e depende totalmente de Deus.\n\nJesus repete seis vezes: "Ouvistes que foi dito... mas eu vos digo" (Mateus 5:21-48). Ele não anula a Lei — ele vai à raiz. Não é apenas "não matar": é não ter ódio. Não é apenas "não adulterar": é não cultivar lascívia. A ética do Reino é uma ética do coração, não de performance exterior. Martinho Lutero entendeu isso: a Lei revela o pecado em sua profundidade para nos lançar à graça.\n\nO "Pai Nosso" (Mateus 6:9-13) é o modelo da oração cristã: começa com Deus ("Pai nosso... santificado seja o teu nome, venha o teu reino"), e só então pede provisão, perdão e proteção. A ordem importa: a doxologia precede o pedido, a relação precede a requisição. A expressão "Pai nosso" era radicalmente nova no mundo judeu — Abba era o termo familiar íntimo.`,
      keyVerses: [
        { ref: 'Mateus 5:3', text: 'Bem-aventurados os pobres de espírito, porque deles é o reino dos céus.' },
        { ref: 'Mateus 5:17', text: 'Não cuideis que vim destruir a Lei ou os Profetas; não vim destruir, mas cumprir.' },
        { ref: 'Mateus 6:33', text: 'Buscai primeiro o reino de Deus e a sua justiça, e todas as demais coisas vos serão acrescentadas.' },
      ],
      theologianInsight: {
        theologian: 'Dietrich Bonhoeffer (1906–1945)',
        quote: '"O Sermão da Montanha não é um ideal impossível — é a descrição da vida de quem foi liberto pela graça. Não é lei para ganhar o reino, mas a ética daqueles que já entraram no reino. A graça barata diz que não precisa obedecer; a graça cara diz que só pode obedecer aquele que foi liberto."',
      },
      theologicalTheme: 'Ética do Reino · Lei do Coração · Graça e Obediência · A Oração do Senhor',
    },
    {
      id: 18,
      period: 'As Parábolas do Reino',
      timespan: 'Mateus 13 · Lucas 10–19',
      icon: 'lightbulb',
      color: '#FF9800',
      summary: 'Nas parábolas, Jesus ensina os mistérios do Reino de Deus em linguagem acessível mas profunda. O filho pródigo, o bom samaritano e o semeador revelam a natureza da graça, da misericórdia e da recepção do evangelho.',
      keyFigures: ['Jesus', 'Os Fariseus', 'Os Discípulos'],
      narrative: `Jesus ensinou em parábolas por uma razão surpreendente: "para que vendo vejam e não percebam" (Marcos 4:12, citando Isaías 6:9). As parábolas não são apenas histórias pedagógicas — são véus que revelam ao coração aberto e ocultam ao coração endurecido. O mesmo sol que derrete a cera endurece o barro.\n\nA parábola do Filho Pródigo (Lucas 15:11-32) é chamada por Rembrandt de "a mais bela parábola que existe". Mas ela é realmente a parábola do Pai Misericordioso: o filho mais novo representa os pecadores óbvios; o filho mais velho representa os fariseus — igualmente perdidos, porém em rebeldia disfarçada de obediência. O pai corre ao encontro do filho que volta — imagem escandalosa numa cultura onde homens de honra não corriam. Deus corre.\n\nA parábola do Bom Samaritano (Lucas 10:25-37) responde a pergunta "quem é meu próximo?" com uma inversão: não pergunta quem merece minha misericórdia, mas como posso ser misericordioso independente de quem precise. O samaritano era o inimigo racial e religioso do ouvinte judeu — Jesus faz o herói exatamente quem o ouvinte menos esperava.`,
      keyVerses: [
        { ref: 'Lucas 15:20', text: 'Ainda estava longe quando seu pai o viu e, cheio de compaixão, correu para ele, jogou-se ao seu pescoço e o beijou.' },
        { ref: 'Mateus 13:44', text: 'O reino dos céus é semelhante a um tesouro escondido num campo, que um homem, ao encontrá-lo, escondeu outra vez; e, na sua alegria, vai, vende tudo o que tem e compra aquele campo.' },
      ],
      theologianInsight: {
        theologian: 'Kenneth Bailey (1930–2016)',
        quote: '"Na cultura do Oriente Médio, um pai não corria — era indigno. Jesus descreve Deus correndo para receber o filho perdido. Esta é a maior afirmação sobre o amor de Deus em toda a Escritura: ele corre."',
      },
      theologicalTheme: 'Graça Inmerecida · Misericórdia sem Fronteiras · O Coração do Pai · Recepção do Evangelho',
    },
    {
      id: 19,
      period: 'A Semana da Paixão e a Crucificação',
      timespan: 'Mateus 21–27 · João 13–19',
      icon: 'favorite_border',
      color: '#B71C1C',
      summary: 'A última semana de Jesus condensa toda a história bíblica: a entrada triunfal cumpre Zacarias 9:9, a Última Ceia institui a Nova Aliança, Getsêmani revela o peso do pecado humano, e a Cruz é o centro do cosmos.',
      keyFigures: ['Jesus', 'Judas', 'Pedro', 'Pilatos', 'Caifás', 'Maria (de Betânia)', 'João'],
      narrative: `A entrada triunfal (Mateus 21:1-11) é calculada: Jesus monta num jumento cumprindo Zacarias 9:9 com precisão, e a multidão grita "Hosana" (Salmo 118:25-26). Não era uma entrada por acaso — era a reivindicação pública de messianidade. Os fariseus entenderam; por isso pediram que Jesus silenciasse os discípulos. Ele respondeu: "Se eles ficarem calados, as pedras clamarão" (Lucas 19:40).\n\nGetsêmani (Mateus 26:36-46) é o momento mais íntimo dos Evangelhos. Jesus, que havia levantado Lázaro dos mortos com autoridade, está agora "com angústia mortal". A oração "se possível, passe de mim este cálice" não é falta de fé — é a humanidade plena de Cristo sentindo o peso de carregar o pecado de toda a humanidade. A diferença entre o cálice de sofrimento físico e o cálice do abandono (Salmo 22) que se avizinhava.\n\nA Cruz é o centro da história. As sete palavras de Jesus na cruz são uma teologia condensada: perdão para os algozes, salvação para o ladrão arrependido, cuidado pela mãe, o clamor do abandono (Salmo 22), a sede cumprindo profecia (Salmo 69:21), "está consumado" (tetelestai = pago integralmente) e a entrega do espírito. O véu do templo rasgado de cima a baixo (Mateus 27:51) anuncia: o acesso a Deus está aberto para todos.`,
      keyVerses: [
        { ref: 'Mateus 26:39', text: 'Meu Pai, se possível, passe de mim este cálice; todavia, não seja como eu quero, mas como tu queres.' },
        { ref: 'João 19:30', text: 'Quando Jesus tomou o vinagre, disse: Está consumado! E, inclinando a cabeça, entregou o espírito.' },
        { ref: 'Mateus 27:51', text: 'E eis que o véu do templo se rasgou em dois, de alto a baixo.' },
      ],
      theologianInsight: {
        theologian: 'John Stott (1921–2011)',
        quote: '"A cruz não é um acidente ou uma derrota. É o evento para o qual toda a história humana converge. Ali, o amor de Deus e a justiça de Deus se encontraram e se abraçaram. Ali, o perdão custou tudo ao perdoador."',
      },
      theologicalTheme: 'Expiação Substitucional · Obediência Filial · Novo Pacto de Sangue · Véu Rasgado',
    },
    {
      id: 20,
      period: 'A Ressurreição e as Aparições',
      timespan: 'Mateus 28 · Lucas 24 · João 20–21 · 1 Coríntios 15',
      icon: 'brightness_7',
      color: '#FFD600',
      summary: 'A ressurreição corporal de Cristo é o evento fundante do cristianismo. Sem ela, não há fé cristã. Com ela, a morte foi derrotada, a justificação confirmada e a nova criação inaugurada.',
      keyFigures: ['Jesus Ressurrecto', 'Maria Madalena', 'Pedro', 'João', 'Tomé', 'Dois de Emaús', 'Paulo'],
      narrative: `A ressurreição não foi uma alucinação coletiva ou uma metáfora espiritual — era a afirmação de um evento histórico, físico, verificável. Paulo elabora o argumento com precisão jurídica em 1 Coríntios 15: Cristo apareceu a Cefas, depois aos Doze, depois a mais de 500 irmãos (a maioria ainda viva quando Paulo escreveu), depois a Tiago, depois a todos os apóstolos, e por último a Paulo mesmo. Este é o relato de um depoimento, não de uma lenda.\n\nO túmulo vazio é o dado histórico mais sólido. Até os inimigos de Jesus não negaram — eles espalharam a história de que o corpo havia sido roubado (Mateus 28:12-15). Mas nenhum adversário jamais produziu o corpo. A transformação dos discípulos — de fugitivos aterrorizados a pregadores que enfrentaram o martírio sem retratar — só tem uma explicação coerente: eles realmente encontraram o Ressurrecto.\n\nA aparição a Maria Madalena (João 20:11-18) é teologicamente rica: a primeira testemunha da ressurreição é uma mulher — num mundo onde o testemunho feminino não era juridicamente aceito. Se a história fosse fabricada, nenhum autor judeu do século I escolheria esta testemunha. O detalhe é a marca de um relato histórico honesto.`,
      keyVerses: [
        { ref: '1 Coríntios 15:17', text: 'E, se Cristo não ressuscitou, é vã a vossa fé; ainda estais nos vossos pecados.' },
        { ref: 'João 20:28', text: 'Respondeu Tomé: Senhor meu e Deus meu!' },
        { ref: 'Romanos 4:25', text: 'O qual foi entregue por causa das nossas transgressões e ressuscitado para nossa justificação.' },
      ],
      theologianInsight: {
        theologian: 'N. T. Wright (1948–)',
        quote: '"Se alguém naquele tempo afirmasse que um morto havia ressuscitado, seria entendido de uma única forma: ele está afirmando que esta pessoa específica voltou corporalmente à vida. Nenhum judeu do século I usaria linguagem de ressurreição para descrever uma experiência espiritual ou visão."',
      },
      theologicalTheme: 'Ressurreição Corporal · Justificação Confirmada · Nova Criação · Evidência Histórica',
    },
    {
      id: 21,
      period: 'Paulo e as Viagens Missionárias',
      timespan: 'Atos 13–28',
      icon: 'explore',
      color: '#1565C0',
      summary: 'Paulo, o maior missionário da história, leva o evangelho do Oriente ao Ocidente em três viagens épicas. Sua estratégia: centros urbanos, sinagogas como porta de entrada, igrejas planteladas em casas. O mundo mediterrâneo nunca mais seria o mesmo.',
      keyFigures: ['Paulo', 'Barnabé', 'Silas', 'Timóteo', 'Lucas', 'Priscila e Áquila', 'Lidia', 'Apolo'],
      narrative: `A conversão de Paulo (Atos 9) é o evento mais transformador da história da Igreja depois de Pentecostes. O maior perseguidor do cristainismo se torna seu maior propagador. Paulo mesmo descreveu isso como a demonstração suprema da graça: "Cristo Jesus veio ao mundo para salvar os pecadores, dos quais eu sou o principal" (1 Timóteo 1:15).\n\nA estratégia missionária de Paulo era sofisticada: ele ia aos centros urbanos (Filipos, Corinto, Éfeso, Roma), visitava a sinagoga primeiro (onde havia judeus e "tementes a Deus" já conhecedores das Escrituras), depois plantava igrejas em casas que funcionavam como células de multiplicação. Em Éfeso, a escola de Tirano tornou-se um centro de treinamento que alcançou toda a Ásia Menor em dois anos (Atos 19:10).\n\nAs cartas de Paulo não são tratados acadêmicos — são respostas pastorais urgentes a crises reais. Gálatas foi escrita numa febre de indignação porque judaizantes estavam adulterando o evangelho da graça. 1 Coríntios responde a divisões, imoralidade sexual, disputas sobre dons espirituais e confusão sobre a ressurreição. Filipenses é uma carta de alegria escrita de uma prisão. As circunstâncias adversas sempre revelavam a teologia mais profunda.`,
      keyVerses: [
        { ref: 'Atos 1:8', text: 'Recebereis poder quando o Espírito Santo vier sobre vós, e sereis minhas testemunhas tanto em Jerusalém... até os confins da terra.' },
        { ref: 'Romanos 10:14', text: 'Como, pois, invocarão aquele em quem não creram? E como crerão naquele de quem não ouviram? E como ouvirão, se não há quem pregue?' },
      ],
      theologianInsight: {
        theologian: 'Roland Allen (1868–1947)',
        quote: '"Paulo nunca fundou uma missão permanente com missionários estacionados. Ele plantou igrejas, treinou líderes locais e seguiu em frente. A estratégia paulina é o antídoto para o paternalismo missionário que mantém as igrejas dependentes de fora."',
      },
      theologicalTheme: 'Missão para os Gentios · Igreja em Casas · Estratégia Urbana · Epístolas Pastorais',
    },
    {
      id: 22,
      period: 'Romanos: A Magna Carta da Fé',
      timespan: 'Romanos 1–16',
      icon: 'gavel',
      color: '#AD1457',
      summary: 'Romanos é a mais sistemática exposição do evangelho na Bíblia. Paulo demonstra que toda a humanidade está perdida (Rm 1-3), a justificação é somente pela fé (Rm 3-5), a santificação é pelo Espírito (Rm 6-8) e a soberania de Deus preserva seu plano (Rm 9-11).',
      keyFigures: ['Paulo', 'A Igreja em Roma', 'Abraão (como exemplo)', 'Adão (como contraste)'],
      narrative: `Romanos é a catedral da teologia paulina. Agostinho foi convertido lendo Romanos 13:13-14; Lutero descobriu a justificação pela fé em Romanos 1:17; Wesley sentiu o coração "estranhamente aquecido" ouvindo o prefácio de Lutero a Romanos. É talvez o livro mais influente da história.\n\nA estrutura é magistral: Romanos 1:18-3:20 é o diagnóstico — tanto gentios quanto judeus estão sob julgamento, "pois todos pecaram e carecem da glória de Deus" (3:23). Romanos 3:21-5:21 é o remédio — a justificação gratuita pela graça mediante a fé no sangue de Cristo. Abraão é o modelo porque creu antes de ser circuncidado, provando que a fé sempre precedeu a Lei.\n\nRomanos 8 é o ápice: "Agora, pois, nenhuma condenação há para os que estão em Cristo Jesus" (8:1). O capítulo move-se da adoção (8:14-17) à certeza (8:28-30) à doxologia triunfante: "Quem nos separará do amor de Cristo?" (8:35). Nenhum sermão, nenhuma tribulação, nenhum poder cósmico pode quebrar a nova aliança. Romanos 9-11 responde à objeção: mas e com Israel? Paulo demonstra que a soberania de Deus não é injustiça — é misericórdia redefinida em termos que a mente humana mal consegue abarcar.`,
      keyVerses: [
        { ref: 'Romanos 1:16', text: 'Não me envergonho do evangelho, porque é o poder de Deus para a salvação de todo aquele que crê.' },
        { ref: 'Romanos 3:23-24', text: 'Porque todos pecaram e carecem da glória de Deus; sendo justificados gratuitamente pela sua graça, pela redenção que há em Cristo Jesus.' },
        { ref: 'Romanos 8:1', text: 'Agora, pois, nenhuma condenação há para os que estão em Cristo Jesus.' },
      ],
      theologianInsight: {
        theologian: 'Martinho Lutero (1483–1546)',
        quote: '"Esta epístola é verdadeiramente a parte principal do NT e o mais puro evangelho. Ela merece ser aprendida de cor por todo cristão, palavra por palavra, e meditada diariamente como o pão da alma."',
      },
      theologicalTheme: 'Justificação · Pecado Universal · Santificação · Soberania Divina',
    },
    {
      id: 23,
      period: 'Hebreus: Cristo Superior a Tudo',
      timespan: 'Hebreus 1–13',
      icon: 'star_outline',
      color: '#4A148C',
      summary: 'Hebreus demonstra que Cristo é superior aos anjos, a Moisés, ao sacerdócio levítico e à própria aliança sinaítica. O sistema do AT era sombra; Cristo é a substância. Sua intercessão eterna é o fundamento da certeza da fé.',
      keyFigures: ['Jesus (Sumo Sacerdote Eterno)', 'Melquisedeque', 'Abraão', 'Moisés', 'Galeria dos Heróis (Hebreus 11)'],
      narrative: `Hebreus foi escrita para cristãos judeus tentados a retornar ao judaísmo para escapar da perseguição. O argumento é: como você pode voltar à sombra quando tem a substância? Cada categoria do sistema levítico — anjos, sacerdotes, sacrifícios, aliança — é demonstrada como inferior ao que Cristo cumpriu.\n\nO capítulo 11, "a galeria dos heróis da fé", é uma das passagens mais inspiradoras da Bíblia: Abraão, Sara, Moisés, Raabe, Gideão, Davi — todos "morreram em fé, sem ter recebido as promessas, mas avistando-as de longe" (11:13). Eles não sabiam o nome de Jesus, mas viveram confiando na promessa de Deus. A definição do capítulo — "a fé é a substância das coisas que se esperam e a certeza de coisas que não se veem" (11:1) — captura a essência da vida cristã.\n\nO capítulo 12 conclui com o corredor de testemunhas: "rodeados, pois, de uma tão grande nuvem de testemunhas" (12:1), fixando os olhos em Jesus, o autor e consumador da fé. O sofrimento dos heróis do capítulo 11 não é negado — é integrado: eles sofreram à vista da promessa. O crente sofre com a promessa cumprida em Cristo, o que torna o sofrimento suportável e até transformador.`,
      keyVerses: [
        { ref: 'Hebreus 1:3', text: 'O qual, sendo o resplendor da sua glória e a imagem expressa da sua pessoa... havendo ele mesmo efetuado a purificação dos pecados, assentou-se à destra da Majestade nas alturas.' },
        { ref: 'Hebreus 11:1', text: 'Ora, a fé é a certeza das coisas que se esperam e a prova das coisas que não se veem.' },
        { ref: 'Hebreus 12:2', text: 'Olhando para Jesus, autor e consumador da fé, o qual, pelo prazer que lhe estava proposto, suportou a cruz.' },
      ],
      theologianInsight: {
        theologian: 'John Owen (1616–1683)',
        quote: '"Hebreus é o livro mais profundo do NT. Se alguém deseja entender a relação entre os dois testamentos, a natureza do sacerdócio de Cristo e os fundamentos da perseverança da fé, deve viver neste livro."',
      },
      theologicalTheme: 'Superioridade de Cristo · Sacerdócio Eterno · Fé como Perseverança · Sombra vs. Substância',
    },
    {
      id: 24,
      period: 'As Cartas de João: O Amor como Marca',
      timespan: '1 João · 2 João · 3 João · João 13–17',
      icon: 'favorite',
      color: '#C62828',
      summary: 'João, o "discípulo que Jesus amava", escreve com urgência pastoral: como saber que você é genuinamente cristão? Os três testes são fé ortodoxa, obediência moral e amor fraternal. Quem ama conhece a Deus.',
      keyFigures: ['João', 'Os Cristãos da Ásia Menor', 'Os Gnósticos', 'A "Eleita Senhora"'],
      narrative: `1 João foi escrita num contexto de crise: gnósticos (docetistas) estavam ensinando que Jesus não veio em carne — era apenas espiritual — e que o conhecimento espiritual (gnosis) libertava do comportamento moral. João responde com três testes de autenticidade da fé: (1) Doutrinário — você confessa que Jesus Cristo veio em carne? (4:2); (2) Moral — você guarda os mandamentos? (2:3-6); (3) Relacional — você ama os irmãos? (4:7-8).\n\nO versículo central é 1 João 4:8: "Deus é amor" (ho theos agapē estin). Mas João é cuidadoso: não diz "amor é Deus" — isso seria idolatria de uma emoção. Deus é a fonte e definição do amor, e o amor foi definido na cruz: "nisto está o amor, não em que nós temos amado a Deus, mas em que ele nos amou a nós, e enviou seu Filho como propiciação pelos nossos pecados" (4:10).\n\nO discurso de despedida de Jesus (João 13-17) — o "coração do quarto Evangelho" — é o texto mais íntimo dos Evangelhos. Jesus lava os pés dos discípulos (serviço como liderança), promete o Espírito Consolador (Paráclito), ora pelo mundo e pela unidade dos crentes (João 17 — a "Grande Oração Sacerdotal"). O novo mandamento: "amem-se uns aos outros como eu os amei" (13:34) — não como os pagãos amam (reciprocidade) ou como a Lei mandava (o próximo), mas como Cristo amou: incondicionalmente.`,
      keyVerses: [
        { ref: '1 João 4:8', text: 'Aquele que não ama não conhece a Deus, porque Deus é amor.' },
        { ref: '1 João 1:9', text: 'Se confessarmos os nossos pecados, ele é fiel e justo para nos perdoar os pecados e nos purificar de toda injustiça.' },
        { ref: 'João 13:34', text: 'Um novo mandamento vos dou: que vos ameis uns aos outros; assim como eu vos amei, que também vós uns aos outros vos ameis.' },
      ],
      theologianInsight: {
        theologian: 'Agostinho de Hipona (354–430)',
        quote: '"Ama e faz o que queres. Se te calar, te calas com amor; se gritas, gritas com amor; se corriges, corriges com amor; se perdoas, perdoas com amor. Se tiveres o amor enraizado em ti, nada senão o bem pode derivar dele."',
      },
      theologicalTheme: 'Deus é Amor · Testes de Autenticidade · Encarnação contra Gnosticismo · Discipulado',
    },
    {
      id: 25,
      period: 'Tiago e Pedro: Fé que Age e Esperança que Sustenta',
      timespan: 'Tiago · 1 Pedro · 2 Pedro · Judas',
      icon: 'anchor',
      color: '#37474F',
      summary: 'Tiago desafia a fé sem obras. Pedro escreve a cristãos dispersos sofrendo perseguição, ancorando a esperança na ressurreição e na glória futura. Estas cartas definem o que é viver como peregrino no mundo.',
      keyFigures: ['Tiago (irmão de Jesus)', 'Pedro', 'Judas (irmão de Tiago)', 'Os Dispersos na Diáspora'],
      narrative: `Tiago foi chamado de "o irmão do Senhor" — cresceu com Jesus e não acreditou nele durante o ministério público (João 7:5). A ressurreição o transformou: tornou-se o líder da Igreja em Jerusalém e escreveu a carta mais prática do NT. "A fé sem obras é morta" (Tiago 2:26) não contradiz Paulo — Paulo fala de obras que GANHAM justificação (impossível); Tiago fala de obras que DEMONSTRAM justificação. A fé genuína inevitavelmente age.\n\n1 Pedro é o manual de vida para o sofrimento cristão. Escrita para "peregrinos e estrangeiros" dispersos pelo Ponto, Galácia, Capadócia, Ásia e Bitínia (1 Pedro 1:1) — cristãos sem pátria. Pedro ancora a esperança na ressurreição: "Bendito o Deus e Pai de nosso Senhor Jesus Cristo, que segundo a sua grande misericórdia nos regenerou para uma esperança viva, pela ressurreição de Jesus Cristo dentre os mortos" (1:3). O sofrimento não é negado — é redefinido como participação no sofrimento de Cristo (4:13) e como refinamento da fé (1:6-7).\n\nA identidade do povo de Deus em 1 Pedro 2:9 — "raça eleita, sacerdócio real, nação santa, povo adquirido" — é extraída do Êxodo (Êxodo 19:6) e aplicada à Igreja: o que Israel era por etnidade e território, os crentes são por graça e vocação. A missão: "para que anuncieis as virtudes daquele que vos chamou das trevas para a sua maravilhosa luz."`,
      keyVerses: [
        { ref: 'Tiago 2:26', text: 'Assim como o corpo sem o espírito está morto, assim também a fé sem obras é morta.' },
        { ref: '1 Pedro 1:3', text: 'Bendito o Deus e Pai de nosso Senhor Jesus Cristo, que segundo a sua grande misericórdia nos regenerou para uma esperança viva.' },
        { ref: '1 Pedro 2:9', text: 'Vós, porém, sois geração eleita, sacerdócio real, nação santa, povo adquirido.' },
      ],
      theologianInsight: {
        theologian: 'Peter Davids (1947–)',
        quote: '"1 Pedro é o guia do exilado: como viver como estrangeiro sem ser irrelevante, como sofrer sem ser esmagado, como manter a identidade cristã numa cultura que não a reconhece. É o livro para a Igreja em tempos pós-cristãos."',
      },
      theologicalTheme: 'Fé que Age · Esperança no Sofrimento · Identidade como Povo de Deus · Peregrinagem',
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
