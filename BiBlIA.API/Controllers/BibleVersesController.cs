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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BibleVerseDto>>> GetVerses()
    {
        var verses = await _context.BibleVerses.ToListAsync();
        return Ok(verses.Select(v => MapToDto(v)));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BibleVerseDto>>> SearchVerse([FromQuery] string book, [FromQuery] int chapter, [FromQuery] int verse)
    {
        var verses = await _context.BibleVerses
            .Where(v => v.Book == book && v.Chapter == chapter && v.Verse == verse)
            .ToListAsync();

        return Ok(verses.Select(v => MapToDto(v)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BibleVerseDto>> GetVerse(int id)
    {
        var verse = await _context.BibleVerses.FindAsync(id);
        if (verse == null)
            return NotFound();

        return Ok(MapToDto(verse));
    }

    [HttpPost]
    public async Task<ActionResult<BibleVerseDto>> CreateVerse(CreateBibleVerseDto dto)
    {
        var verse = new BibleVerse
        {
            Book = dto.Book,
            Chapter = dto.Chapter,
            Verse = dto.Verse,
            Text = dto.Text,
            Version = dto.Version,
            CreatedAt = DateTime.UtcNow
        };

        _context.BibleVerses.Add(verse);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVerse), new { id = verse.Id }, MapToDto(verse));
    }

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

    private static BibleVerseDto MapToDto(BibleVerse verse)
    {
        return new BibleVerseDto
        {
            Id = verse.Id,
            Book = verse.Book,
            Chapter = verse.Chapter,
            Verse = verse.Verse,
            Text = verse.Text,
            Version = verse.Version
        };
    }
}