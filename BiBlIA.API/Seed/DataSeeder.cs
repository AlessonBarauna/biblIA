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
        await SeedBibleStudyNotesAsync(db);
        await SeedTheologyAsync(db);
        await SeedChurchHistoryAsync(db);
        await SeedEschatologyAsync(db);
        await SeedReadingPlansAsync(db);
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
        var genesis = await db.BibleBooks.FirstAsync(b => b.OrderIndex == 1);
        var psalms = await db.BibleBooks.FirstAsync(b => b.OrderIndex == 19);
        var john = await db.BibleBooks.FirstAsync(b => b.OrderIndex == 43);
        var romans = await db.BibleBooks.FirstAsync(b => b.OrderIndex == 45);

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

    // ── Bible Study Notes ─────────────────────────────────────────────────────

    private static async Task SeedBibleStudyNotesAsync(AppDbContext db)
    {
        // Guard por entrada: verifica (BookId, Chapter) individualmente.
        // Assim novos capítulos podem ser adicionados sem recriar o banco.
        var books = await db.BibleBooks
            .Where(b => new[] { 1, 2, 19, 23, 27, 40, 43, 45, 46, 48, 49, 58, 66 }.Contains(b.OrderIndex))
            .ToDictionaryAsync(b => b.OrderIndex);

        if (!books.ContainsKey(1)) return; // livros ainda não seedados

        books.TryGetValue(1, out var genesis);
        books.TryGetValue(2, out var exodus);
        books.TryGetValue(19, out var psalms);
        books.TryGetValue(23, out var isaiah);
        books.TryGetValue(27, out var daniel);
        books.TryGetValue(40, out var matthew);
        books.TryGetValue(43, out var john);
        books.TryGetValue(45, out var romans);
        books.TryGetValue(46, out var corinth1);
        books.TryGetValue(48, out var galatians);
        books.TryGetValue(49, out var ephesians);
        books.TryGetValue(58, out var hebrews);
        books.TryGetValue(66, out var rev);

        var notes = new List<BibleStudyNote>();

        // ── Gênesis 1 ─────────────────────────────────────────────────────────
        notes.Add(new BibleStudyNote
        {
            BookId = genesis.Id,
            Chapter = 1,
            Title = "A Criação — Fundamentos da Existência",
            Context = "Gênesis 1 abre as Escrituras com uma declaração teológica, não um tratado científico moderno. O texto foi escrito por Moisés para Israel recém-libertado do Egito, um povo cercado por cosmovisões politeístas onde os astros eram deuses, a criação era luta entre divindades e o ser humano era escravo dos deuses. O Gênesis desmonta tudo isso: há um único Deus soberano, a criação é boa e ordenada, e o ser humano é portador da imagem divina.",
            TheologicalSignificance = "Este capítulo estabelece os dois pilares da antropologia cristã: (1) Criação ex nihilo — Deus cria do nada por sua Palavra soberana, não a partir de matéria pré-existente (Hb 11:3); (2) Imago Dei — o ser humano, homem e mulher, é criado à imagem e semelhança de Deus (v. 26-27), o que confere dignidade inalienável a todo ser humano independente de raça, sexo ou condição. Calvino chamou o Gênesis 1 de 'o teatro da glória de Deus'.",
            KeyThemes = "Criação ex nihilo · Imago Dei · Soberania divina · Bondade da criação · Mandato cultural · Monoteísmo",
            CrossReferences = "João 1:1-3; Hebreus 11:3; Colossenses 1:15-17; Salmos 33:6,9; Provérbios 8:22-31",
            Commentary = "O padrão estrutural do capítulo — 'E disse Deus... e assim se fez... e viu Deus que era bom' — revela a criação como ato de palavra soberana e avaliação. Os dias 1-3 formam 'reinos' e os dias 4-6 seus 'governantes': luz/trevas → astros; céu/mar → aves/peixes; terra/vegetação → animais/homem. A criação é funcional e ordenada, com o sábado (dia 7, Gn 2:1-3) como clímax e sinal da aliança.",
            AuthorNote = "Baseado em Calvino (Commentaries on Genesis), Matthew Henry (Commentary), Wayne Grudem (Teologia Sistemática, cap. 15) e John Walton (The Lost World of Genesis One)"
        });

        // ── Gênesis 3 ─────────────────────────────────────────────────────────
        notes.Add(new BibleStudyNote
        {
            BookId = genesis.Id,
            Chapter = 3,
            Title = "A Queda — Origem do Pecado e a Protoevangelium",
            Context = "Gênesis 3 narra a entrada do pecado na criação perfeita. A serpente, instrumento de Satanás (Ap 12:9), questiona a Palavra de Deus ('É assim que Deus disse?') e ataca o caráter divino ('Deus sabe que...'), as duas estratégias permanentes da tentação: duvidar da Palavra e difamar o caráter de Deus. Adão, presente e passivo (v. 6), falha em sua responsabilidade como guardião do jardim.",
            TheologicalSignificance = "A Queda explica a condição humana: (1) Separação de Deus — expulsão do jardim, quebra da comunhão; (2) Pecado original — todos em Adão pecaram (Rm 5:12); (3) Consequências: trabalho difícil, dor no parto, morte física e espiritual. Mas v. 15 contém a Protoevangelium ('primeiro evangelho'): 'Porei inimizade entre ti e a mulher, entre a tua descendência e a dela; esta te ferirá a cabeça, e tu lhe ferirás o calcanhar.' — a primeira promessa messiânica, cumprida na cruz e ressurreição de Cristo.",
            KeyThemes = "Origem do pecado · Tentação · Responsabilidade humana · Consequências da queda · Protoevangelium · Graça preveniente",
            CrossReferences = "Romanos 5:12-21; 1 Coríntios 15:21-22; 2 Coríntios 11:3; Apocalipse 12:9; João 8:44",
            Commentary = "A sequência da tentação em 1 João 2:16 encontra eco em Gênesis 3:6: 'a mulher viu que a árvore era boa para comer' (concupiscência da carne), 'agradável aos olhos' (concupiscência dos olhos), 'desejável para dar entendimento' (soberba da vida). Jesus resistiu às mesmas três tentações no deserto (Mt 4) — o Segundo Adão onde o primeiro falhou.",
            AuthorNote = "Baseado em Matthew Henry, Calvino (Commentaries), e Derek Kidner (Genesis, Tyndale OT Commentary)"
        });

        // ── Êxodo 20 ──────────────────────────────────────────────────────────
        if (exodus != null)
            notes.Add(new BibleStudyNote
            {
                BookId = exodus.Id,
                Chapter = 20,
                Title = "Os Dez Mandamentos — A Lei da Aliança",
                Context = "Êxodo 20 é o coração da aliança mosaica, promulgada no monte Sinai após a libertação do Egito. Importante: os mandamentos são dados APÓS a redenção, não como condição para ela — 'Eu sou o Senhor teu Deus, que te tirei do Egito' (v. 2) precede todos os mandamentos. A lei revela quem é o Deus redentor e como o povo redimido deve viver.",
                TheologicalSignificance = "Os Dez Mandamentos (Decálogo) estruturam-se em dois grupos: (1) Tábua 1 (v. 3-11) — deveres para com Deus: sem outros deuses, sem ídolos, não usar o nome em vão, guardar o sábado; (2) Tábua 2 (v. 12-17) — deveres para com o próximo: honrar pais, não matar, não adulterar, não furtar, não dar falso testemunho, não cobiçar. Jesus resumiu em dois mandamentos (Mt 22:37-40) e cumpriu plenamente a Lei (Mt 5:17). Paulo ensina que a Lei revela o pecado (Rm 3:20) e aponta para Cristo (Gl 3:24).",
                KeyThemes = "Decálogo · Lei e graça · Aliança mosaica · Adoração exclusiva · Ética social · Sábado",
                CrossReferences = "Deuteronômio 5:6-21; Mateus 5:17-20; 22:37-40; Romanos 3:20; 7:7-12; Gálatas 3:24",
                Commentary = "A proibição de imagens (v. 4-6) não proíbe toda arte — mas qualquer representação de Deus, pois Deus é Espírito (Jo 4:24) e transcende toda representação. A promessa anexa ao quinto mandamento ('para que se prolonguem os teus dias', v. 12) é citada por Paulo como o 'primeiro mandamento com promessa' (Ef 6:2-3).",
                AuthorNote = "Baseado em Calvino (Institutes II.8), Matthew Henry, e John Durham (Exodus, Word Biblical Commentary)"
            });

        // ── Salmos 23 ─────────────────────────────────────────────────────────
        notes.Add(new BibleStudyNote
        {
            BookId = psalms.Id,
            Chapter = 23,
            Title = "O Senhor é Meu Pastor — Confiança Total em Deus",
            Context = "Escrito por Davi, pastor e rei de Israel, este salmo reflete tanto sua experiência pessoal com o pastoreio quanto sua fé profunda em Yahweh como pastor da nação (Sl 80:1). É o mais amado dos Salmos e foi cantado no leito de morte de incontáveis crentes ao longo de séculos.",
            TheologicalSignificance = "O salmo progride em duas metáforas: (1) v. 1-4: Yahweh como Pastor — provisão (pastos, águas tranquilas), restauração, guia, proteção no vale da sombra da morte; (2) v. 5-6: Yahweh como Anfitrião — banquete diante dos inimigos, unção de honra, bondade e misericórdia como companheiras permanentes. Jesus se identifica como o 'Bom Pastor' (Jo 10:11,14), cumprindo este salmo — ele dá a vida pelas ovelhas.",
            KeyThemes = "Cuidado divino · Provisão · Proteção · Morte sem medo · Hospitalidade divina · Pastoreio messiânico",
            CrossReferences = "João 10:11,14; Ezequiel 34:11-16; Isaías 40:11; Hebreus 13:20; 1 Pedro 2:25; Apocalipse 7:17",
            Commentary = "'Vale da sombra da morte' (v. 4, tzalmaveth) pode ser traduzido como 'vale de trevas densas' — seja morte literal ou circunstâncias opressoras. 'Teu cajado e teu bordão me consolam': o cajado (shebet) servia para defender contra predadores; o bordão (misheneth) para apoiar o pastor e guiar as ovelhas. Ambos falam do cuidado protetor e guiador de Deus.",
            AuthorNote = "Baseado em Spurgeon (The Treasury of David), Matthew Henry, e Derek Kidner (Psalms 1-72, Tyndale)"
        });

        // ── Isaías 53 ─────────────────────────────────────────────────────────
        if (isaiah != null)
            notes.Add(new BibleStudyNote
            {
                BookId = isaiah.Id,
                Chapter = 53,
                Title = "O Servo Sofredor — A Profecia da Expiação",
                Context = "Isaías 53 faz parte do quarto e último 'Cântico do Servo' (52:13-53:12), escrito 700 anos antes de Cristo. É o capítulo mais citado no NT (mais de 40 referências). Os judeus do primeiro século esperavam um Messias conquistador; Isaías profetiza um Messias sofredor. O capítulo começa com espanto universal ('a quem foi revelado o braço do Senhor?') e termina com a exaltação do Servo.",
                TheologicalSignificance = "Este capítulo é o locus classicus da substituição penal no AT: 'ele foi ferido pelas nossas transgressões, moído pelas nossas iniquidades' (v. 5); 'o Senhor fez cair sobre ele a iniquidade de nós todos' (v. 6); 'pelo seu conhecimento o meu servo justo justificará a muitos, e as iniquidades deles ele mesmo suportará' (v. 11). Philip apologize ensinou que Filipe leu este capítulo ao eunuco etíope (At 8:32-35), que reconheceu Jesus. É impossível ler sem ver a cruz.",
                KeyThemes = "Substituição penal · Sofrimento vicário · Exaltação após humilhação · Justiça divina · Missão messiânica · Profecia cumprida",
                CrossReferences = "Atos 8:32-35; Romanos 4:25; 1 Pedro 2:22-25; 2 Coríntios 5:21; Hebreus 9:28; 1 João 2:2",
                Commentary = "A estrutura quiástica do capítulo é notável: A (exaltação, 52:13-15) — B (rejeição humana, 53:1-3) — C (sofrimento vicário, 53:4-6) — B' (silêncio ante o sofrimento, 53:7-9) — A' (exaltação e fruto, 53:10-12). 'Pela sua ferida fomos sarados' (v. 5) é o versículo mais citado para a doutrina da expiação substitutiva em toda a Escritura.",
                AuthorNote = "Baseado em Calvino (Commentary on Isaiah), Matthew Henry, e John Oswalt (Isaiah, NICOT)"
            });

        // ── Mateus 5 ──────────────────────────────────────────────────────────
        if (matthew != null)
            notes.Add(new BibleStudyNote
            {
                BookId = matthew.Id,
                Chapter = 5,
                Title = "O Sermão do Monte — A Ética do Reino",
                Context = "Mateus 5-7 contém o mais longo discurso de Jesus registrado nos Evangelhos. Jesus sobe ao monte (como Moisés ao Sinai) e estabelece a ética do Reino. O capítulo 5 começa com as Bem-aventuranças (v. 3-12) — retrato do caráter do cidadão do Reino — e continua com a relação entre Jesus e a Lei (v. 17-20) e seis antíteses ('Ouvistes que foi dito... mas eu vos digo', v. 21-48).",
                TheologicalSignificance = "As Bem-aventuranças invertem os valores do mundo: os pobres de espírito, os que choram, os mansos, os que têm fome de justiça, os misericordiosos, os puros de coração, os pacificadores e os perseguidos são os bem-aventurados. As antíteses mostram que Jesus não aboliu a Lei mas a cumpriu e aprofundou — do externo ao interno (ira = assassinato no coração; lascívia = adultério no coração). A demanda ética é impossível por forças humanas: só é cumprida naqueles que nasceram de novo.",
                KeyThemes = "Bem-aventuranças · Ética do Reino · Lei e graça · Radicalidade do discipulado · Sal e luz · Justiça que excede a dos fariseus",
                CrossReferences = "Lucas 6:20-49 (Sermão na Planície); Romanos 8:3-4; Gálatas 5:22-23; Tiago 3:13-18; Miquéias 6:8",
                Commentary = "'Bem-aventurados os pobres de espírito' (v. 3) — não os economicamente pobres, mas os que reconhecem sua bancarrota espiritual diante de Deus. É o oposto da autoconfiança religiosa. 'Sede, pois, vós perfeitos, como é perfeito o vosso Pai celestial' (v. 48) — não perfeição sinless nesta vida, mas teleios = completo, maduro, íntegro; o padrão é o caráter do Pai.",
                AuthorNote = "Baseado em D. A. Carson (The Sermon on the Mount), John Stott (O Sermão do Monte), Matthew Henry"
            });

        // ── João 3 ────────────────────────────────────────────────────────────
        notes.Add(new BibleStudyNote
        {
            BookId = john.Id,
            Chapter = 3,
            Title = "Novo Nascimento e João 3:16 — O Evangelho em Miniatura",
            Context = "Nicodemos era membro do Sinédrio (conselho supremo judeu de 70 líderes) e um fariseu — o mais rigoroso grupo religioso de Israel. Ele vem a Jesus de noite (por medo? por necessidade genuína de conversa privada?). Sua confissão inicial é teológica correta mas incompleta: reconhece Jesus como 'Mestre vindo de Deus' mas não como Messias e Salvador.",
            TheologicalSignificance = "Jesus ensina que o novo nascimento (gennēthē anōthen — nascer de novo/do alto) é necessário e não opcional para 'ver o Reino de Deus' (v. 3). Não é reforma moral ou decisão religiosa — é obra soberana do Espírito (v. 8, 'o vento sopra onde quer'). João 3:16 é chamado de 'o Evangelho em miniatura': Deus (agente), amou (motivação), o mundo (extensão), de tal maneira que deu (ação), o Filho unigênito (dádiva), para que todo aquele que nele crê (condição), não pereça (conseqüência negativa), mas tenha a vida eterna (positiva).",
            KeyThemes = "Novo nascimento · Soberania do Espírito · Amor divino · Fé · Vida eterna · Condenação · Luz e trevas",
            CrossReferences = "1 Pedro 1:3,23; Tiago 1:18; Efésios 2:4-5; Tito 3:5; 1 João 5:1; Romanos 8:14-16",
            Commentary = "'Pois Deus amou o mundo de tal maneira' — o amor de Deus é a causa, não o efeito, da fé humana. 'Unigênito' (monogenēs) significa 'único de seu tipo', não 'gerado no tempo' — aponta para a relação única e eterna do Filho com o Pai. 'Não pereça' (apollymi) é perda total, destruição — a seriedade do julgamento eterno legitima a urgência da missão.",
            AuthorNote = "Baseado em Calvino (Commentary on John), Leon Morris (The Gospel According to John, NICNT), Matthew Henry"
        });

        // ── Romanos 3 ─────────────────────────────────────────────────────────
        notes.Add(new BibleStudyNote
        {
            BookId = romans.Id,
            Chapter = 3,
            Title = "Justificação pela Fé — O Coração do Evangelho",
            Context = "Romanos 1-3 é o grande diagnóstico da condição humana: gentios sem a Lei pecaram contra a luz da criação (1:18-32); judeus com a Lei também pecaram (2:1-29); portanto 'todos pecaram e carecem da glória de Deus' (3:23). Romanos 3:21-31 é a virada — a solução divina para a condenação universal: a justiça de Deus revelada pelo evangelho.",
            TheologicalSignificance = "Três conceitos-chave em 3:24-25: (1) Justificação (dikaiōsis) — declaração forense de inocência; Deus não nos torna justos gradualmente, mas nos declara justos instantaneamente ao justificar pela fé; (2) Redenção (apolytrōsis) — libertação mediante pagamento de resgate; (3) Propiciação (hilastērion) — satisfação da ira divina; a mesma palavra usada para a 'tampa da arca da aliança' no AT onde o sangue era aspergido. A cruz é onde a justiça e a misericórdia de Deus se encontram: Deus é 'justo e justificador' (v. 26) simultaneamente.",
            KeyThemes = "Pecado universal · Justificação · Propiciação · Redenção · Fé · Obras da Lei · Glória de Deus",
            CrossReferences = "Gálatas 2:16; 3:10-14; Efésios 2:8-9; Filipenses 3:9; 2 Coríntios 5:21; Isaías 53:11; Habacuque 2:4",
            Commentary = "'A justiça de Deus' (v. 21) é ambígua — pode ser: (a) atributo de Deus = sua perfeição moral; (b) status que Deus confere = justiça imputada ao crente. Luther descobriu que era ambas: Deus é justo e confere sua justiça ao pecador pela fé — 'a mais doce de todas as doutrinas'. Lutero disse que ao entender v. 17 ('o justo viverá pela fé'), sentiu que havia entrado nas portas do paraíso.",
            AuthorNote = "Baseado em Calvino (Commentary on Romans), Douglas Moo (Romans, NICNT), Matthew Henry, Martin Luther (Prefácio a Romanos)"
        });

        // ── Romanos 8 ─────────────────────────────────────────────────────────
        notes.Add(new BibleStudyNote
        {
            BookId = romans.Id,
            Chapter = 8,
            Title = "Vida no Espírito e Segurança Eterna",
            Context = "Romanos 8 é o clímax da carta — após o diagnóstico do pecado (1-3), a justificação pela fé (3-5) e a luta interior do crente (7), Paulo descreve a vida abundante no Espírito. Começa com 'Portanto, agora nenhuma condenação há para os que estão em Cristo Jesus' (v. 1) e termina com a inseperabilidade do amor de Deus (v. 31-39). É o capítulo mais completo sobre o Espírito Santo no NT.",
            TheologicalSignificance = "O Espírito Santo aparece 19 vezes no capítulo: ele liberta (v. 2), vivifica (v. 10-11), guia (v. 14), adota (v. 15-16), intercede (v. 26-27), e garante a glorificação futura (v. 17,30). A cadeia de ouro da salvação (v. 29-30) — predestinação → chamado → justificação → glorificação — afirma que todos os que Deus chamou serão glorificados, garantindo a perseverança dos santos. 'Quem nos separará do amor de Cristo?' (v. 35) recebe a resposta mais enfática da Bíblia: nada na criação.",
            KeyThemes = "Vida no Espírito · Adoção · Gemidos da criação · Intercessão do Espírito · Cadeia áurea da salvação · Segurança do crente",
            CrossReferences = "João 14:16-17; Gálatas 4:6; Efésios 1:13-14; 1 João 3:1-2; João 10:27-30; Filipenses 1:6",
            Commentary = "'O Espírito mesmo intercede por nós com gemidos inexprimíveis' (v. 26) — na oração do crente, o Espírito se une aos nossos gemidos e os apresenta ao Pai de acordo com a vontade divina. Não é que rezamos errado; é que nossa limitação humana é suprida pela intercessão trinitária (Filho intercede no céu, Hb 7:25; Espírito intercede em nós). A segurança do crente não repousa em sua fidelidade, mas na fidelidade de Deus.",
            AuthorNote = "Baseado em Calvino (Commentary on Romans), Spurgeon (Romans, sermões), John Murray (Romans, NICNT)"
        });

        // ── 1 Coríntios 13 ────────────────────────────────────────────────────
        if (corinth1 != null)
            notes.Add(new BibleStudyNote
            {
                BookId = corinth1.Id,
                Chapter = 13,
                Title = "O Hino do Amor — Agapē como Critério de Tudo",
                Context = "1 Coríntios 13 está inserido na discussão sobre dons espirituais (cap. 12-14). A igreja de Corinto era espiritualmente rica em dons mas moralmente imatura — havia divisões, orgulho, imoralidade. Paulo insere este capítulo como o 'caminho mais excelente' (12:31): sem amor, todos os dons são inúteis. É poesia e teologia ao mesmo tempo.",
                TheologicalSignificance = "O amor (agapē) descrito não é sentimento romântico (eros) nem amizade (philia), mas amor incondicional, decisão de querer o bem do outro. Paulo primeiro mostra que sem amor os maiores dons (línguas angélicas, profecia, conhecimento, fé para milagres, martírio) 'nada valem' (v. 1-3). Depois descreve 15 características do amor (v. 4-7). Finalmente: fé, esperança e amor permanecem, 'mas o maior destes é o amor' (v. 13) — porque amor é o próprio caráter de Deus (1 Jo 4:8).",
                KeyThemes = "Amor ágape · Primazia do amor sobre os dons · Maturidade cristã · Permanência do amor · Fé, esperança e amor",
                CrossReferences = "1 João 4:8,16; João 13:34-35; Gálatas 5:22; Romanos 13:8-10; Colossenses 3:14; Mateus 22:37-40",
                Commentary = "As 15 características do amor em v. 4-7 podem ser divididas: 7 negativas (o que o amor NÃO é: invejoso, arrogante, rude, egoísta, irritável, rancoroso, aprazível com injustiça) e 8 positivas. A tradução literal de v. 7 é: 'tudo cobre, tudo crê, tudo espera, tudo suporta' — quatro verbos absolutos que descrevem amor como cobertura incondicional, confiança resiliente, esperança persistente e suporte inabalável.",
                AuthorNote = "Baseado em Spurgeon (sermão sobre 1 Co 13), Matthew Henry, Gordon Fee (1 Corinthians, NICNT)"
            });

        // ── Apocalipse 21 ─────────────────────────────────────────────────────
        if (rev != null)
            notes.Add(new BibleStudyNote
            {
                BookId = rev.Id,
                Chapter = 21,
                Title = "A Nova Jerusalém — A Consumação de Todas as Coisas",
                Context = "Apocalipse 21-22 é o clímax não só do Apocalipse mas de toda a Bíblia — a consumação do plano redentor de Deus. O livro foi escrito por João em Patmos (cerca de 95 d.C.) para igrejas sob perseguição imperial romana. A visão final responde à pergunta de todos os que sofrem: como isso vai terminar? A resposta é triunfante.",
                TheologicalSignificance = "Três declarações definem a nova criação: (1) 'Eis que faço novas todas as coisas' (v. 5) — não destruição e substituição, mas renovação e transformação; (2) 'E ouvi uma grande voz que dizia: Eis o tabernáculo de Deus com os homens' (v. 3) — o propósito de toda a história: Deus habitando com seu povo; (3) 'Não haverá mais morte, nem luto, nem choro, nem dor' (v. 4) — a reversão total da Queda (Gn 3). A Nova Jerusalém não é lugar para onde vamos, mas realidade que desce — 'descendo do céu de Deus' (v. 2,10).",
                KeyThemes = "Nova criação · Habitação de Deus com os homens · Fim do sofrimento · Nova Jerusalém · Noiva de Cristo · Consumação da aliança",
                CrossReferences = "Gênesis 1-2; Isaías 65:17-25; 2 Pedro 3:13; 1 Coríntios 15:54-55; Ezequiel 37:27; João 14:2-3",
                Commentary = "A descrição da Cidade — 12.000 estádios (2.200 km) em cubo perfeito (v. 16) — não é arquitetura literal mas linguagem simbólica de perfeição e completude. O cubo lembra o Santo dos Santos (1 Rs 6:20), sugerindo que a cidade inteira é o lugar da presença divina. 'Não haverá templo' (v. 22) porque Deus e o Cordeiro são o templo — comunhão direta e permanente.",
                AuthorNote = "Baseado em Matthew Henry, G. K. Beale (Revelation, NIGTC), e N. T. Wright (Surprised by Hope)"
            });

        // ── Novas notas (adicionadas em 2026-03-28) ──────────────────────────

        // João 1 — O Verbo eterno
        if (john != null)
            notes.Add(new BibleStudyNote
            {
                BookId = john.Id,
                Chapter = 1,
                Title = "O Verbo Eterno — O Prólogo de João",
                Context = "O Prólogo de João (1:1-18) é um dos textos mais profundos do NT, escrito como uma abertura poética e teológica que enquadra todo o Evangelho. João escreve para um público greco-romano familiarizado com o conceito de Logos (razão, palavra) dos filósofos, mas redefine o termo completamente: o Logos não é um princípio abstrato, mas uma Pessoa que 'se fez carne e habitou entre nós'. O paralelismo com Gênesis 1 ('No princípio') é intencional.",
                TheologicalSignificance = "João 1:1-3 afirma a pré-existência, a distinção pessoal e a plena divindade do Filho: 'o Verbo era Deus' (theos ēn ho logos). O versículo 14 é o texto central da Encarnação: 'O Verbo se fez carne' (sarx egeneto) — Deus assumiu a natureza humana plena sem abandonar a divindade, o que o Concílio de Calcedônia (451) formulou como 'uma pessoa, duas naturezas'. 'Vimos a sua glória' aponta para uma experiência apostólica real, não mitológica.",
                KeyThemes = "Logos · Pré-existência de Cristo · Encarnação · Luz e trevas · Testemunho de João Batista · Graça sobre graça",
                CrossReferences = "Gênesis 1:1; Colossenses 1:15-17; Hebreus 1:1-3; Filipenses 2:6-7; 1 João 1:1-3; Apocalipse 19:13",
                Commentary = "'Nele estava a vida, e a vida era a luz dos homens' (v. 4) — vida (zōē) em João é sempre vida divina, eterna, não apenas existência biológica. A metáfora da luz introduz o tema central do Evangelho: Jesus como luz que os homens preferem rejeitar (v. 10-11) mas que ilumina todo crente. 'Cheio de graça e de verdade' (v. 14) — charis kai alētheia é eco do AT hesed we-emet (amor leal e fidelidade) — Yahweh revelado em Cristo.",
                AuthorNote = "Baseado em Calvino (Commentary on John), Leon Morris (The Gospel According to John, NICNT), e C.S. Lewis (Mere Christianity, cap. sobre o Filho de Deus)"
            });

        // Gênesis 22 — O sacrifício de Isaque
        if (genesis != null)
            notes.Add(new BibleStudyNote
            {
                BookId = genesis.Id,
                Chapter = 22,
                Title = "O Sacrifício de Isaque — Fé Radical e Tipologia Cristológica",
                Context = "Gênesis 22, a Akedah ('ligação' em hebraico), é considerado o clímax espiritual do livro e o ponto mais alto da fé de Abraão. Deus havia prometido que a descendência de Abraão viria por Isaque (21:12) — agora ordena o sacrifício do mesmo filho. A tensão teológica é máxima: como Deus pode contradizer sua própria promessa? Esta é a décima e suprema prova da fé de Abraão.",
                TheologicalSignificance = "O capítulo é primariamente tipológico: Abraão (pai) oferece seu filho único amado num monte (Moriá = Jerusalém, 2 Cr 3:1); o filho carrega a lenha de seu próprio sacrifício; no último momento, Deus provê um cordeiro substituto. Hebreus 11:17-19 revela a fé de Abraão: ele esperava que Deus ressuscitaria Isaque, pois a promessa estava nele. João 3:16 ecoa deliberadamente a linguagem de Gn 22:2 ('filho único amado'): o que Abraão não precisou fazer, Deus fez — entregou seu próprio Filho.",
                KeyThemes = "Fé radical · Obediência incondicional · Providência divina · Tipologia cristológica · Substituição · 'Deus proverá'",
                CrossReferences = "João 3:16; Hebreus 11:17-19; Romanos 8:32; 2 Crônicas 3:1; Gálatas 3:16; 1 Pedro 1:19-20",
                Commentary = "'Deus proverá para si o cordeiro' (v. 8, yireh) — a palavra 'proverá' significa literalmente 'verá, providenciará'. O nome do lugar, 'Jeová-Jirê' (v. 14, 'O Senhor proverá'), tornou-se nome de Deus. A fé de Abraão não era cega — era ancorada no caráter de Deus que havia prometido. Hebreus 11 chama isso de 'ressurreição', pois Abraão raciocinou que Deus poderia ressuscitar Isaque dos mortos.",
                AuthorNote = "Baseado em Calvino (Commentaries on Genesis), Derek Kidner (Genesis, Tyndale), e John Walton (Genesis, NIV Application Commentary)"
            });

        // Romanos 5 — Justificação e paz, Adão e Cristo
        if (romans != null)
            notes.Add(new BibleStudyNote
            {
                BookId = romans.Id,
                Chapter = 5,
                Title = "Paz com Deus e o Segundo Adão",
                Context = "Romanos 5 é a ponte entre a doutrina da justificação (cap. 3-4) e a doutrina da santificação (cap. 6-8). Paulo começa com as bênçãos presentes e futuras do justificado (v. 1-11) e termina com o grande paralelo entre Adão e Cristo (v. 12-21), o fundamento bíblico para a doutrina do pecado original e da imputação da justiça.",
                TheologicalSignificance = "Os frutos da justificação (v. 1-5): paz com Deus (não apenas paz de Deus), acesso à graça, esperança da glória, e o paradoxo que o sofrimento produz perseverança → caráter → esperança. A base: o amor de Deus derramado no coração pelo Espírito (v. 5). O paralelo Adão-Cristo (v. 12-21) é a base exegética do pecado original: por um homem (Adão), o pecado entrou; por um homem (Cristo), a graça reina. A fórmula 'muito mais' (v. 15,17) indica que o que Cristo deu supera em muito o que Adão perdeu.",
                KeyThemes = "Paz com Deus · Justificação · Sofrimento e esperança · Amor de Deus · Adão e Cristo · Pecado original · Imputação",
                CrossReferences = "Gênesis 3:17-19; 1 Coríntios 15:21-22,45-49; Filipenses 4:7; João 16:33; Romanos 8:17; Efésios 2:4-5",
                Commentary = "'Sendo ainda pecadores, Cristo morreu por nós' (v. 8) é a prova suprema do amor de Deus: não morreu por bons, mas por inimigos (v. 10). 'Muito mais agora, sendo justificados pelo seu sangue, seremos salvos por ele da ira' (v. 9) — se a morte de Cristo nos reconciliou quando éramos inimigos, quanto mais sua vida (intercessão contínua, Hb 7:25) nos guardará. A certeza da salvação repousa no amor demonstrado na cruz.",
                AuthorNote = "Baseado em Calvino (Commentary on Romans), Douglas Moo (Romans, NICNT), e Martyn Lloyd-Jones (Romans, vol. 4)"
            });

        // Efésios 2 — Por graça sois salvos
        if (ephesians != null)
            notes.Add(new BibleStudyNote
            {
                BookId = ephesians.Id,
                Chapter = 2,
                Title = "Salvos pela Graça mediante a Fé",
                Context = "Efésios 2 é o capítulo mais completo sobre a doutrina da graça no NT. Paulo descreve em três movimentos: (1) a condição humana sem Cristo (v. 1-3); (2) a intervenção divina pela graça (v. 4-10); (3) a unidade de judeus e gentios em Cristo (v. 11-22). É escrito para uma igreja majoritariamente gentílica em Éfeso, que precisava entender tanto sua salvação quanto sua identidade no corpo de Cristo.",
                TheologicalSignificance = "Versículos 8-9 são o texto mais preciso sobre graça e fé no NT: 'Pela graça sois salvos, mediante a fé; e isso não vem de vós, é dom de Deus; não por obras, para que ninguém se glorie.' Três afirmações: (1) A salvação é pela graça (charis — favor imerecido); (2) mediante a fé (pistis — confiança receptiva, não mérito); (3) e nem mesmo a fé é obra humana — 'isso' (touto) é dom de Deus. O v. 10 completa: 'porque somos feitura dele, criados em Cristo Jesus para as boas obras' — obras são fruto, não raiz da salvação.",
                KeyThemes = "Graça soberana · Fé como dom · Depravação humana · Vivificação espiritual · Obras como fruto · Unidade de judeus e gentios",
                CrossReferences = "Romanos 3:24; João 6:44; Filipenses 1:29; Tito 3:5; 1 Pedro 1:3; Colossenses 2:13; Gálatas 2:16",
                Commentary = "'Estáveis mortos em vossas transgressões e pecados' (v. 1) — morte espiritual não é fraqueza mas incapacidade total. Um morto não coopera com sua ressurreição. Isso fundamenta a necessidade de uma obra totalmente divina (regeneração) que precede e possibilita a fé. 'Mas Deus, sendo rico em misericórdia' (v. 4) — as duas palavras mais esperançosas da Bíblia: 'mas Deus' interrompem a trajetória de morte com a intervenção graciosa.",
                AuthorNote = "Baseado em Calvino (Commentary on Ephesians), John Stott (The Message of Ephesians), e Martyn Lloyd-Jones (Ephesians, vol. 2)"
            });

        // João 14 — A casa do Pai, o Caminho
        if (john != null)
            notes.Add(new BibleStudyNote
            {
                BookId = john.Id,
                Chapter = 14,
                Title = "A Casa do Pai — Consolação, Caminho e Espírito",
                Context = "João 14-17 é o Discurso de Despedida, proferido na noite do Getsêmani, horas antes da crucificação. Os discípulos estavam perturbados com o anúncio da partida de Jesus (13:33). O capítulo 14 é o 'sermão da consolação' — Jesus fala ao coração dos que perdem alguém. É o capítulo mais citado em funerais cristãos.",
                TheologicalSignificance = "'Eu sou o caminho, a verdade e a vida' (v. 6) — o sétimo e mais abrangente dos 'Eu Sou' de João. Caminho (hodos): único acesso ao Pai; Verdade (alētheia): revelação plena e confiável de Deus; Vida (zōē): fonte da vida eterna. 'Ninguém vem ao Pai senão por mim' — exclusividade do evangelho que incomoda o pluralismo mas é afirmado sem ambiguidade. O Paráclito (v. 16-17,26): Espírito da Verdade que habitará nos crentes permanentemente, ensinará e lembrará as palavras de Jesus.",
                KeyThemes = "Esperança escatológica · Presença de Cristo · Exclusividade salvífica · Oração em nome de Jesus · Paráclito (Espírito Santo) · Paz de Cristo",
                CrossReferences = "João 11:25; Atos 4:12; Hebreus 10:19-22; 1 João 2:1; João 16:7; Romanos 8:34; 1 Timóteo 2:5",
                Commentary = "'Vou preparar lugar para vós' (v. 2-3) — a linguagem é de hospitalidade oriental: o anfitrião prepara o quarto e depois vai buscar os hóspedes pessoalmente. A promessa não é de um lugar físico mas de comunhão eterna com o Pai e o Filho. 'A paz que vos dou' (v. 27) — shalom judaico em plenitude: não ausência de problemas, mas presença tranquilizante de Deus. 'Não vos turbe o coração' (v. 1,27) — imperativo que supõe perturbação real mas aponta para um recurso real: fé em Deus e em Jesus.",
                AuthorNote = "Baseado em Calvino (Commentary on John), Spurgeon (sermões sobre João 14), e F. F. Bruce (The Gospel of John)"
            });

        // Salmos 119 — A Palavra de Deus
        if (psalms != null)
            notes.Add(new BibleStudyNote
            {
                BookId = psalms.Id,
                Chapter = 119,
                Title = "A Palavra de Deus — O Maior Salmo",
                Context = "Com 176 versículos, o Salmo 119 é o capítulo mais longo da Bíblia. É um acróstico alfabético perfeito: dividido em 22 estrofes (uma para cada letra do alfabeto hebraico), cada versículo dentro da estrofe começa com aquela letra. Essa estrutura não é ornamento: é arte comunicando que o amor pela Palavra abrange de aleph a tau — de A a Z, o inteiro. O autor é desconhecido; o tema é único: a glória e suficiência da Palavra de Deus.",
                TheologicalSignificance = "Oito sinônimos são usados para 'Palavra de Deus': lei (torah), testemunho (edut), preceitos (piqqudim), estatutos (huqqim), mandamentos (mitzvot), juízos (mishpatim), palavra (dabar) e promessas (imra). Cada um revela uma faceta: lei (direção), testemunho (revelação), preceitos (encargos específicos), estatutos (decretos gravados), mandamentos (ordens), juízos (decisões judiciais), palavra (comunicação pessoal), promessas (fidelidade de Deus). O salmista afirma que a Palavra traz: libertação (v. 45), consolo (v. 50,52), luz (v. 105), paz (v. 165), vida (v. 50,93).",
                KeyThemes = "Amor pela Palavra · Meditação · Obediência · Perseverança na aflição · Suficiência das Escrituras · Iluminação do Espírito",
                CrossReferences = "Salmos 19:7-11; Josué 1:8; 2 Timóteo 3:16-17; Hebreus 4:12; João 17:17; Mateus 4:4",
                Commentary = "'Lâmpada para os meus pés é a tua palavra, e luz para o meu caminho' (v. 105) — não holofote que ilumina o horizonte distante, mas lamparina que ilumina o próximo passo. Fidelidade no imediato, não ansiedade sobre o futuro. 'Quão doces são as tuas palavras ao meu paladar! Mais do que o mel à minha boca' (v. 103) — o salmista desenvolveu gosto espiritual pelas Escrituras. O gosto é cultivado, não natural — vem da meditação constante (v. 15,48,97).",
                AuthorNote = "Baseado em Spurgeon (The Treasury of David, vol. 5), Derek Kidner (Psalms 73-150, Tyndale), e Eugene Peterson (A Long Obedience in the Same Direction)"
            });

        // Hebreus 11 — A galeria da fé
        if (hebrews != null)
            notes.Add(new BibleStudyNote
            {
                BookId = hebrews.Id,
                Chapter = 11,
                Title = "A Galeria da Fé — Heróis que Não Receberam a Promessa",
                Context = "Hebreus foi escrito para cristãos judeus tentados a abandonar a fé e retornar ao judaísmo, possivelmente sob perseguição (10:32-34). O capítulo 11 é o clímax do argumento: olhe para esses heróis do AT que perseveraram sem ver o cumprimento — vocês têm mais (Cristo já veio), portanto perseverem mais. É simultaneamente teologia da fé e pedagogia do sofrimento.",
                TheologicalSignificance = "A definição de fé em v. 1 é única no NT: 'certeza (hypostasis — substância, fundação) de coisas que se esperam, prova (elenchos — evidência, convicção) de coisas que não se veem.' Fé não é crença sem evidência, mas convicção baseada na revelação de Deus sobre realidades ainda não visíveis. O capítulo percorre Gênesis ao período intertestamentário. O verso mais tocante: v. 39-40 — 'todos estes, tendo sido aprovados por sua fé, não receberam o que foi prometido, porque Deus tinha provido algo melhor para nós.'",
                KeyThemes = "Definição da fé · Perseverança · Esperança escatológica · Heróis do AT · Sofrimento pela fé · Promessa cumprida em Cristo",
                CrossReferences = "Hebreus 12:1-2; Gênesis 15:6; Romanos 4:18-21; 2 Coríntios 4:17-18; 1 Pedro 1:8-9; João 20:29",
                Commentary = "A estrutura do capítulo repete 'pela fé' (pistei) como refrão — 18 vezes. Cada herói representa uma faceta da fé: Abraão (obediência sem ver o destino, v. 8), Moisés (escolha da reprobridade com Cristo acima dos tesouros do Egito, v. 26), Raabe (fé que ultrapassa identidade étnica, v. 31). A lista termina em silêncio deliberado: os mártires anônimos (v. 36-38) que 'o mundo não era digno de possuir'. A fé não garante prosperidade — garante fidelidade de Deus.",
                AuthorNote = "Baseado em F. F. Bruce (Hebrews, NICNT), John Owen (Exposition of Hebrews), e John Piper (By Faith, Not by Sight)"
            });

        // Gálatas 5 — Liberdade e fruto do Espírito
        if (galatians != null)
            notes.Add(new BibleStudyNote
            {
                BookId = galatians.Id,
                Chapter = 5,
                Title = "Liberdade Cristã e o Fruto do Espírito",
                Context = "Gálatas foi escrita em tom urgente (é a única carta de Paulo sem ação de graças inicial) para igrejas ameaçadas por judaizantes — ensino que a circuncisão e obras da Lei eram necessárias para a justificação. O capítulo 5 aplica a liberdade do evangelho (declarada em 5:1) à vida prática, distinguindo duas possibilidades: andar segundo a carne ou segundo o Espírito.",
                TheologicalSignificance = "O fruto do Espírito (v. 22-23) não são 'obras' que produzimos, mas 'fruto' que o Espírito produz em nós — a distinção é crucial para entender a santificação. As 9 características formam um todo orgânico: amor (agapē) como raiz, e as demais como expressões. A lista das obras da carne (v. 19-21) é o oposto: fragmentação, divisão, perda de controle. 'Contra estas coisas não há lei' (v. 23) — onde o Espírito produz caráter, a lei é desnecessária porque a intenção da lei é cumprida.",
                KeyThemes = "Liberdade cristã · Carne vs. Espírito · Fruto do Espírito · Lei do amor · Crucificação da carne · Andar pelo Espírito",
                CrossReferences = "Romanos 8:4-9; João 15:4-5; 1 Coríntios 13:4-7; Efésios 5:9; Colossenses 3:12-15; 2 Pedro 1:5-8",
                Commentary = "'Andai pelo Espírito e não satisfareis os desejos da carne' (v. 16) — imperativo ativo: não passividade mística mas cooperação deliberada com a obra do Espírito. 'Os que são de Cristo Jesus crucificaram a carne com as suas paixões e concupiscências' (v. 24) — tempo aoristo, ação definida: na conversão, houve uma ruptura decisiva com o 'eu' anterior. A vida cristã é viver conforme essa ruptura já ocorrida — identidade (já crucificados) determina comportamento (portanto andar pelo Espírito).",
                AuthorNote = "Baseado em Calvino (Commentary on Galatians), John Stott (The Message of Galatians), e Tim Keller (Galatians for You)"
            });

        // Daniel 7 — O Filho do Homem
        if (daniel != null)
            notes.Add(new BibleStudyNote
            {
                BookId = daniel.Id,
                Chapter = 7,
                Title = "O Filho do Homem — A Visão dos Quatro Reinos",
                Context = "Daniel 7 é o capítulo mais importante do livro e um dos mais citados no NT (especialmente em Marcos e Apocalipse). Daniel está no exílio babilônico. A visão ocorre no primeiro ano de Belsazar (c. 553 a.C.) e é composta de dois elementos: os quatro reinos representados por bestas (v. 1-14) e a interpretação do anjo (v. 15-28). O capítulo faz a transição da narrativa histórica para a apocalíptica profética.",
                TheologicalSignificance = "A cena do 'Ancião de Dias' (v. 9-10) é uma das descrições mais majestosas de Deus no AT — trono de chamas, rio de fogo, miríades em serviço. O 'Filho do Homem' (v. 13-14) recebe domínio, glória e reino eterno — todos os povos o servem. Jesus adota este título como autorrevelação messiânica preferida (Mc 14:62 — diante do Sinédrio, citando Daniel 7:13). O título une humanidade ('filho do homem') e divindade (recebe adoração universal) — é tanto o representante humano de Israel quanto o Filho divino de Deus.",
                KeyThemes = "Soberania de Deus sobre a história · Quatro impérios · Ancião de Dias · Filho do Homem · Reino eterno · Perseguição e vitória",
                CrossReferences = "Mateus 26:64; Marcos 13:26; Apocalipse 1:7,13-14; 4:2-3; 5:7; João 5:27; Atos 7:56",
                Commentary = "Os quatro reinos (Babilônia/Medo-Pérsia/Grécia/Roma, segundo a interpretação clássica) simbolizam impérios humanos que parecem invencíveis mas são temporários. O 'chifre pequeno' (v. 8) que blasfema e persegue os santos tem sido identificado com Antíoco IV Epifânio (2o séc. a.C.) como cumprimento histórico e com o Anticristo como cumprimento escatológico. O ponto central: nenhum reino humano dura; só o reino do Filho do Homem é eterno.",
                AuthorNote = "Baseado em John Goldingay (Daniel, Word Biblical Commentary), Joyce Baldwin (Daniel, Tyndale), e N. T. Wright (Jesus and the Victory of God)"
            });

        // Filtra: insere apenas notas para capítulos sem nota ainda
        var existingKeys = await db.BibleStudyNotes
            .Select(n => new { n.BookId, n.Chapter })
            .ToListAsync();

        var newNotes = notes
            .Where(n => !existingKeys.Any(e => e.BookId == n.BookId && e.Chapter == n.Chapter))
            .ToList();

        if (newNotes.Count > 0)
        {
            await db.BibleStudyNotes.AddRangeAsync(newNotes);
            await db.SaveChangesAsync();
        }
    }

    // ── Theology ──────────────────────────────────────────────────────────────
    // Usa UPSERT por Title: insere novos cursos e atualiza ExternalUrl/Provider dos existentes.
    // Os módulos/quizzes só são inseridos quando o curso é novo (cascade na criação).

    private static async Task SeedTheologyAsync(AppDbContext db)
    {
        var seeds = BuildTheologySeeds();

        foreach (var seed in seeds)
        {
            var existing = await db.TheologyCourses
                .FirstOrDefaultAsync(c => c.Title == seed.Title);

            if (existing == null)
            {
                await db.TheologyCourses.AddAsync(seed);
            }
            else
            {
                // Atualiza apenas os campos novos — não toca em módulos/quizzes já seedados
                existing.ExternalUrl = seed.ExternalUrl;
                existing.Provider = seed.Provider;
            }
        }

        await db.SaveChangesAsync();
    }

    private static List<TheologyCourse> BuildTheologySeeds()
    {
        return new List<TheologyCourse>
        {
        // Curso 1: Introdução à Teologia Sistemática
        new TheologyCourse
        {
            Title = "Introdução à Teologia Sistemática",
            Description = "Uma visão geral das doutrinas fundamentais da fé cristã, organizadas de forma sistemática.",
            Category = "Teologia Sistemática",
            DurationHours = 20,
            Level = "Básico",
            ImageIcon = "school",
            ExternalUrl = "https://www.biblicaltraining.org/systematic-theology/wayne-grudem",
            Provider = "BiblicalTraining.org",
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
        },

        // Curso 2: Cristologia
        new TheologyCourse
        {
            Title = "Cristologia: A Pessoa e a Obra de Cristo",
            Description = "Estudo aprofundado sobre quem Jesus é (sua pessoa) e o que ele fez (sua obra redentora).",
            Category = "Cristologia",
            DurationHours = 15,
            Level = "Intermediário",
            ImageIcon = "cross",
            ExternalUrl = "https://www.ligonier.org/learn/series/foundations-an-overview-of-systematic-theology",
            Provider = "Ligonier Ministries",
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
        },

        // Curso 3: Hermenêutica Bíblica
        new TheologyCourse
        {
            Title = "Hermenêutica: Como Interpretar a Bíblia",
            Description = "Princípios e métodos para uma interpretação sólida e responsável das Escrituras.",
            Category = "Hermenêutica",
            DurationHours = 12,
            Level = "Básico",
            ImageIcon = "book",
            ExternalUrl = "https://www.biblicaltraining.org/biblical-hermeneutics/robert-stein",
            Provider = "BiblicalTraining.org",
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
        },

        // Curso 4: Teologia do Antigo Testamento
        new TheologyCourse
        {
            Title = "Teologia do Antigo Testamento",
            Description = "Estudo das grandes doutrinas e temas teológicos que perpassam o Antigo Testamento, do Gênesis ao Malaquias.",
            Category = "Teologia Bíblica",
            DurationHours = 18,
            Level = "Intermediário",
            ImageIcon = "history_edu",
            ExternalUrl = "https://www.biblicaltraining.org/old-testament-theology/john-oswalt",
            Provider = "BiblicalTraining.org",
            Modules = new List<TheologyModule>
            {
                new()
                {
                    OrderIndex = 1,
                    Title = "A Aliança como Estrutura do AT",
                    Content = "O conceito de aliança (berith) é o fio condutor do Antigo Testamento. Deus estabelece relações de aliança com Noé (Gn 9), Abraão (Gn 15, 17), Moisés/Israel (Êx 19-24), Davi (2 Sm 7) e anuncia uma Nova Aliança (Jr 31:31-34). Cada aliança revela progressivamente o caráter de Deus e seu plano redentor. A aliança abraâmica promete bênção universal; a mosaica revela a santidade divina e a necessidade de expiação; a davídica aponta para o Rei-Messias eterno; a nova aliança promete transformação interior pelo Espírito. O NT vê todas estas alianças cumpridas em Cristo.",
                    References = "Gênesis 15; Êxodo 19-24; 2 Samuel 7; Jeremias 31:31-34; Hebreus 8-9",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual profeta anunciou explicitamente a 'Nova Aliança' que seria escrita no coração?",
                            OptionA = "Isaías",
                            OptionB = "Ezequiel",
                            OptionC = "Jeremias",
                            OptionD = "Daniel",
                            CorrectAnswer = "C",
                            Explanation = "Jeremias 31:31-34 contém a promessa da Nova Aliança: 'Porei a minha lei no seu interior e a escreverei no seu coração.' Esta é a única ocorrência exata da expressão 'Nova Aliança' no AT, citada em Hebreus 8:8-12."
                        }
                    }
                },
                new()
                {
                    OrderIndex = 2,
                    Title = "Os Profetas: Mensagem e Missão",
                    Content = "Os profetas do AT eram porta-vozes de Deus (nabi = chamado, porta-voz). Sua missão era: (1) Forthtelling — proclamar a Palavra de Deus para o presente (chamada ao arrependimento, denúncia da injustiça social, afirmação da soberania divina); (2) Foretelling — revelar eventos futuros, especialmente o Messias. Isaías profetizou o Servo Sofredor (cap. 53), nascimento virginal (7:14) e o reino eterno (9:6-7). Ezequiel viu a glória divina partindo do templo e retornando. Daniel recebeu visões apocalípticas. Os Profetas Menores (Oséias a Malaquias) cobriam Israel, Judá e nações vizinhas.",
                    References = "Isaías 1:1-20; Isaías 53; Oséias 1-3; Amós 5:21-24; Malaquias 3:1",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual é a diferença principal entre 'forthtelling' e 'foretelling' na profecia bíblica?",
                            OptionA = "Forthtelling prediz o futuro; foretelling fala ao presente",
                            OptionB = "Forthtelling é profecia oral; foretelling é profecia escrita",
                            OptionC = "Forthtelling proclama ao presente; foretelling revela o futuro",
                            OptionD = "Não há diferença — são sinônimos",
                            CorrectAnswer = "C",
                            Explanation = "Forthtelling (proclamar para frente) é a função primária: o profeta fala a Palavra de Deus para sua geração — chamando ao arrependimento, denunciando injustiça. Foretelling (predizer) é secundário: Deus revela eventos futuros para confirmar sua soberania e preparar seu povo."
                        }
                    }
                }
            }
        },

        // Curso 5: Teologia do Novo Testamento
        new TheologyCourse
        {
            Title = "Teologia do Novo Testamento",
            Description = "Os grandes temas teológicos do NT: o Reino de Deus, a cristologia paulina, a eclesiologia e a escatologia.",
            Category = "Teologia Bíblica",
            DurationHours = 16,
            Level = "Intermediário",
            ImageIcon = "auto_stories",
            ExternalUrl = "https://www.biblicaltraining.org/new-testament-theology/thomas-schreiner",
            Provider = "BiblicalTraining.org",
            Modules = new List<TheologyModule>
            {
                new()
                {
                    OrderIndex = 1,
                    Title = "O Reino de Deus nos Evangelhos",
                    Content = "O Reino de Deus (basileia tou Theou) é o tema central da pregação de Jesus. Não é apenas um lugar, mas o reinado soberano de Deus sobre toda a criação. Jesus anuncia: 'O tempo está cumprido e o Reino de Deus está próximo' (Mc 1:15). O Reino tem dois aspectos: (1) Já: inaugurado na vinda de Cristo — curas, exorcismos, perdão de pecados são sinais do Reino presente; (2) Ainda não: a consumação futura quando Cristo voltar, toda injustiça cessar e Deus habitar entre seu povo (Ap 21). Esta tensão 'já-mas-ainda-não' é fundamental para compreender a ética cristã, a missão da Igreja e a esperança escatológica.",
                    References = "Marcos 1:14-15; Mateus 5-7; Lucas 17:20-21; 1 Coríntios 15:20-28",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "O que significa a tensão 'já-mas-ainda-não' no ensino sobre o Reino de Deus?",
                            OptionA = "O Reino virá no futuro, mas ainda não chegou",
                            OptionB = "O Reino foi inaugurado por Cristo mas aguarda consumação futura",
                            OptionC = "O Reino é puramente espiritual e não tem dimensão futura",
                            OptionD = "O Reino é puramente futuro e não tem impacto no presente",
                            CorrectAnswer = "B",
                            Explanation = "A tensão escatológica 'já-mas-ainda-não' (inaugurated eschatology) significa que o Reino foi inaugurado na vinda de Cristo (curas, ressurreição, Pentecostes = 'já'), mas aguarda consumação na Sua volta (fim da morte, do pecado, nova criação = 'ainda não')."
                        }
                    }
                },
                new()
                {
                    OrderIndex = 2,
                    Title = "A Teologia de Paulo: Graça, Fé e Justificação",
                    Content = "Paulo é o maior teólogo do NT. Seus temas centrais: (1) Justificação pela Fé — somos declarados justos diante de Deus não por obras, mas pela fé em Cristo (Rm 3:21-26; Gl 2:16); (2) União com Cristo — o crente está 'em Cristo' (en Christō, 164x em Paulo), morrendo e ressuscitando com Ele (Rm 6:1-11); (3) O Espírito Santo — agente da vida nova, que intercede, santifica e garante a herança (Rm 8); (4) Israel e as Nações — o mistério da inclusão dos gentios (Ef 2:11-22; Rm 9-11); (5) A Igreja como Corpo de Cristo — diversidade de dons, unidade no Espírito (1 Co 12; Ef 4). Paulo responde à questão: como um pecador pode ser aceito por um Deus santo?",
                    References = "Romanos 3:21-26; Gálatas 2:16; Efésios 2:8-9; Filipenses 3:9",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Em quantas ocorrências aproximadas Paulo usa a expressão 'em Cristo' (en Christō) em suas epístolas?",
                            OptionA = "Cerca de 20 vezes",
                            OptionB = "Cerca de 60 vezes",
                            OptionC = "Cerca de 164 vezes",
                            OptionD = "Cerca de 300 vezes",
                            CorrectAnswer = "C",
                            Explanation = "'Em Cristo' (en Christō / en Kyriō) aparece aproximadamente 164 vezes nas cartas paulinas, tornando a 'união com Cristo' a categoria mais frequente e fundamental da teologia de Paulo — mais central que 'justificação pela fé'."
                        }
                    }
                }
            }
        },

        // Curso 6: Apologética Cristã
        new TheologyCourse
        {
            Title = "Apologética Cristã",
            Description = "Defesa racional da fé cristã diante de objeções filosóficas, científicas e históricas. Como dar razão da esperança.",
            Category = "Apologética",
            DurationHours = 14,
            Level = "Avançado",
            ImageIcon = "shield",
            ExternalUrl = "https://www.ligonier.org/learn/series/defending-your-faith",
            Provider = "Ligonier Ministries",
            Modules = new List<TheologyModule>
            {
                new()
                {
                    OrderIndex = 1,
                    Title = "Fundamentos da Apologética",
                    Content = "Apologética (do grego apologia — defesa) é a disciplina que defende a razoabilidade da fé cristã. 1 Pedro 3:15 ordena: 'Estai sempre preparados para responder com mansidão e temor a qualquer que vos pedir razão da esperança.' As principais escolas apologéticas são: (1) Evidencialismo (Habermas, Montgomery) — argumenta a partir de evidências históricas, especialmente a ressurreição; (2) Apologética Clássica (Sproul, Kreeft) — primeiro estabelece theísmo via argumentos filosóficos, depois especificamente a fé cristã; (3) Pressuposicionalismo (Van Til, Bahnsen) — toda evidência é interpretada dentro de cosmovisões; a cosmovisão cristã é pressuposta como única que torna o conhecimento possível; (4) Apologética Cumulativa (Cumming) — convergência de múltiplas linhas de evidência. C. S. Lewis usou uma abordagem narrativa/literária magistral em 'Mero Cristianismo'.",
                    References = "1 Pedro 3:15; Atos 17:16-34; Romanos 1:18-20; Mero Cristianismo — C. S. Lewis",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual escola apologética argumenta que toda evidência é interpretada dentro de cosmovisões, e que a cristã é a única internamente coerente?",
                            OptionA = "Evidencialismo",
                            OptionB = "Apologética Clássica",
                            OptionC = "Pressuposicionalismo",
                            OptionD = "Apologética Cumulativa",
                            CorrectAnswer = "C",
                            Explanation = "O Pressuposicionalismo, desenvolvido por Cornelius Van Til e Gordon Bahnsen, ensina que o crente deve pressupor a verdade cristã como fundamento do conhecimento. Só a cosmovisão bíblica pode justificar a lógica, a ciência e os valores morais — as outras cosmovisões 'pegam carona' na visão cristã de mundo."
                        }
                    }
                }
            }
        },

        // Curso 7: As Cinco Solas da Reforma
        new TheologyCourse
        {
            Title = "As Cinco Solas da Reforma Protestante",
            Description = "Os cinco pilares doutrinais que definiram a Reforma do séc. XVI e que continuam como fundamentos da fé evangélica.",
            Category = "Teologia Histórica",
            DurationHours = 10,
            Level = "Básico",
            ImageIcon = "menu_book",
            ExternalUrl = "https://www.ligonier.org/learn/series/luther-and-the-reformation",
            Provider = "Ligonier Ministries",
            Modules = new List<TheologyModule>
            {
                new()
                {
                    OrderIndex = 1,
                    Title = "Sola Scriptura e Sola Fide",
                    Content = "As Cinco Solas sintetizam a teologia da Reforma: (1) Sola Scriptura — somente a Escritura é a autoridade suprema e infalível para fé e prática; rejeita a tradição como autoridade co-igual (contra Roma). Lutero em Worms (1521): 'Aqui estou, não posso fazer de outro modo.' (2) Sola Fide — somente pela fé somos justificados; a fé é o instrumento que nos une a Cristo, não as obras (Rm 3:28; Gl 2:16). Lutero chamou esta doutrina de 'articulus stantis et cadentis ecclesiae' — o artigo pelo qual a Igreja se sustenta ou cai. (3) Sola Gratia — somente pela graça, não por mérito humano, somos salvos (Ef 2:8-9). (4) Solus Christus — somente Cristo é mediador entre Deus e os homens (1 Tm 2:5). (5) Soli Deo Gloria — toda a glória pertence somente a Deus, não ao homem.",
                    References = "Romanos 3:28; Gálatas 2:16; Efésios 2:8-9; 1 Timóteo 2:5; 2 Timóteo 3:16",
                    Quizzes = new List<TheologyQuiz>
                    {
                        new()
                        {
                            Question = "Qual das Cinco Solas Lutero chamou de 'artigo pelo qual a Igreja se sustenta ou cai'?",
                            OptionA = "Sola Scriptura",
                            OptionB = "Sola Gratia",
                            OptionC = "Sola Fide",
                            OptionD = "Solus Christus",
                            CorrectAnswer = "C",
                            Explanation = "Lutero chamou a Sola Fide (justificação somente pela fé) de 'articulus stantis et cadentis ecclesiae' — o artigo pelo qual a Igreja se sustenta ou cai. Esta doutrina era o coração da Reforma porque confrontava diretamente o sistema de méritos e indulgências da Igreja Romana."
                        }
                    }
                }
            }
        },
        }; // fim da lista
    }

    private static async Task SeedChurchHistoryAsync(AppDbContext db)
    {
        // Upsert por Name: insere apenas heróis ainda não existentes.
        var existingNames = await db.ChurchHeroes.Select(h => h.Name).ToListAsync();

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
            },
            new()
            {
                Name = "Agostinho de Hipona",
                Period = "Patrística",
                BirthYear = 354,
                DeathYear = 430,
                Nationality = "Norte-Africano (Argélia atual)",
                Category = "Theologian",
                Biography = "Filho de Mônica (cristã devotada) e Patrício (pagão), Agostinho levou uma juventude de buscas intelectuais e morais turbulentas — maniqueu por 9 anos, depois neoplatônico. Convertido em 386 ao ouvir uma voz infantil repetir 'Tolle, lege' (Pega, lê) e ler Romanos 13:13-14 num jardim em Milão. Ordenado em 391, bispo de Hipona em 395. Suas 'Confissões' são a primeira autobiografia espiritual da literatura ocidental. Combateu o maniqueísmo, o donatismo e o pelagianismo, moldando a teologia ocidental por 1.600 anos.",
                KeyContributions = "Doutrina da graça soberana (contra Pelágio); teoria da guerra justa; filosofia da história (A Cidade de Deus); doutrina do pecado original; teologia trinitária (De Trinitate); influência decisiva sobre Lutero, Calvino e a Reforma.",
                FavoriteVerse = "Romanos 13:14 — 'Revesti-vos do Senhor Jesus Cristo.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "João Crisóstomo",
                Period = "Patrística",
                BirthYear = 347,
                DeathYear = 407,
                Nationality = "Grego (Antioquia)",
                Category = "Theologian",
                Biography = "Crisóstomo ('boca de ouro') é o maior pregador da Igreja Grega. Nascido em Antioquia, estudou retórica com o famoso Libânio. Após anos de vida ascética, foi ordenado diácono (381) e presbítero (386). Seus sermões expositivos em Antioquia — sobre Mateus, João, as Epístolas de Paulo — definem o modelo da pregação expositiva por 1.700 anos. Tornou-se Arcebispo de Constantinopla (398) e foi exilado duas vezes por denunciar a corrupção da corte imperial, especialmente da imperatriz Eudóxia.",
                KeyContributions = "Padrão da pregação expositiva; comentários sobre todas as epístolas de Paulo; teologia da Eucaristia; defesa dos pobres e denúncia da riqueza ímpia; influência na liturgia ortodoxa oriental.",
                FavoriteVerse = "Filipenses 4:4 — 'Alegrai-vos sempre no Senhor; outra vez digo, alegrai-vos.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "Anselmo de Cantuária",
                Period = "Escolástica Medieval",
                BirthYear = 1033,
                DeathYear = 1109,
                Nationality = "Italiano (naturalizado inglês)",
                Category = "Theologian",
                Biography = "Monge beneditino e arcebispo de Cantuária, Anselmo é chamado 'o pai da Escolástica'. Seu método: 'fides quaerens intellectum' (fé em busca de entendimento) — a fé precede a razão, mas a razão pode explorar e defender a fé. O Proslogion contém o argumento ontológico para a existência de Deus. O Cur Deus Homo ('Por que Deus se fez homem?') é a formulação mais influente da teoria da satisfação da expiação: o pecado ofende infinitamente a honra de Deus; apenas um ser ao mesmo tempo humano e divino pode restaurar essa honra.",
                KeyContributions = "Argumento ontológico; teoria da satisfação (expiação); método escolástico; defesa da independência da Igreja ante o Estado; influência sobre Tomás de Aquino e toda a teologia medieval.",
                FavoriteVerse = "Salmos 27:8 — 'O meu coração te diz: Busca o meu rosto.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "Amy Carmichael",
                Period = "Missões Modernas",
                BirthYear = 1867,
                DeathYear = 1951,
                Nationality = "Irlandesa",
                Category = "Missionary",
                Biography = "Missionária na Índia por 55 anos sem nunca retornar à Irlanda, Amy Carmichael fundou a Dohnavur Fellowship no Tamil Nadu para resgatar crianças de templos hindus onde eram usadas em prostituição ritual. Adotou roupas e costumes indianos, pioneira na contextualização missionária. Em 1931 sofreu uma queda que a deixou acamada pelos últimos 20 anos de vida — desse quarto escreveu livros que transformaram gerações de missionários.",
                KeyContributions = "Dohnavur Fellowship; resgate de crianças exploradas; livros devocionais (If, Gold by Moonlight, His Thoughts Said); modelo de missão contextualizada; teologia do sofrimento.",
                FavoriteVerse = "Gálatas 2:20 — 'Já estou crucificado com Cristo; e vivo, não mais eu, mas Cristo vive em mim.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "C.S. Lewis",
                Period = "Séc. XX — Apologética",
                BirthYear = 1898,
                DeathYear = 1963,
                Nationality = "Irlandês-Britânico",
                Category = "Apologist",
                Biography = "Clive Staples Lewis era professor de literatura em Oxford e Cambridge e ateu convicto até os 32 anos, quando foi convertido ao teísmo (1929) e ao cristianismo (1931) por meio de conversas com J.R.R. Tolkien e Hugo Dyson. Após a conversão, tornou-se o apologista cristão mais popular do século XX, alcançando pessoas fora da Igreja que a teologia acadêmica nunca atingiria.",
                KeyContributions = "Mere Christianity; The Problem of Pain; The Screwtape Letters; As Crônicas de Nárnia; argumento trilemático (louco, mentiroso ou Senhor); argumento do desejo ('Joy') como sinal do transcendente.",
                FavoriteVerse = "João 14:6 — 'Eu sou o caminho, a verdade e a vida.'",
                ImageUrl = ""
            },
            new()
            {
                Name = "Bakht Singh",
                Period = "Séc. XX — Avivamento Asiático",
                BirthYear = 1903,
                DeathYear = 2000,
                Nationality = "Indiano (Punjab)",
                Category = "Evangelist",
                Biography = "Nascido em família Sikh abastada no Punjab, Bakht Singh partiu para a Inglaterra em 1926 para estudar Engenharia, tornando-se ateu e abraçando os vícios ocidentais. Sua conversão ocorreu em 11 de agosto de 1928, a bordo de um transatlântico rumo ao Canadá, ao participar de um culto e sentir um poder divino envolvê-lo. Em 1929, lendo o Evangelho de João de joelhos num quarto nevado em Winnipeg, ouviu a voz de Cristo confrontá-lo com seus pecados e recebeu a segurança do perdão pelo sangue de Jesus. Batizado em Vancouver em 1932, renunciou à carreira de engenheiro e retornou à Índia em 1933. Tornou-se um dos evangelistas mais poderosos do subcontinente, usado por Deus em grandes avivamentos — incluindo o notável avivamento de 1937 em Martinpur. Em 1941 fundou a primeira igreja 'Jehovah Shammah' em Madras, baseada nos princípios de Atos 2:42, estabelecendo um modelo de igreja neotestamentária que cresceu para centenas de assembléias. Suas Santas Convocações anuais em Hyderabad reuniam até 25.000 pessoas. Ministrou também na Europa, Austrália, América do Norte e África. Comparado por J. Edwin Orr aos maiores evangelistas ocidentais como Finney e Moody, seu conhecimento bíblico impressionou figuras como Martyn Lloyd-Jones. Sofreu da doença de Parkinson nos últimos dez anos de vida. Em seu funeral, mais de 250.000 pessoas marcharam cantando, segurando suas Bíblias no alto.",
                KeyContributions = "Avivamento de 1937 em Martinpur (Punjab); fundação da rede de igrejas Jehovah Shammah baseadas em Atos 2:42; centro de treinamento Hebrom em Hyderabad; Santas Convocações anuais; indigenização do evangelho sem dependência de missões estrangeiras; princípios de vida por fé sem solicitar apoio humano.",
                FavoriteVerse = "Atos 2:42 — 'E perseveravam na doutrina dos apóstolos, na comunhão uns com outros, no partir do pão e nas orações.'",
                ImageUrl = ""
            },
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

        var newHeroes = heroes.Where(h => !existingNames.Contains(h.Name)).ToList();
        if (newHeroes.Count > 0)
        {
            await db.ChurchHeroes.AddRangeAsync(newHeroes);
        }

        if (!await db.Revivals.AnyAsync())
        {
            await db.Revivals.AddRangeAsync(revivals);
        }

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

    // ── Reading Plans ─────────────────────────────────────────────────────────

    private static async Task SeedReadingPlansAsync(AppDbContext db)
    {
        if (await db.ReadingPlans.AnyAsync())
            return;

        var plans = new List<ReadingPlan>
        {
            new()
            {
                Name        = "Bíblia em 1 Ano",
                Description = "Percorra os 66 livros da Bíblia em 365 dias com leituras diárias balanceadas do Antigo e Novo Testamento.",
                TotalDays   = 365,
                Strategy    = "full_bible",
                Icon        = "📅"
            },
            new()
            {
                Name        = "Novo Testamento em 90 Dias",
                Description = "Leia os 27 livros do Novo Testamento em 3 meses. Ideal para aprofundar no ensino de Cristo e dos apóstolos.",
                TotalDays   = 90,
                Strategy    = "new_testament",
                Icon        = "✝️"
            },
            new()
            {
                Name        = "Evangelhos em 30 Dias",
                Description = "Mergulhe nos quatro Evangelhos — Mateus, Marcos, Lucas e João — em 30 dias de leitura focada na vida de Jesus.",
                TotalDays   = 30,
                Strategy    = "gospels",
                Icon        = "🕊️"
            }
        };

        await db.ReadingPlans.AddRangeAsync(plans);
        await db.SaveChangesAsync();
    }
}