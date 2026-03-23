using BíblIA.Api.Data;
using BíblIA.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedBibleBooksAsync(db);
    }

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
            new() { OrderIndex = 58, Name = "Hebrews",        Abbreviation = "Heb",  Testament = "NT", ChapterCount =  13, Description = "A superioridade de Cristo sobre os ângulos, Moisés e o sacerdócio levítico." },
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
}
