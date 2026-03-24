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

        var chapters = await _context.BibleVerses
            .Where(v => v.BookId == bookId)
            .GroupBy(v => v.Chapter)
            .Select(g => new ChapterSummaryDto(g.Key, g.Count()))
            .OrderBy(c => c.Chapter)
            .ToListAsync();

        return Ok(chapters);
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
            Id = v.Id,
            BookId = v.BookId,
            BookName = book.Name,
            Chapter = v.Chapter,
            Verse = v.Verse,
            TextACF = v.TextACF,
            TextKJV = v.TextKJV
        }));
    }

    // POST /api/bible/import
    // Importação em lote — idempotente (pula versículos já existentes).
    // Recebe até 2000 versículos por requisição; scripts externos chamam em batches.
    [HttpPost("import")]
    public async Task<ActionResult<ImportResultDto>> ImportVerses([FromBody] List<ImportVerseDto> verses)
    {
        if (verses == null || verses.Count == 0)
            return BadRequest("Nenhum versículo fornecido.");

        // Monta dicionário orderIndex → bookId para lookup O(1)
        var books = (await _context.BibleBooks
            .Select(b => new { b.OrderIndex, b.Id })
            .ToListAsync())
            .ToDictionary(b => b.OrderIndex, b => b.Id);

        // Carrega chaves existentes como ValueTuple para equality correta
        var existingKeys = (await _context.BibleVerses
            .Select(v => new { v.BookId, v.Chapter, v.Verse })
            .ToListAsync())
            .Select(v => (v.BookId, v.Chapter, v.Verse))
            .ToHashSet();

        var toInsert = new List<BíblIA.Api.Models.BibleVerse>(verses.Count);
        int skipped = 0;

        foreach (var dto in verses)
        {
            if (!books.TryGetValue(dto.BookOrderIndex, out var bookId))
            {
                skipped++;
                continue;
            }

            if (existingKeys.Contains((bookId, dto.Chapter, dto.Verse)))
            {
                skipped++;
                continue;
            }

            toInsert.Add(new BíblIA.Api.Models.BibleVerse
            {
                BookId = bookId,
                Chapter = dto.Chapter,
                Verse = dto.Verse,
                TextACF = dto.TextACF,
                TextKJV = dto.TextKJV
            });

            // Adiciona ao set para evitar duplicatas dentro do mesmo lote
            existingKeys.Add((bookId, dto.Chapter, dto.Verse));
        }

        if (toInsert.Count > 0)
        {
            await _context.BibleVerses.AddRangeAsync(toInsert);
            await _context.SaveChangesAsync();
        }

        return Ok(new ImportResultDto
        {
            Total    = verses.Count,
            Imported = toInsert.Count,
            Skipped  = skipped
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
            Id = v.Id,
            BookId = v.BookId,
            BookName = book.Name,
            Chapter = v.Chapter,
            Verse = v.Verse,
            TextACF = v.TextACF,
            TextKJV = v.TextKJV
        });
    }
}

// DTO auxiliar — representa o resumo de um capítulo
public record ChapterSummaryDto(int Chapter, int VerseCount);
