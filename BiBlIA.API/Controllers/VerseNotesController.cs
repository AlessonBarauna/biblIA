using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using BíblIA.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BíblIA.Api.Controllers;

[ApiController]
[Route("api/verse-notes")]
[Authorize]
public class VerseNotesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VerseNotesController(AppDbContext context)
    {
        _context = context;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/verse-notes?bookId=1&chapter=3
    // Retorna todas as anotações do usuário, com filtro opcional por livro/capítulo.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VerseNoteDto>>> GetNotes(
        [FromQuery] int? bookId,
        [FromQuery] int? chapter)
    {
        var query = _context.VerseNotes
            .Where(n => n.UserId == CurrentUserId)
            .AsQueryable();

        if (bookId.HasValue)   query = query.Where(n => n.BookId  == bookId.Value);
        if (chapter.HasValue)  query = query.Where(n => n.Chapter == chapter.Value);

        // Join com BibleBooks para incluir bookName no DTO
        var notes = await query
            .Join(_context.BibleBooks,
                  n => n.BookId,
                  b => b.Id,
                  (n, b) => new VerseNoteDto
                  {
                      Id        = n.Id,
                      BookId    = n.BookId,
                      BookName  = b.Name,
                      Chapter   = n.Chapter,
                      Verse     = n.Verse,
                      Note      = n.Note,
                      UpdatedAt = n.UpdatedAt
                  })
            .OrderBy(n => n.BookId).ThenBy(n => n.Chapter).ThenBy(n => n.Verse)
            .ToListAsync();

        return Ok(notes);
    }

    // PUT /api/verse-notes/{bookId}/{chapter}/{verse}
    // Upsert: cria se não existir, atualiza se já existir.
    [HttpPut("{bookId}/{chapter}/{verse}")]
    public async Task<ActionResult<VerseNoteDto>> Upsert(
        int bookId, int chapter, int verse,
        [FromBody] UpsertVerseNoteDto dto)
    {
        var note = await _context.VerseNotes
            .FirstOrDefaultAsync(n => n.UserId == CurrentUserId
                                   && n.BookId == bookId
                                   && n.Chapter == chapter
                                   && n.Verse == verse);

        if (note == null)
        {
            note = new VerseNote
            {
                UserId  = CurrentUserId,
                BookId  = bookId,
                Chapter = chapter,
                Verse   = verse,
                Note    = dto.Note
            };
            _context.VerseNotes.Add(note);
        }
        else
        {
            note.Note      = dto.Note;
            note.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok(Map(note));
    }

    // DELETE /api/verse-notes/{bookId}/{chapter}/{verse}
    [HttpDelete("{bookId}/{chapter}/{verse}")]
    public async Task<IActionResult> Delete(int bookId, int chapter, int verse)
    {
        var note = await _context.VerseNotes
            .FirstOrDefaultAsync(n => n.UserId == CurrentUserId
                                   && n.BookId == bookId
                                   && n.Chapter == chapter
                                   && n.Verse == verse);

        if (note == null) return NotFound();

        _context.VerseNotes.Remove(note);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static VerseNoteDto Map(VerseNote n) => new()
    {
        Id        = n.Id,
        BookId    = n.BookId,
        BookName  = "",   // não disponível sem join — usado apenas no Upsert
        Chapter   = n.Chapter,
        Verse     = n.Verse,
        Note      = n.Note,
        UpdatedAt = n.UpdatedAt
    };
}
