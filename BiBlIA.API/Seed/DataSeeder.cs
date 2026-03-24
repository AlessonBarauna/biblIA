using BíblIA.Api.Data;
using BíblIA.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedBibleBooksAsync(db);
        await SeedBibleVersesAsync(db);
        await SeedTheologyAsync(db);
        await SeedChurchHistoryAsync(db);
        await SeedEschatologyAsync(db);
    }

    // ── Bible Books ───────────────────────────────────────────────────────────

    private static async Task SeedBibleBooksAsync(AppDbContext db)
    {
        if (await db.BibleBooks.AnyAsync())
            return;

        var books = new List<BibleBook>
        {
            // ── Antigo Testamento ────────────────────────────────────────────────
            new() { OrderIndex =  1, Name = "Genesis",        Abbreviation = "Gen",  Testament = "OT", ChapterCount =  50, Description = "A origem do mundo, da humanidade e do povo de Deus." },
            new() { OrderIndex =  2, Name = "Exodus",         Abbreviation = "Exo",  Testament = "OT", ChapterCount =  40, Description = "A libertação de Israel do Egito e a entrega da Lei no Sinai." },
            new() { OrderIndex =  3, Name = "Leviticus",      Abbreviation = "Lev",  Testament = "OT", ChapterCount =  27, Description = "Leis de santidade, sacrifícios e rituais para o povo de Deus." },
            new() { OrderIndex =  4, Name = "Numbers",        Abbreviation = "Num",  Testament = "OT", ChapterCount =  36, Description = "A jornada de Israel pelo deserto durante 40 anos." },
            new() { OrderIndex =  5, Name = "Deuteronomy",    Abbreviation = "Deu",  Testament = "OT", ChapterCount =  34, Description = "Os discursos finais de Moisés e a renovação da Aliança." },
            new() { OrderIndex =  6, Name = "Joshua",         Abbreviation = "Jos",  Testament = "OT", ChapterCount =  24, Description = "A conquista da Terra Prometida sob a liderança de Josué." },
            new() { OrderIndex =  7, Name = "Judges",         Abbreviation = "Jdg",  Testament = "OT", ChapterCount =  21, Description = "Os ciclos de apostasia, opressão e libertação em Israel." },
            new() { OrderIndex =  8, Name = "Ruth",           Abbreviation = "Rut",  Testament = "OT", ChapterCount =   4, Description = "História de fidelidade e redenção na linhagem davídica." },
            new() { OrderIndex =  9, Name = "1 Samuel",       Abbreviation = "1Sa",  Testament = "OT", ChapterCount =  31, Description = "A ascensão da monarquia israelita: Samuel, Saul e Davi." },
            new() { OrderIndex = 10, Name = "2 Samuel",       Abbreviation = "2Sa",  Testament = "OT", ChapterCount =  24, Description = "O reinado de Davi: glórias, pecados e consequências." },
            new() { OrderIndex = 11, Name = "1 Kings",        Abbreviation = "1Ki",  Testament = "OT", ChapterCount =  22, Description = "Salomão e a divisão do reino em Israel e Judá." },
            new() { OrderIndex = 12, Name = "2 Kings",        Abbreviation = "2Ki",  Testament = "OT", ChapterCount =  25, Description = "O declínio e exílio dos reinos de Israel e Judá." },
            new() { OrderIndex = 13, Name = "1 Chronicles",   Abbreviation = "1Ch",  Testament = "OT", ChapterCount =  29, Description = "Genealogias e o reinado de Davi visto teologicamente." },
            new() { OrderIndex = 14, Name = "2 Chronicles",   Abbreviation = "2Ch",  Testament = "OT", ChapterCount =  36, Description = "Salomão, o templo e os reis de Judá até o exílio." },
            new() { OrderIndex = 15, Name = "Ezra",           Abbreviation = "Ezr",  Testament = "OT", ChapterCount =  10, Description = "O retorno do exílio e a restauração do culto em Jerusalém." },
            new() { OrderIndex = 16, Name = "Nehemiah",       Abbreviation = "Neh",  Testament = "OT", ChapterCount =  13, Description = "Reconstrução dos muros de Jerusalém e reforma religiosa." },
            new() { OrderIndex = 17, Name = "Esther",         Abbreviation = "Est",  Testament = "OT", ChapterCount =  10, Description = "Providência divina na preservação do povo judeu na Pérsia." },
            new() { OrderIndex = 18, Name = "Job",            Abbreviation = "Job",  Testament = "OT", ChapterCount =  42, Description = "O sofrimento do justo e a soberania insondável de Deus." },
            new() { OrderIndex = 19, Name = "Psalms",         Abbreviation = "Psa",  Testament = "OT", ChapterCount = 150, Description = "Coletânea de 150 poemas e hinos de adoração a Deus." },
            new() { OrderIndex = 20, Name = "Proverbs",       Abbreviation = "Pro",  Testament = "OT", ChapterCount =  31, Description = "Máximas de sabedoria prática para uma vida piedosa." },
            new() { OrderIndex = 21, Name = "Ecclesiastes",   Abbreviation = "Ecc",  Testament = "OT", ChapterCount =  12, Description = "Reflexão sobre o sentido da vida sob o sol." },
            new() { OrderIndex = 22, Name = "Song of Solomon",Abbreviation = "SoS",  Testament = "OT", ChapterCount =   8, Description = "Poema de amor celebrando o amor conjugal e a relação de Deus com seu povo." },
            new() { OrderIndex = 23, Name = "Isaiah",         Abbreviation = "Isa",  Testament = "OT", ChapterCount =  66, Description = "Profecias de julgamento e restauração, com o Servo Sofredor." },
            new() { OrderIndex = 24, Name = "Jeremiah",       Abbreviation = "Jer",  Testament = "OT", ChapterCount =  52, Description = "Profecias do julgamento de Judá e a Nova Aliança prometida." },
            new() { OrderIndex = 25, Name = "Lamentations",   Abbreviation = "Lam",  Testament = "OT", ChapterCount =   5, Description = "Elegias sobre a destruição de Jerusalém pela Babilônia." },
            new() { OrderIndex = 26, Name = "Ezekiel",        Abbreviation = "Eze",  Testament = "OT", ChapterCount =  48, Description = "Visões e profecias sobre o exílio e a restauração de Israel." },
            new() { OrderIndex = 27, Name = "Daniel",         Abbreviation = "Dan",  Testament = "OT", ChapterCount =  12, Description = "Fidelidade a Deus no exílio e visões apocalípticas." },
            new() { OrderIndex = 28, Name = "Hosea",          Abbreviation = "Hos",  Testament = "OT", ChapterCount =  14, Description = "O amor fiel de Deus por um povo infiel, ilustrado pelo casamento de Oseias." },
            new() { OrderIndex = 29, Name = "Joel",           Abbreviation = "Joe",  Testament = "OT", ChapterCount =   3, Description = "Chamado ao arrependimento e promessa da efusão do Espírito." },
            new() { OrderIndex = 30, Name = "Amos",           Abbreviation = "Amo",  Testament = "OT", ChapterCount =   9, Description = "Julgamento sobre as nações e a injustiça social em Israel." },
            new() { OrderIndex = 31, Name = "Obadiah",        Abbreviation = "Oba",  Testament = "OT", ChapterCount =   1, Description = "Julgamento de Edom pelo orgulho e traição a Israel." },
            new() { OrderIndex = 32, Name = "Jonah",          Abbreviation = "Jon",  Testament = "OT", ChapterCount =   4, Description = "A misericórdia de Deus além de Israel, mostrada em Nínive." },
            new() { OrderIndex = 33, Name = "Micah",          Abbreviation = "Mic",  Testament = "OT", ChapterCount =   7, Description = "Julgamento e esperança: o Messias nascerá em Belém." },
            new() { OrderIndex = 34, Name = "Nahum",          Abbreviation = "Nah",  Testament = "OT", ChapterCount =   3, Description = "A queda de Nínive e o julgamento do opressor." },
            new() { OrderIndex = 35, Name = "Habakkuk",       Abbreviation = "Hab",  Testament = "OT", ChapterCount =   3, Description = "Diálogo com Deus sobre o mal e confiança soberana." },
            new() { OrderIndex = 36, Name = "Zephaniah",      Abbreviation = "Zep",  Testament = "OT", ChapterCount =   3, Description = "O Dia do Senhor como julgamento e esperança de restauração." },
            new() { OrderIndex = 37, Name = "Haggai",         Abbreviation = "Hag",  Testament = "OT", ChapterCount =   2, Description = "Chamado para reconstruir o templo após o retorno do exílio." },
            new() { OrderIndex = 38, Name = "Zechariah",      Abbreviation = "Zec",  Testament = "OT", ChapterCount =  14, Description = "Visões e profecias messiânicas sobre o reino de Deus." },
            new() { OrderIndex = 39, Name = "Malachi",        Abbreviation = "Mal",  Testament = "OT", ChapterCount =   4, Description = "Último profeta do AT: chamado à fidelidade e promessa de Elias." },

            // ── Novo Testamento ──────────────────────────────────────────────────
            new() { OrderIndex = 40, Name = "Matthew",        Abbreviation = "Mat",  Testament = "NT", ChapterCount =  28, Description = "Jesus como Messias davídico e cumprimento da Lei e dos Profetas." },
            new() { OrderIndex = 41, Name = "Mark",           Abbreviation = "Mar",  Testament = "NT", ChapterCount =  16, Description = "Evangelho dinâmico e direto: Jesus como Servo sofredor." },
            new() { OrderIndex = 42, Name = "Luke",           Abbreviation = "Luk",  Testament = "NT", ChapterCount =  24, Description = "Jesus como Salvador universal, com ênfase nos marginalizados." },
            new() { OrderIndex = 43, Name = "John",           Abbreviation = "Joh",  Testament = "NT", ChapterCount =  21, Description = "Jesus como Filho de Deus encarnado — o Logos." },
            new() { OrderIndex = 44, Name = "Acts",           Abbreviation = "Act",  Testament = "NT", ChapterCount =  28, Description = "A expansão da Igreja pelo poder do Espírito Santo." },
            new() { OrderIndex = 45, Name = "Romans",         Abbreviation = "Rom",  Testament = "NT", ChapterCount =  16, Description = "A justiça de Deus e a salvação pela fé em Cristo." },
            new() { OrderIndex = 46, Name = "1 Corinthians",  Abbreviation = "1Co",  Testament = "NT", ChapterCount =  16, Description = "Instrução sobre unidade, dons espirituais e a ressurreição." },
            new() { OrderIndex = 47, Name = "2 Corinthians",  Abbreviation = "2Co",  Testament = "NT", ChapterCount =  13, Description = "O ministério apostólico, o sofrimento e a reconciliação." },
            new() { OrderIndex = 48, Name = "Galatians",      Abbreviation = "Gal",  Testament = "NT", ChapterCount =   6, Description = "Liberdade em Cristo: o evangelho da graça contra o legalismo." },
            new() { OrderIndex = 49, Name = "Ephesians",      Abbreviation = "Eph",  Testament = "NT", ChapterCount =   6, Description = "A Igreja como corpo de Cristo e a armadura espiritual." },
            new() { OrderIndex = 50, Name = "Philippians",    Abbreviation = "Php",  Testament = "NT", ChapterCount =   4, Description = "Carta de alegria: contentamento e humildade à semelhança de Cristo." },
            new() { OrderIndex = 51, Name = "Colossians",     Abbreviation = "Col",  Testament = "NT", ChapterCount =   4, Description = "A supremacia de Cristo sobre toda criação e heresia." },
            new() { OrderIndex = 52, Name = "1 Thessalonians",Abbreviation = "1Th",  Testament = "NT", ChapterCount =   5, Description = "Encorajamento e instrução sobre a vinda de Cristo." },
            new() { OrderIndex = 53, Name = "2 Thessalonians",Abbreviation = "2Th",  Testament = "NT", ChapterCount =   3, Description = "Correção sobre eventos escatológicos e chamado à perseverança." },
            new() { OrderIndex = 54, Name = "1 Timothy",      Abbreviation = "1Ti",  Testament = "NT", ChapterCount =   6, Description = "Instrução pastoral sobre liderança, doutrina e conduta na Igreja." },
            new() { OrderIndex = 55, Name = "2 Timothy",      Abbreviation = "2Ti",  Testament = "NT", ChapterCount =   4, Description = "Última carta de Paulo: perseverar no evangelho até o fim." },
            new() { OrderIndex = 56, Name = "Titus",          Abbreviation = "Tit",  Testament = "NT", ChapterCount =   3, Description = "Organização eclesial e sã doutrina na ilha de Creta." },
            new() { OrderIndex = 57, Name = "Philemon",       Abbreviation = "Phm",  Testament = "NT", ChapterCount =   1, Description = "Apelo pela reconciliação com Onésimo como irmão em Cristo." },
            new() { OrderIndex = 58, Name = "Hebrews",        Abbreviation = "Heb",  Testament = "NT", ChapterCount =  13, Description = "A superioridade de Cristo sobre os anjos, Moisés e o sacerdócio levítico." },
            new() { OrderIndex = 59, Name = "James",          Abbreviation = "Jam",  Testament = "NT", ChapterCount =   5, Description = "Fé viva que se manifesta em obras e sabedoria prática." },
            new() { OrderIndex = 60, Name = "1 Peter",        Abbreviation = "1Pe",  Testament = "NT", ChapterCount =   5, Description = "Esperança e perseverança em meio ao sofrimento e perseguição." },
            new() { OrderIndex = 61, Name = "2 Peter",        Abbreviation = "2Pe",  Testament = "NT", ChapterCount =   3, Description = "Alerta contra falsos mestres e confirmação da Palavra profética." },
            new() { OrderIndex = 62, Name = "1 John",         Abbreviation = "1Jo",  Testament = "NT", ChapterCount =   5, Description = "Comunhão com Deus: amor, luz e certeza da vida eterna." },
            new() { OrderIndex = 63, Name = "2 John",         Abbreviation = "2Jo",  Testament = "NT", ChapterCount =   1, Description = "Alerta contra falsos mestres e chamado ao amor." },
            new() { OrderIndex = 64, Name = "3 John",         Abbreviation = "3Jo",  Testament = "NT", ChapterCount =   1, Description = "Hospitalidade cristã e integridade no ministério." },
            new() { OrderIndex = 65, Name = "Jude",           Abbreviation = "Jud",  Testament = "NT", ChapterCount =   1, Description = "Contender pela fé apostólica contra o engano e apostasia." },
            new() { OrderIndex = 66, Name = "Revelation",     Abbreviation = "Rev",  Testament = "NT", ChapterCount =  22, Description = "A vitória final de Cristo e o cumprimento de toda a história redentora." },
        };

        await db.BibleBooks.AddRangeAsync(books);
        await db.SaveChangesAsync();
    }

    // ── Bible Verses (sample chapters) ───────────────────────────────────────
    // Semeia versículos de passagens icônicas para demonstrar a navegação.
    // O livro é referenciado pelo OrderIndex para não depender do Id gerado.

    private static async Task SeedBibleVersesAsync(AppDbContext db)
    {
        if (await db.BibleVerses.AnyAsync())
            return;

        // Busca livros por OrderIndex para obter os Ids corretos
        var genesis  = await db.BibleBooks.FirstAsync(b => b.OrderIndex == 1);
        var psalms   = await db.BibleBooks.FirstAsync(b => b.OrderIndex == 19);
        var john     = await db.BibleBooks.FirstAsync(b => b.OrderIndex == 43);
        var romans   = await db.BibleBooks.FirstAsync(b => b.OrderIndex == 45);

        var verses = new List<BibleVerse>();

        // Gênesis 1:1-5 — A criação
        verses.AddRange(new[]
        {
            new BibleVerse { BookId = genesis.Id, Chapter = 1, Verse = 1, TextACF = "No princípio, criou Deus os céus e a terra.", TextKJV = "In the beginning God created the heaven and the earth." },
            new BibleVerse { BookId = genesis.Id, Chapter = 1, Verse = 2, TextACF = "A terra, porém, estava sem forma e vazia; havia trevas sobre a face do abismo, e o Espírito de Deus se movia sobre a face das águas.", TextKJV = "And the earth was without form, and void; and darkness was upon the face of the deep. And the Spirit of God moved upon the face of the waters." },
            new BibleVerse { BookId = genesis.Id, Chapter = 1, Verse = 3, TextACF = "Disse Deus: Haja luz; e houve luz.", TextKJV = "And God said, Let there be light: and there was light." },
            new BibleVerse { BookId = genesis.Id, Chapter = 1, Verse = 4, TextACF = "Viu Deus que a luz era boa; e fez separação entre a luz e as trevas.", TextKJV = "And God saw the light, that it was good: and God divided the light from the darkness." },
            new BibleVerse { BookId = genesis.Id, Chapter = 1, Verse = 5, TextACF = "E Deus chamou à luz Dia, e às trevas chamou Noite. Houve tarde e manhã, o primeiro dia.", TextKJV = "And God called the light Day, and the darkness he called Night. And the evening and the morning were the first day." },
        });

        // Salmos 23:1-6 — O bom pastor
        verses.AddRange(new[]
        {
            new BibleVerse { BookId = psalms.Id, Chapter = 23, Verse = 1, TextACF = "O Senhor é o meu pastor; nada me faltará.", TextKJV = "The LORD is my shepherd; I shall not want." },
            new BibleVerse { BookId = psalms.Id, Chapter = 23, Verse = 2, TextACF = "Ele me faz repousar em pastos verdejantes. Leva-me às águas tranquilas.", TextKJV = "He maketh me to lie down in green pastures: he leadeth me beside the still waters." },
            new BibleVerse { BookId = psalms.Id, Chapter = 23, Verse = 3, TextACF = "Refrigera a minha alma, guia-me pelas veredas da justiça por amor do seu nome.", TextKJV = "He restoreth my soul: he leadeth me in the paths of righteousness for his name's sake." },
            new BibleVerse { BookId = psalms.Id, Chapter = 23, Verse = 4, TextACF = "Ainda que eu ande pelo vale da sombra da morte, não temerei mal nenhum, porque tu estás comigo; o teu cajado e o teu bordão me consolam.", TextKJV = "Yea, though I walk through the valley of the shadow of death, I will fear no evil: for thou art with me; thy rod and thy staff they comfort me." },
            new BibleVerse { BookId = psalms.Id, Chapter = 23, Verse = 5, TextACF = "Preparas uma mesa perante mim na presença dos meus adversários, unges a minha cabeça com óleo; o meu cálice transborda.", TextKJV = "Thou preparest a table before me in the presence of mine enemies: thou anointest my head with oil; my cup runneth over." },
            new BibleVerse { BookId = psalms.Id, Chapter = 23, Verse = 6, TextACF = "Bondade e misericórdia certamente me seguirão todos os dias da minha vida, e habitarei na casa do Senhor por longos dias.", TextKJV = "Surely goodness and mercy shall follow me all the days of my life: and I will dwell in the house of the LORD for ever." },
        });

        // João 3:1-21 — O novo nascimento e João 3:16
        verses.AddRange(new[]
        {
            new BibleVerse { BookId = john.Id, Chapter = 3, Verse = 1,  TextACF = "Havia entre os fariseus um homem chamado Nicodemos, um dos principais dos judeus.", TextKJV = "There was a man of the Pharisees, named Nicodemus, a ruler of the Jews:" },
            new BibleVerse { BookId = john.Id, Chapter = 3, Verse = 2,  TextACF = "Este foi ter com Jesus de noite e disse-lhe: Rabi, sabemos que és Mestre vindo da parte de Deus; porque ninguém pode fazer estes sinais que tu fazes, se Deus não for com ele.", TextKJV = "The same came to Jesus by night, and said unto him, Rabbi, we know that thou art a teacher come from God: for no man can do these miracles that thou doest, except God be with him." },
            new BibleVerse { BookId = john.Id, Chapter = 3, Verse = 3,  TextACF = "Jesus respondeu e disse-lhe: Em verdade, em verdade te digo que aquele que não nascer de novo não pode ver o reino de Deus.", TextKJV = "Jesus answered and said unto him, Verily, verily, I say unto thee, Except a man be born again, he cannot see the kingdom of God." },
            new BibleVerse { BookId = john.Id, Chapter = 3, Verse = 4,  TextACF = "Nicodemos lhe perguntou: Como pode um homem nascer sendo já velho? Pode tornar a entrar no ventre de sua mãe e nascer?", TextKJV = "Nicodemus saith unto him, How can a man be born when he is old? can he enter the second time into his mother's womb, and be born?" },
            new BibleVerse { BookId = john.Id, Chapter = 3, Verse = 5,  TextACF = "Jesus respondeu: Em verdade, em verdade te digo que aquele que não nascer da água e do Espírito não pode entrar no reino de Deus.", TextKJV = "Jesus answered, Verily, verily, I say unto thee, Except a man be born of water and of the Spirit, he cannot enter into the kingdom of God." },
            new BibleVerse { BookId = john.Id, Chapter = 3, Verse = 16, TextACF = "Porque Deus amou o mundo de tal maneira que deu o seu Filho unigênito, para que todo aquele que nele crê não pereça, mas tenha a vida eterna.", TextKJV = "For God so loved the world, that he gave his only begotten Son, that whosoever believeth in him should not perish, but have everlasting life." },
            new BibleVerse { BookId = john.Id, Chapter = 3, Verse = 17, TextACF = "Porque Deus enviou o seu Filho ao mundo, não para que condenasse o mundo, mas para que o mundo fosse salvo por ele.", TextKJV = "For God sent not his Son into the world to condemn the world; but that the world through him might be saved." },
        });

        // Romanos 8:28-39 — Mais que vencedores
        verses.AddRange(new[]
        {
            new BibleVerse { BookId = romans.Id, Chapter = 8, Verse = 28, TextACF = "Sabemos que todas as coisas cooperam para o bem daqueles que amam a Deus, daqueles que são chamados segundo o seu propósito.", TextKJV = "And we know that all things work together for good to them that love God, to them who are the called according to his purpose." },
            new BibleVerse { BookId = romans.Id, Chapter = 8, Verse = 29, TextACF = "Porque os que dantes conheceu, também os predestinou para serem conformes à imagem de seu Filho, a fim de que ele seja o primogênito entre muitos irmãos.", TextKJV = "For whom he did foreknow, he also did predestinate to be conformed to the image of his Son, that he might be the firstborn among many brethren." },
            new BibleVerse { BookId = romans.Id, Chapter = 8, Verse = 31, TextACF = "Que diremos, pois, a estas coisas? Se Deus é por nós, quem será contra nós?", TextKJV = "What shall we then say to these things? If God be for us, who can be against us?" },
            new BibleVerse { BookId = romans.Id, Chapter = 8, Verse = 37, TextACF = "Mas em todas estas coisas somos mais que vencedores por aquele que nos amou.", TextKJV = "Nay, in all these things we are more than conquerors through him that loved us." },
            new BibleVerse { BookId = romans.Id, Chapter = 8, Verse = 38, TextACF = "Porque eu estou persuadido de que nem a morte, nem a vida, nem os anjos, nem os principados, nem as potestades, nem o presente, nem o porvir,", TextKJV = "For I am persuaded, that neither death, nor life, nor angels, nor principalities, nor powers, nor things present, nor things to come," },
            new BibleVerse { BookId = romans.Id, Chapter = 8, Verse = 39, TextACF = "Nem a altura, nem a profundidade, nem alguma outra criatura nos poderá separar do amor de Deus, que está em Cristo Jesus, nosso Senhor.", TextKJV = "Nor height, nor depth, nor any other creature, shall be able to separate us from the love of God, which is in Christ Jesus our Lord." },
        });

        await db.BibleVerses.AddRangeAsync(verses);
        await db.SaveChangesAsync();
    }

    // ── Theology ──────────────────────────────────────────────────────────────

    private static async Task SeedTheologyAsync(AppDbContext db)
    {
        if (await db.TheologyCourses.AnyAsync())
            return;

        // Curso 1: Introdução à Teologia Sistemática
        var courseSystematic = new TheologyCourse
        {
            Title = "Introdução à Teologia Sistemática",
            Description = "Uma visão geral das doutrinas fundamentais da fé cristã, organizadas de forma sistemática.",
            Category = "Teologia Sistemática",
            DurationHours = 20,
            Level = "Básico",
            ImageIcon = "school",
            Modules = new List<TheologyModule>
            {
                new()
                {
                    OrderIndex = 1,
                    Title = "O que é Teologia?",
                    Content = "Teologia é literalmente o 'estudo de Deus' (do grego theos = Deus, logos = estudo). A teologia sistemática organiza as verdades bíblicas em categorias coerentes: Bibliologia (Escrituras), Teologia Própria (Deus), Cristologia (Cristo), Pneumatologia (Espírito Santo), Soteriologia (salvação), Eclesiologia (Igreja) e Escatologia (eventos finais). Diferente da teologia bíblica, que segue o fluxo histórico-canônico, a teologia sistemática busca responder: 'O que a Bíblia inteira ensina sobre X?'",
                    References = "Teologia Sistemática — Wayne Grudem; Teologia Sistemática — Louis Berkhof; A Sã Doutrina — J. I. Packer",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "O que significa o termo 'teologia' etimologicamente?",
                            OptionA = "Estudo da Bíblia",
                            OptionB = "Estudo de Deus",
                            OptionC = "Estudo da Igreja",
                            OptionD = "Estudo da salvação",
                            CorrectAnswer = "B",
                            Explanation = "Teologia vem do grego 'theos' (Deus) + 'logos' (estudo/palavra), significando literalmente o estudo de Deus e das coisas divinas."
                        },
                        new()
                        {
                            Question = "Qual ramo da teologia sistemática estuda a doutrina da salvação?",
                            OptionA = "Cristologia",
                            OptionB = "Eclesiologia",
                            OptionC = "Soteriologia",
                            OptionD = "Pneumatologia",
                            CorrectAnswer = "C",
                            Explanation = "Soteriologia vem do grego 'soteria' (salvação) e estuda a obra redentora de Deus, incluindo eleição, justificação, santificação e glorificação."
                        }
                    }
                },
                new()
                {
                    OrderIndex = 2,
                    Title = "A Doutrina das Escrituras (Bibliologia)",
                    Content = "A Bibliologia estuda a natureza e a autoridade da Bíblia. Os conceitos centrais são: (1) Revelação — Deus se comunica com a humanidade (geral: criação; especial: Escritura e Cristo); (2) Inspiração — as Escrituras são divinamente inspiradas (2 Tm 3:16), produzidas por homens movidos pelo Espírito Santo sem anular suas personalidades; (3) Inerrância — a Bíblia é sem erros em tudo que afirma originalmente; (4) Cânon — os 66 livros reconhecidos como autoritativos; (5) Suficiência — a Bíblia contém tudo necessário para salvação e vida piedosa.",
                    References = "2 Timóteo 3:16-17; 2 Pedro 1:20-21; The Scripture Principle — Clark Pinnock; Scripture and Truth — D. A. Carson",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual versículo é o texto fundamental sobre a inspiração das Escrituras?",
                            OptionA = "João 1:1",
                            OptionB = "2 Timóteo 3:16",
                            OptionC = "Romanos 3:23",
                            OptionD = "Hebreus 11:1",
                            CorrectAnswer = "B",
                            Explanation = "2 Timóteo 3:16 diz: 'Toda a Escritura é divinamente inspirada' (theopneustos — soprada por Deus), sendo o locus classicus da doutrina da inspiração."
                        }
                    }
                },
                new()
                {
                    OrderIndex = 3,
                    Title = "A Doutrina de Deus (Teologia Própria)",
                    Content = "Esta doutrina estuda os atributos divinos e a Trindade. Os atributos de Deus se dividem em: (1) Incomunicáveis — que Deus não compartilha com criaturas: aseidade (existência própria), imutabilidade, impassibilidade, eternidade, onipresença, onisciência, onipotência, soberania; (2) Comunicáveis — que refletem em criaturas: amor, santidade, justiça, bondade, verdade. A Trindade ensina que há um único Deus em três Pessoas — Pai, Filho e Espírito Santo — coeternos, coiguais em essência, distintos em pessoas e relacionamento.",
                    References = "Deuteronômio 6:4; Mateus 28:19; Knowing God — J. I. Packer; The Attributes of God — A. W. Pink",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual atributo divino se refere à existência de Deus a partir de si mesmo, sem causa externa?",
                            OptionA = "Onipotência",
                            OptionB = "Imutabilidade",
                            OptionC = "Aseidade",
                            OptionD = "Onipresença",
                            CorrectAnswer = "C",
                            Explanation = "Aseidade (do latim 'a se' — de si mesmo) é o atributo pelo qual Deus existe por si mesmo, independente de qualquer outra causa. Ele é o ser necessário, o 'EU SOU' (Êx 3:14)."
                        }
                    }
                }
            }
        };

        // Curso 2: Cristologia
        var courseChristology = new TheologyCourse
        {
            Title = "Cristologia: A Pessoa e a Obra de Cristo",
            Description = "Estudo aprofundado sobre quem Jesus é (sua pessoa) e o que ele fez (sua obra redentora).",
            Category = "Cristologia",
            DurationHours = 15,
            Level = "Intermediário",
            ImageIcon = "cross",
            Modules = new List<TheologyModule>
            {
                new()
                {
                    OrderIndex = 1,
                    Title = "A Divindade de Cristo",
                    Content = "O NT afirma explicitamente a divindade de Jesus: (1) Títulos divinos: 'Senhor' (Kyrios — nome divino do AT), 'Filho de Deus', 'EU SOU' (Jo 8:58); (2) Atributos divinos: onisciência (Jo 21:17), onipotência (Mt 28:18), eternidade (Jo 1:1; 8:58); (3) Obras divinas: criação (Cl 1:16), perdão de pecados (Mc 2:5-7), ressurreição dos mortos (Jo 5:28-29); (4) Adoração recebida (Mt 28:17; Ap 5:12-13). Heresias históricas: arianismo (Jesus como criatura suprema), docetismo (Jesus apenas parecia humano), modalismo (Pai=Filho=Espírito em sequência).",
                    References = "João 1:1-14; Colossenses 1:15-20; Filipenses 2:5-11; Hebreus 1:1-4",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual heresia ensinava que Jesus era uma criatura, o primeiro e mais perfeito ser criado por Deus?",
                            OptionA = "Docetismo",
                            OptionB = "Modalismo",
                            OptionC = "Arianismo",
                            OptionD = "Nestorianismo",
                            CorrectAnswer = "C",
                            Explanation = "O arianismo, formulado por Ário de Alexandria (séc. IV), ensinava que o Filho foi criado pelo Pai — 'houve um tempo quando o Filho não era'. Foi condenado no Concílio de Niceia (325 d.C.)."
                        }
                    }
                },
                new()
                {
                    OrderIndex = 2,
                    Title = "A Expiação: Teorias e Significado",
                    Content = "A expiação é a obra de Cristo na cruz para reconciliar pecadores com Deus. As principais teorias são: (1) Substituição Penal (Reformada) — Cristo sofreu a penalidade devida aos pecadores, satisfazendo a justiça divina (Is 53:4-6; 2 Co 5:21); (2) Teoria da Satisfação (Anselmo) — Cristo satisfez a honra de Deus ofendida pelo pecado; (3) Teoria Governamental (Grotius) — Deus como governador perdoa, mas mantém a ordem moral; (4) Teoria do Exemplo Moral (Aberlado) — a cruz motiva mudança por demonstrar o amor divino; (5) Christus Victor (Aulen) — Cristo derrota o diabo, o pecado e a morte. A Reforma enfatizou a substituição penal como núcleo bíblico.",
                    References = "Isaías 53:4-6; Romanos 3:25-26; 2 Coríntios 5:21; Gálatas 3:13",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual teoria da expiação ensina que Cristo pagou a penalidade devida pelos pecadores em seu lugar?",
                            OptionA = "Teoria do Exemplo Moral",
                            OptionB = "Substituição Penal",
                            OptionC = "Christus Victor",
                            OptionD = "Teoria Governamental",
                            CorrectAnswer = "B",
                            Explanation = "A Substituição Penal, central na teologia reformada, ensina que Cristo foi nosso substituto penal: recebeu sobre si a ira justa de Deus que nossos pecados mereciam (Is 53:5; 2 Co 5:21; Rm 3:25-26)."
                        }
                    }
                }
            }
        };

        // Curso 3: Hermenêutica Bíblica
        var courseHermeneutics = new TheologyCourse
        {
            Title = "Hermenêutica: Como Interpretar a Bíblia",
            Description = "Princípios e métodos para uma interpretação sólida e responsável das Escrituras.",
            Category = "Hermenêutica",
            DurationHours = 12,
            Level = "Básico",
            ImageIcon = "book",
            Modules = new List<TheologyModule>
            {
                new()
                {
                    OrderIndex = 1,
                    Title = "Princípios Fundamentais de Hermenêutica",
                    Content = "Hermenêutica é a ciência e arte da interpretação textual. Para a Bíblia, os princípios básicos são: (1) Sentido Literal — busque o sentido natural do texto, não alegórico por padrão; (2) Contexto — o texto sem contexto é pretexto; leia o versículo no parágrafo, capítulo, livro, Testamento e Bíblia inteira; (3) Gênero Literário — lei, narrativa, poesia, profecia, epistola, apocalipse têm regras distintas; (4) Contexto Histórico-Cultural — entenda a audiência original; (5) Analogia da Fé — a Escritura interpreta a Escritura; passagens claras iluminam as obscuras; (6) Gramática — o significado das palavras no idioma original (hebraico/grego) é fundamental.",
                    References = "Neemias 8:8; 2 Timóteo 2:15; Fee & Stuart - Como Ler a Bíblia com Eficiência; Walter Kaiser - Exegese do Antigo Testamento",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual princípio hermenêutico ensina que passagens claras da Escritura devem iluminar as passagens obscuras?",
                            OptionA = "Princípio do Contexto",
                            OptionB = "Princípio Literal",
                            OptionC = "Analogia da Fé",
                            OptionD = "Princípio Histórico-Cultural",
                            CorrectAnswer = "C",
                            Explanation = "A Analogia da Fé (analogia fidei) estabelece que a Escritura é seu próprio intérprete. Textos obscuros ou menos claros devem ser interpretados à luz de textos mais claros que tratam do mesmo assunto."
                        }
                    }
                }
            }
        };

        await db.TheologyCourses.AddRangeAsync(courseSystematic, courseChristology, courseHermeneutics);
        await db.SaveChangesAsync();
    }

    // ── Church History ────────────────────────────────────────────────────────

    private static async Task SeedChurchHistoryAsync(AppDbContext db)
    {
        if (await db.ChurchHeroes.AnyAsync())
            return;

        var heroes = new List<ChurchHero>
        {
            new()
            {
                Name = "Martinho Lutero",
                Period = "Reforma Protestante",
                BirthYear = 1483,
                DeathYear = 1546,
                Nationality = "Alemão",
                Category = "Reformer",
                Biography = "Monge agostiniano e professor de teologia em Wittenberg, Lutero iniciou a Reforma Protestante ao pregar as 95 Teses em 31 de outubro de 1517, contestando as indulgências papais. Sua descoberta da justificação pela fé somente (sola fide), a partir de Romanos 1:17, transformou o mundo ocidental. Excomunhado pelo papa Leão X em 1521 e declarado herege na Dieta de Worms, ele se recusou a se retratar: 'Aqui estou, não posso fazer outra coisa.' Traduziu a Bíblia para o alemão popular, democratizando o acesso às Escrituras.",
                KeyContributions = "Sola Fide, Sola Scriptura, Sola Gratia; tradução da Bíblia para o alemão; catecismos Menor e Maior; reforma litúrgica e educação universal.",
                FavoriteVerse = "Romanos 1:17 — 'O justo viverá pela fé.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "João Calvino",
                Period = "Reforma Protestante",
                BirthYear = 1509,
                DeathYear = 1564,
                Nationality = "Francês",
                Category = "Reformer",
                Biography = "Teólogo humanista convertido ao protestantismo por volta de 1533, Calvino tornou-se o arquiteto intelectual da segunda geração da Reforma. Instalado em Genebra, transformou a cidade numa república teocrática modelo. Sua obra 'Institutas da Religião Cristã' (1536, expandida até 1559) é a sistematização teológica mais completa da Reforma. Desenvolveu a teologia da soberania de Deus, a predestinação dupla e o pacto de graça. Seu comentário cobre quase todo o AT e NT.",
                KeyContributions = "Teologia da soberania de Deus; doutrina da predestinação; governo presbiteriano; ética protestante do trabalho; sistema educacional em Genebra; influência no puritanismo e presbiterianismo mundial.",
                FavoriteVerse = "Efésios 1:4-5 — 'Segundo nos elegeu nele antes da fundação do mundo.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "João Wesley",
                Period = "Avivamento do Séc. XVIII",
                BirthYear = 1703,
                DeathYear = 1791,
                Nationality = "Inglês",
                Category = "Revivalist",
                Biography = "Fundador do metodismo, Wesley pregou ao ar livre quando as igrejas anglicanas lhe fecharam as portas. Junto com George Whitefield, liderou o Grande Avivamento que transformou a Inglaterra e evitou uma revolução social. Em 1738, num culto na Aldersgate Street, sentiu seu coração 'estranhamente aquecido' ao ouvir a leitura do prefácio de Lutero a Romanos — sua conversão evangélica. Percorreu aproximadamente 400.000 km a cavalo pregando o evangelho, realizando cerca de 40.000 sermões.",
                KeyContributions = "Fundação do metodismo; teologia arminiana evangelizada; ênfase na santificação e perfeição cristã; trabalho social entre os pobres; organização em classes e sociedades; evangelismo ao ar livre.",
                FavoriteVerse = "Romanos 8:1 — 'Portanto, agora nenhuma condenação há para os que estão em Cristo Jesus.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "Charles Spurgeon",
                Period = "Séc. XIX — Era Vitoriana",
                BirthYear = 1834,
                DeathYear = 1892,
                Nationality = "Inglês",
                Category = "Evangelist",
                Biography = "Convertido aos 15 anos ao ouvir um pregador metodista citar Isaías 45:22, Spurgeon tornou-se pastor batista aos 20 anos e o pregador mais popular da era vitoriana. Sua congregação no Metropolitan Tabernacle em Londres chegava a 10.000 pessoas. Publicou mais de 3.500 sermões e 140 livros. Sofreu de depressão severa mas escreveu profundamente sobre a graça de Deus no sofrimento. É chamado de 'O Príncipe dos Pregadores'.",
                KeyContributions = "Metropolitan Tabernacle; Orphanage Stockwell; Pastor's College; 63 volumes de sermões; Tesouro de Davi (comentário dos Salmos); defesa do calvinismo na 'Controvérsia Declinante'.",
                FavoriteVerse = "Isaías 45:22 — 'Olha para mim e sê salvo, todos os confins da terra.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "Hudson Taylor",
                Period = "Séc. XIX — Missões Modernas",
                BirthYear = 1832,
                DeathYear = 1905,
                Nationality = "Inglês",
                Category = "Missionary",
                Biography = "Fundador da Missão ao Interior da China (CIM), Taylor dedicou 51 anos à evangelização da China. Pioneiro em contextualização missionária: adotou roupas, penteado e costumes chineses quando todos os missionários ocidentais mantinham seus trajes europeus. Fundou a CIM em 1865, recrutando 800 missionários durante sua vida. Responsável pela evangelização de milhões de chineses. Sua famosa citação: 'Deus não precisa de homens grandes, mas de homens dependentes de um Deus grande.'",
                KeyContributions = "Fundação da CIM (hoje OMF International); evangelização do interior da China; contextualização missionária; dependência de fé para sustento (princípio Hudson Taylor); abertura de províncias chinesas ao evangelho.",
                FavoriteVerse = "Mateus 9:37-38 — 'A messe é grande, mas os ceifeiros são poucos.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "Policarpo de Esmirna",
                Period = "Igreja Primitiva",
                BirthYear = 69,
                DeathYear = 155,
                Nationality = "Greco-Romano (Turquia atual)",
                Category = "Martyr",
                Biography = "Discípulo direto do apóstolo João e bispo de Esmirna, Policarpo é um dos pais apostólicos mais importantes. Aos 86 anos, foi preso e levado ao anfiteatro para ser queimado vivo por se recusar a renunciar a Cristo. O procônsul lhe disse: 'Renega Cristo e te pouparei.' Policarpo respondeu: 'Oitenta e seis anos o sirvo e ele nunca me fez nenhum mal. Como poderia blasfemar contra meu Rei que me salvou?' Sua morte é narrada no 'Martírio de Policarpo', um dos documentos cristãos mais antigos.",
                KeyContributions = "Elo vital entre os apóstolos e a Igreja Patrística; defesa da cristologia ortodoxa contra o gnosticismo; modelo de martírio fiel; influência sobre Ireneu de Lyon.",
                FavoriteVerse = "Filipenses 4:13 — 'Tudo posso naquele que me fortalece.'",
                ImageUrl = ""
            }
        };

        var revivals = new List<Revival>
        {
            new()
            {
                Name = "Grande Avivamento na América do Norte",
                Year = 1734,
                EndYear = 1743,
                Location = "Nova Inglaterra, EUA",
                LeaderNames = "Jonathan Edwards, George Whitefield",
                Description = "O Primeiro Grande Avivamento transformou as colônias americanas. Começou em Northampton, Massachusetts, através da pregação de Jonathan Edwards. George Whitefield percorreu as colônias pregando ao ar livre para multidões de até 20.000 pessoas. Caracterizado por conversões dramáticas, choro, tremores físicos e profunda convicção de pecado. Edwards escreveu 'A Narrativa Fiel' documentando o avivamento.",
                KeyEvents = "1734: Início em Northampton através de Edwards; 1740: Chegada de Whitefield às colônias; pregação de 'Pecadores nas Mãos de um Deus Irado' (1741); formação de igrejas congregacionais e presbiterianas; fundação do Princeton College.",
                Impact = "Transformação espiritual de dezenas de milhares; unificação colonial antes da Revolução Americana; crescimento do protestantismo evangélico; ênfase na experiência pessoal de conversão.",
                EstimatedConversions = "Dezenas de milhares nas 13 colônias"
            },
            new()
            {
                Name = "Avivamento de Gales",
                Year = 1904,
                EndYear = 1905,
                Location = "País de Gales, Reino Unido",
                LeaderNames = "Evan Roberts",
                Description = "Um dos avivamentos mais extraordinários do século XX. Evan Roberts, jovem mineiro de 26 anos, foi o instrumento central. Em seis meses, aproximadamente 100.000 pessoas foram convertidas no País de Gales. Os cultos duravam a noite toda, sem pregação formal — o Espírito Santo movia as pessoas a confessarem pecados, cantarem e orarem espontaneamente. As tabernas fecharam por falta de clientes, o crime diminuiu drasticamente e até os cavalos das minas não entendiam mais os comandos de blasfêmia dos mineiros convertidos.",
                KeyEvents = "Setembro 1904: Início com Evan Roberts em Loughor; cultos espontâneos com confissão pública; canto espontâneo sem liderança; colapso do crime e alcoolismo; relatórios em jornais mundiais.",
                Impact = "100.000 conversões em 5 meses; transformação social no País de Gales; influência no Avivamento Azusa Street (1906); disseminação mundial do pentecostalismo.",
                EstimatedConversions = "100.000 em 5 meses (País de Gales)"
            },
            new()
            {
                Name = "Avivamento da Rua Azusa",
                Year = 1906,
                EndYear = 1909,
                Location = "Los Angeles, Califórnia, EUA",
                LeaderNames = "William J. Seymour",
                Description = "Considerado o ponto de partida do movimento pentecostal moderno. William Seymour, filho de escravos, pregou em um antigo estábulo na Rua Azusa. O avivamento foi marcado pelo falar em línguas (glossolalia), curas, profecias e a presença sobrenatural de Deus. O mais notável foi o colapso das barreiras raciais em plena era Jim Crow: negros, brancos e hispânicos adoravam juntos. Frank Bartleman escreveu: 'A linha da cor foi lavada pelo sangue.'",
                KeyEvents = "Abril 1906: Início do avivamento; glossolalia e curas; barreiras raciais derrubadas; jornais reportam fenômenos sobrenaturais; missionários de todo o mundo visitam e levam o avivamento para casa.",
                Impact = "Nascimento do pentecostalismo moderno; hoje mais de 600 milhões de pentecostais/carismáticos no mundo; evangelização global acelerada; Assembleia de Deus e centenas de denominações pentecostais fundadas.",
                EstimatedConversions = "Ponto de origem para 600 milhões de pentecostais/carismáticos contemporâneos"
            }
        };

        await db.ChurchHeroes.AddRangeAsync(heroes);
        await db.Revivals.AddRangeAsync(revivals);
        await db.SaveChangesAsync();
    }

    // ── Eschatology ───────────────────────────────────────────────────────────

    private static async Task SeedEschatologyAsync(AppDbContext db)
    {
        if (await db.EschatologyViews.AnyAsync())
            return;

        var views = new List<EschatologyView>
        {
            new()
            {
                Name = "Amilenismo",
                Summary = "O milênio de Apocalipse 20 é simbólico, representando o reinado de Cristo no coração dos crentes entre a primeira e segunda vinda. Não há milênio literal futuro na terra.",
                DetailedExplanation = "O amilenismo (do grego 'a' = sem + millennium) não nega o milênio, mas o interpreta espiritualmente. Cristo reina agora no coração dos crentes e no céu, onde as almas dos mártires reinam com ele (Ap 20:4). O 'acorrentamento' de Satanás (Ap 20:2-3) ocorreu na cruz/ressurreição, limitando seu poder de enganar as nações (Lc 10:18; Jo 12:31). A história culminará com a segunda vinda de Cristo, ressurreição geral, julgamento final e o estado eterno — céu novo e terra nova. Agostinho de Hipona sistematizou esta visão em 'A Cidade de Deus'.",
                MainTheologians = "Agostinho de Hipona, Lúcio Crisóstomo, Calvino, Abraham Kuyper, Anthony Hoekema, Kim Riddlebarger, Sam Storms",
                KeyScriptures = "Apocalipse 20:1-6; Lucas 10:18; João 12:31; Romanos 16:20; 2 Tessalonicenses 1:6-10",
                Strengths = "Leva a sério o gênero apocalíptico de Apocalipse; consistente com a hermenêutica simbólica das visões de João; evita especulação sobre cronologia futura; enfatiza o reinado presente de Cristo e o avanço do reino.",
                Weaknesses = "Dificuldade com a literalidade de 'mil anos'; tensão com promessas do AT sobre restauração de Israel; pode enfraquecer a expectativa de transformação histórica futura.",
                MillenniumView = "Simbólico — presente era da Igreja",
                RaptureView = "Não há arrebatamento pré-tribulacional; segunda vinda única e gloriosa",
                TribulationView = "Tribulação presente ao longo de toda a era da Igreja; não futura e localizada"
            },
            new()
            {
                Name = "Pós-milenismo",
                Summary = "Cristo retornará após um milênio literal ou figurativo de expansão do reino de Deus na terra, no qual a Igreja triunfará e as nações serão discipladas.",
                DetailedExplanation = "O pós-milenismo ensina que o evangelho terá êxito crescente, transformando as culturas e sociedades antes da segunda vinda. O 'milênio' pode ser literal (1000 anos) ou figurativo (era longa de prosperidade do reino). A Grande Comissão será plenamente cumprida — todas as nações serão discipladas. Ao final desse período de florescimento, Satanás será solto brevemente, haverá apostasia e perseguição, e então Cristo retornará para o julgamento final. Jonathan Edwards e os puritanos viam o avanço missionário como cumprimento desta visão.",
                MainTheologians = "Jonathan Edwards, John Owen, Charles Hodge, B. B. Warfield, Loraine Boettner, Rousas Rushdoony, Kenneth Gentry",
                KeyScriptures = "Mateus 28:18-20; Salmos 2:8; Salmos 110:1; Isaías 2:2-4; Daniel 2:35; 1 Coríntios 15:25",
                Strengths = "Alta visão do poder transformador do evangelho; motivação missionária robusta; otimismo histórico baseado nas promessas de Deus; valorização da transformação cultural e social.",
                Weaknesses = "Tensão com a narrativa de apostasia crescente no NT (2 Tm 3:1-5; Mt 24:12); dificuldade diante do fracasso histórico de 'cristandade'; risco de confundir progresso cultural com o reino de Deus.",
                MillenniumView = "Período futuro de expansão e triunfo do reino, antes da segunda vinda",
                RaptureView = "Segunda vinda única após o milênio; sem arrebatamento separado",
                TribulationView = "Tribulação do séc. I (70 d.C.) ou apostasia final breve antes da segunda vinda"
            },
            new()
            {
                Name = "Pré-milenismo Histórico",
                Summary = "Cristo retornará antes de um milênio literal de 1000 anos em que reinará fisicamente na terra, com os santos ressuscitados. Não há arrebatamento pré-tribulacional.",
                DetailedExplanation = "O pré-milenismo histórico é provavelmente a visão mais antiga da Igreja pós-apostólica. Ensina que haverá grande tribulação, seguida da segunda vinda gloriosa de Cristo (visível a todos), ressurreição dos mártires e santos, e um reinado de Cristo na terra por 1000 anos literais ou longos. Após o milênio, Satanás é solto, a grande batalha final ocorre, há a ressurreição de todos os mortos e o julgamento final. Difere do dispensacionalismo por não ter arrebatamento secreto pré-tribulacional e por ver a Igreja como continuação do Israel do NT.",
                MainTheologians = "Ireneu de Lyon, Justino Mártir, Papias, George Eldon Ladd, Wayne Grudem, John Piper, Craig Blomberg",
                KeyScriptures = "Apocalipse 19:11-21; Apocalipse 20:1-6; 1 Tessalonicenses 4:16-17; Mateus 24:29-31; Daniel 7:13-14",
                Strengths = "Leveza literal com o texto de Apocalipse 20; raízes na patrística primitiva; distingue ressurreições (primeira e segunda); coerência com promessas do AT de reinado messiânico na terra.",
                Weaknesses = "Tensão entre Israel e Igreja nas promessas; como conciliar um reinado físico com o estado eterno; menor clareza sobre a identidade dos santos que reinarão.",
                MillenniumView = "Literal (1000 anos) após a segunda vinda de Cristo",
                RaptureView = "Sem arrebatamento pré-tribulacional; segunda vinda única e gloriosa",
                TribulationView = "Grande tribulação futura antes da segunda vinda; santos presentes nela"
            },
            new()
            {
                Name = "Dispensacionalismo Pré-milenista",
                Summary = "Sistema teológico que divide a história em 'dispensações'. Defende arrebatamento pré-tribulacional, tribulação de 7 anos e milênio literal de 1000 anos com Israel restaurado em posição central.",
                DetailedExplanation = "Desenvolvido por John Nelson Darby (séc. XIX) e popularizado pela Bíblia de Scofield e Hal Lindsey, o dispensacionalismo distingue Israel (programa terreno) da Igreja (programa celestial). Seus marcos: (1) Arrebatamento secreto — Cristo leva a Igreja antes da tribulação (1 Ts 4:17); (2) Tribulação de 7 anos — Daniel 9:27; período de ira divina sobre Israel; (3) Segunda vinda gloriosa — Cristo retorna com a Igreja para salvar Israel; (4) Milênio literal — reinado de Cristo em Jerusalém; templo reconstruído com sacrifícios memoriais; (5) Distinção Israel/Igreja — as promessas do AT a Israel se cumprirão literalmente com Israel étnico, não com a Igreja.",
                MainTheologians = "John Nelson Darby, C. I. Scofield, Lewis Sperry Chafer, John F. Walvoord, Charles Ryrie, Hal Lindsey, Tim LaHaye",
                KeyScriptures = "1 Tessalonicenses 4:16-17; Daniel 9:24-27; Apocalipse 4-19 (tribulação); Zacarias 14; Ezequiel 40-48 (templo)",
                Strengths = "Leitura literal e consistente das promessas do AT a Israel; sistema muito detalhado e mapeado; clareza cronológica dos eventos futuros; base popular de estudo profético.",
                Weaknesses = "Distinção Israel/Igreja não claramente suportada pelo NT (Gl 3:28-29; Rm 11; Ef 2:11-22); arrebatamento secreto pré-tribulacional sem precedente patrístico; risco de tornar a profecia uma ciência de especulação; pressuposto hermenêutico de literalismo rígido no AT.",
                MillenniumView = "Literal (1000 anos) após segunda vinda; foco em Israel em Jerusalém",
                RaptureView = "Arrebatamento pré-tribulacional secreto (1 Ts 4:16-17); 7 anos antes da segunda vinda",
                TribulationView = "Tribulação futura de 7 anos (semana de Daniel); Igreja arrebatada antes; foco em Israel"
            }
        };

        await db.EschatologyViews.AddRangeAsync(views);
        await db.SaveChangesAsync();
    }
}
