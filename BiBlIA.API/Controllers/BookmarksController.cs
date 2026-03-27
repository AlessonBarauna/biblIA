using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using BíblIA.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BíblIA.Api.Controllers;

// [Authorize] garante que apenas usuários autenticados acessem este controller.
// O userId é extraído do JWT — nunca confiamos em userId vindo do cliente.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookmarksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookmarksController(AppDbContext context)
    {
        _context = context;
    }

    // Extrai o userId do claim NameIdentifier do token JWT.
    // ClaimTypes.NameIdentifier corresponde ao claim gerado em JwtService.GenerateToken().
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookmarkDto>>> GetBookmarks()
    {
        var userId = GetUserId();

        var bookmarks = await _context.BookmarkVerses
            .Include(b => b.Book)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(bookmarks.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<BookmarkDto>> AddBookmark(CreateBookmarkDto dto)
    {
        var userId = GetUserId();

        // Evita duplicatas silenciosas — conflito é mais honesto que salvar dois registros
        var exists = await _context.BookmarkVerses.AnyAsync(b =>
            b.UserId == userId &&
            b.BookId == dto.BookId &&
            b.Chapter == dto.Chapter &&
            b.Verse == dto.Verse);

        if (exists)
            return Conflict("Versículo já está nos favoritos.");

        var bookmark = new BookmarkVerse
        {
            UserId = userId,
            BookId = dto.BookId,
            Chapter = dto.Chapter,
            Verse = dto.Verse,
            VerseText = dto.VerseText,
            Note = dto.Note,
            CreatedAt = DateTime.UtcNow
        };

        _context.BookmarkVerses.Add(bookmark);
        await _context.SaveChangesAsync();

        // Carrega a navigation property Book para montar o DTO de retorno
        await _context.Entry(bookmark).Reference(b => b.Book).LoadAsync();

        return CreatedAtAction(nameof(GetBookmarks), MapToDto(bookmark));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveBookmark(int id)
    {
        var userId = GetUserId();

        // Filtra por UserId junto com Id — impede que um usuário delete favoritos de outro
        var bookmark = await _context.BookmarkVerses
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (bookmark == null)
            return NotFound();

        _context.BookmarkVerses.Remove(bookmark);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static BookmarkDto MapToDto(BookmarkVerse b) => new()
    {
        Id = b.Id,
        UserId = b.UserId,
        BookId = b.BookId,
        BookName = b.Book?.Name ?? string.Empty,
        Chapter = b.Chapter,
        Verse = b.Verse,
        VerseText = b.VerseText,
        Note = b.Note,
        CreatedAt = b.CreatedAt
    };
}
