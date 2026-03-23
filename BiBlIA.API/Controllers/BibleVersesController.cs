using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using BíblIA.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BibleVersesController : ControllerBase
{
    private readonly AppDbContext _context;

    public BibleVersesController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/bibleverses?bookId=1&chapter=3
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BibleVerseDto>>> GetVerses(
        [FromQuery] int? bookId,
        [FromQuery] int? chapter)
    {
        var query = _context.BibleVerses
            .Include(v => v.Book)
            .AsQueryable();

        if (bookId.HasValue)
            query = query.Where(v => v.BookId == bookId.Value);

        if (chapter.HasValue)
            query = query.Where(v => v.Chapter == chapter.Value);

        var verses = await query
            .OrderBy(v => v.BookId)
            .ThenBy(v => v.Chapter)
            .ThenBy(v => v.Verse)
            .ToListAsync();

        return Ok(verses.Select(MapToDto));
    }

    // GET /api/bibleverses/search?bookId=1&chapter=3&verse=16
    [HttpGet("search")]
    public async Task<ActionResult<BibleVerseDto>> SearchVerse(
        [FromQuery] int bookId,
        [FromQuery] int chapter,
        [FromQuery] int verse)
    {
        var v = await _context.BibleVerses
            .Include(v => v.Book)
            .FirstOrDefaultAsync(v => v.BookId == bookId && v.Chapter == chapter && v.Verse == verse);

        if (v == null)
            return NotFound();

        return Ok(MapToDto(v));
    }

    // GET /api/bibleverses/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BibleVerseDto>> GetVerse(int id)
    {
        var verse = await _context.BibleVerses
            .Include(v => v.Book)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (verse == null)
            return NotFound();

        return Ok(MapToDto(verse));
    }

    // POST /api/bibleverses
    [HttpPost]
    public async Task<ActionResult<BibleVerseDto>> CreateVerse(CreateBibleVerseDto dto)
    {
        var bookExists = await _context.BibleBooks.AnyAsync(b => b.Id == dto.BookId);
        if (!bookExists)
            return BadRequest($"Livro com Id {dto.BookId} não encontrado.");

        var verse = new BibleVerse
        {
            BookId = dto.BookId,
            Chapter = dto.Chapter,
            Verse = dto.Verse,
            TextACF = dto.TextACF,
            TextKJV = dto.TextKJV,
            CreatedAt = DateTime.UtcNow
        };

        _context.BibleVerses.Add(verse);
        await _context.SaveChangesAsync();

        // Recarrega com o Book para mapear o DTO corretamente
        await _context.Entry(verse).Reference(v => v.Book).LoadAsync();

        return CreatedAtAction(nameof(GetVerse), new { id = verse.Id }, MapToDto(verse));
    }

    // DELETE /api/bibleverses/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVerse(int id)
    {
        var verse = await _context.BibleVerses.FindAsync(id);
        if (verse == null)
            return NotFound();

        _context.BibleVerses.Remove(verse);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static BibleVerseDto MapToDto(BibleVerse verse) => new()
    {
        Id = verse.Id,
        BookId = verse.BookId,
        BookName = verse.Book?.Name ?? string.Empty,
        Chapter = verse.Chapter,
        Verse = verse.Verse,
        TextACF = verse.TextACF,
        TextKJV = verse.TextKJV
    };
}
