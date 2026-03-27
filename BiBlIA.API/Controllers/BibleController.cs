using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Controllers;

/// <summary>
/// Navegação hierárquica da Bíblia: livros → capítulos → versículos.
/// Endpoints somente-leitura, otimizados para o fluxo do leitor.
/// </summary>
[ApiController]
[Route("api/bible")]
public class BibleController : ControllerBase
{
    private readonly AppDbContext _context;

    public BibleController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/bible/books
    // GET /api/bible/books?testament=OT  (filtra por testamento)
    [HttpGet("books")]
    public async Task<ActionResult<IEnumerable<BibleBookDto>>> GetBooks([FromQuery] string? testament)
    {
        var query = _context.BibleBooks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(testament))
            query = query.Where(b => b.Testament == testament.ToUpperInvariant());

        var books = await query
            .OrderBy(b => b.OrderIndex)
            .ToListAsync();

        return Ok(books.Select(b => new BibleBookDto
        {
            Id = b.Id,
            Name = b.Name,
            Abbreviation = b.Abbreviation,
            Testament = b.Testament,
            ChapterCount = b.ChapterCount,
            Description = b.Description,
            OrderIndex = b.OrderIndex
        }));
    }

    // GET /api/bible/books/{bookId}
    [HttpGet("books/{bookId}")]
    public async Task<ActionResult<BibleBookDto>> GetBook(int bookId)
    {
        var book = await _context.BibleBooks.FindAsync(bookId);

        if (book == null)
            return NotFound();

        return Ok(new BibleBookDto
        {
            Id = book.Id,
            Name = book.Name,
            Abbreviation = book.Abbreviation,
            Testament = book.Testament,
            ChapterCount = book.ChapterCount,
            Description = book.Description,
            OrderIndex = book.OrderIndex
        });
    }

    // GET /api/bible/books/{bookId}/chapters
    // Retorna a lista de números de capítulos com contagem de versículos
    [HttpGet("books/{bookId}/chapters")]
    public async Task<ActionResult<IEnumerable<ChapterSummaryDto>>> GetChapters(int bookId)
    {
        var bookExists = await _context.BibleBooks.AnyAsync(b => b.Id == bookId);
        if (!bookExists)
            return NotFound();

        // EF Core não consegue traduzir construtores posicionais dentro de GroupBy.
        // Projetamos para tipo anônimo (traduzível) e mapeamos no C# depois.
        var raw = await _context.BibleVerses
            .Where(v => v.BookId == bookId)
            .GroupBy(v => v.Chapter)
            .Select(g => new { Chapter = g.Key, VerseCount = g.Count() })
            .OrderBy(c => c.Chapter)
            .ToListAsync();

        return Ok(raw.Select(c => new ChapterSummaryDto(c.Chapter, c.VerseCount)));
    }

    // GET /api/bible/books/{bookId}/chapters/{chapter}
    // Retorna todos os versículos de um capítulo
    [HttpGet("books/{bookId}/chapters/{chapter}")]
    public async Task<ActionResult<IEnumerable<BibleVerseDto>>> GetChapter(int bookId, int chapter)
    {
        var book = await _context.BibleBooks.FindAsync(bookId);
        if (book == null)
            return NotFound("Livro não encontrado.");

        if (chapter < 1 || chapter > book.ChapterCount)
            return BadRequest($"Capítulo {chapter} não existe em {book.Name} (1–{book.ChapterCount}).");

        var verses = await _context.BibleVerses
            .Where(v => v.BookId == bookId && v.Chapter == chapter)
            .OrderBy(v => v.Verse)
            .ToListAsync();

        return Ok(verses.Select(v => new BibleVerseDto
        {
            Id       = v.Id,
            BookId   = v.BookId,
            BookName = book.Name,
            Chapter  = v.Chapter,
            Verse    = v.Verse,
            TextKJV  = v.TextKJV,
            TextAA   = v.TextAA,
            TextACF  = v.TextACF,
            TextNVI  = v.TextNVI
        }));
    }

    // POST /api/bible/import
    // Upsert em lote: insere versículos novos e atualiza traduções dos já existentes.
    // Recebe até 2000 versículos por requisição; scripts externos chamam em batches.
    [HttpPost("import")]
    public async Task<ActionResult<ImportResultDto>> ImportVerses([FromBody] List<ImportVerseDto> verses)
    {
        if (verses == null || verses.Count == 0)
            return BadRequest("Nenhum versículo fornecido.");

        // orderIndex → bookId  (lookup O(1))
        var books = (await _context.BibleBooks
            .Select(b => new { b.OrderIndex, b.Id })
            .ToListAsync())
            .ToDictionary(b => b.OrderIndex, b => b.Id);

        // Carrega entidades existentes indexadas por (bookId, chapter, verse)
        // para poder atualizar os campos de tradução sem deletar e reinserir.
        var existing = (await _context.BibleVerses.ToListAsync())
            .ToDictionary(v => (v.BookId, v.Chapter, v.Verse));

        var toInsert  = new List<BíblIA.Api.Models.BibleVerse>(verses.Count);
        int updated   = 0;
        int skipped   = 0;

        foreach (var dto in verses)
        {
            if (!books.TryGetValue(dto.BookOrderIndex, out var bookId))
            {
                skipped++;
                continue;
            }

            var key = (bookId, dto.Chapter, dto.Verse);

            if (existing.TryGetValue(key, out var entity))
            {
                // Versículo já existe — atualiza apenas as traduções não-vazias
                // para preservar dados de importações anteriores.
                if (!string.IsNullOrEmpty(dto.TextKJV)) entity.TextKJV = dto.TextKJV;
                if (!string.IsNullOrEmpty(dto.TextAA))  entity.TextAA  = dto.TextAA;
                if (!string.IsNullOrEmpty(dto.TextACF)) entity.TextACF = dto.TextACF;
                if (!string.IsNullOrEmpty(dto.TextNVI)) entity.TextNVI = dto.TextNVI;
                updated++;
            }
            else
            {
                var novo = new BíblIA.Api.Models.BibleVerse
                {
                    BookId  = bookId,
                    Chapter = dto.Chapter,
                    Verse   = dto.Verse,
                    TextKJV = dto.TextKJV,
                    TextAA  = dto.TextAA,
                    TextACF = dto.TextACF,
                    TextNVI = dto.TextNVI
                };
                toInsert.Add(novo);
                // Previne duplicata dentro do mesmo lote
                existing[key] = novo;
            }
        }

        if (toInsert.Count > 0)
            await _context.BibleVerses.AddRangeAsync(toInsert);

        // SaveChanges persiste tanto os inserts quanto as entidades modificadas
        if (toInsert.Count > 0 || updated > 0)
            await _context.SaveChangesAsync();

        return Ok(new ImportResultDto
        {
            Total    = verses.Count,
            Imported = toInsert.Count + updated,
            Skipped  = skipped
        });
    }

    // GET /api/bible/books/{bookId}/chapters/{chapter}/note
    // Retorna a nota de estudo do capítulo (404 se não houver nota)
    [HttpGet("books/{bookId}/chapters/{chapter}/note")]
    public async Task<ActionResult<BibleStudyNoteDto>> GetChapterNote(int bookId, int chapter)
    {
        var note = await _context.BibleStudyNotes
            .FirstOrDefaultAsync(n => n.BookId == bookId && n.Chapter == chapter);

        if (note == null)
            return NotFound();

        return Ok(new BibleStudyNoteDto
        {
            Id                      = note.Id,
            BookId                  = note.BookId,
            Chapter                 = note.Chapter,
            Title                   = note.Title,
            Context                 = note.Context,
            TheologicalSignificance = note.TheologicalSignificance,
            KeyThemes               = note.KeyThemes,
            CrossReferences         = note.CrossReferences,
            Commentary              = note.Commentary,
            AuthorNote              = note.AuthorNote
        });
    }

    // GET /api/bible/books/{bookId}/chapters/{chapter}/verses/{verse}
    [HttpGet("books/{bookId}/chapters/{chapter}/verses/{verse}")]
    public async Task<ActionResult<BibleVerseDto>> GetVerse(int bookId, int chapter, int verse)
    {
        var book = await _context.BibleBooks.FindAsync(bookId);
        if (book == null)
            return NotFound("Livro não encontrado.");

        var v = await _context.BibleVerses
            .FirstOrDefaultAsync(v => v.BookId == bookId && v.Chapter == chapter && v.Verse == verse);

        if (v == null)
            return NotFound($"{book.Abbreviation} {chapter}:{verse} não encontrado.");

        return Ok(new BibleVerseDto
        {
            Id       = v.Id,
            BookId   = v.BookId,
            BookName = book.Name,
            Chapter  = v.Chapter,
            Verse    = v.Verse,
            TextKJV  = v.TextKJV,
            TextAA   = v.TextAA,
            TextACF  = v.TextACF,
            TextNVI  = v.TextNVI
        });
    }

    // GET /api/bible/search?query=...&limit=10
    // Busca full-text nos campos textKJV e textACF. Retorna até `limit` resultados (padrão 10, máx 50).
    // Usado pelo chat para sugestão de referências cruzadas.
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BibleVerseDto>>> Search(
        [FromQuery] string query,
        [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            return BadRequest("Query deve ter pelo menos 3 caracteres.");

        limit = Math.Clamp(limit, 1, 50);
        var term = query.ToLower();

        // Busca em todas as traduções disponíveis
        var verses = await _context.BibleVerses
            .Where(v => v.TextKJV.ToLower().Contains(term)
                     || v.TextAA.ToLower().Contains(term)
                     || v.TextACF.ToLower().Contains(term)
                     || v.TextNVI.ToLower().Contains(term))
            .OrderBy(v => v.BookId).ThenBy(v => v.Chapter).ThenBy(v => v.Verse)
            .Take(limit)
            .Join(_context.BibleBooks,
                  v => v.BookId,
                  b => b.Id,
                  (v, b) => new BibleVerseDto
                  {
                      Id       = v.Id,
                      BookId   = v.BookId,
                      BookName = b.Name,
                      Chapter  = v.Chapter,
                      Verse    = v.Verse,
                      TextKJV  = v.TextKJV,
                      TextAA   = v.TextAA,
                      TextACF  = v.TextACF,
                      TextNVI  = v.TextNVI
                  })
            .ToListAsync();

        return Ok(verses);
    }

    // GET /api/bible/verse-of-day
    // Retorna um versículo determinístico para o dia atual.
    // A seleção usa (ano * 366 + diaDoAno) % total para variar todo dia
    // e ciclar por todos os 31k versículos ao longo dos anos — sem randomness,
    // sem estado: qualquer instância do servidor retorna o mesmo versículo no mesmo dia.
    [HttpGet("verse-of-day")]
    public async Task<ActionResult<BibleVerseDto>> GetVerseOfDay()
    {
        var total = await _context.BibleVerses.CountAsync();
        if (total == 0)
            return NotFound("Nenhum versículo importado.");

        var today = DateTime.UtcNow;
        var dayIndex = (today.Year * 366 + today.DayOfYear) % total;

        var verse = await _context.BibleVerses
            .OrderBy(v => v.Id)
            .Skip(dayIndex)
            .Take(1)
            .Join(_context.BibleBooks,
                  v => v.BookId,
                  b => b.Id,
                  (v, b) => new BibleVerseDto
                  {
                      Id       = v.Id,
                      BookId   = v.BookId,
                      BookName = b.Name,
                      Chapter  = v.Chapter,
                      Verse    = v.Verse,
                      TextKJV  = v.TextKJV,
                      TextAA   = v.TextAA,
                      TextACF  = v.TextACF,
                      TextNVI  = v.TextNVI
                  })
            .FirstOrDefaultAsync();

        return verse is null ? NotFound() : Ok(verse);
    }
}

// DTO auxiliar — representa o resumo de um capítulo
public record ChapterSummaryDto(int Chapter, int VerseCount);
