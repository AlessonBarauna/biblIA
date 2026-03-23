using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Controllers;

/// <summary>
/// História da Igreja — heróis cristãos, avivamentos, marcos históricos.
/// Endpoints: heróis, avivamentos, timeline, filtros.
/// </summary>
[ApiController]
[Route("api/history")]
public class HistoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public HistoryController(AppDbContext context)
    {
        _context = context;
    }

    // ── HERÓIS DA IGREJA ──────────────────────────────────────────────────────

    /// <summary>GET /api/history/heroes — Lista heróis da Igreja</summary>
    [HttpGet("heroes")]
    public async Task<ActionResult<IEnumerable<ChurchHeroDto>>> GetHeroes([FromQuery] string? category)
    {
        var query = _context.ChurchHeroes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(h => h.Category == category);

        var heroes = await query
            .OrderByDescending(h => h.BirthYear)
            .ToListAsync();

        return Ok(heroes.Select(h => MapToDto(h)));
    }

    /// <summary>GET /api/history/heroes/{heroId} — Detalhes de um herói</summary>
    [HttpGet("heroes/{heroId}")]
    public async Task<ActionResult<ChurchHeroDto>> GetHero(int heroId)
    {
        var hero = await _context.ChurchHeroes.FindAsync(heroId);

        if (hero == null)
            return NotFound();

        return Ok(MapToDto(hero));
    }

    /// <summary>GET /api/history/heroes/category/{category} — Heróis por categoria</summary>
    [HttpGet("heroes/category/{category}")]
    public async Task<ActionResult<IEnumerable<ChurchHeroDto>>> GetHeroesByCategory(string category)
    {
        var heroes = await _context.ChurchHeroes
            .Where(h => h.Category == category)
            .OrderByDescending(h => h.BirthYear)
            .ToListAsync();

        if (!heroes.Any())
            return NotFound($"Nenhum herói encontrado na categoria '{category}'.");

        return Ok(heroes.Select(h => MapToDto(h)));
    }

    /// <summary>GET /api/history/heroes/search?query=... — Busca heróis por nome</summary>
    [HttpGet("heroes/search")]
    public async Task<ActionResult<IEnumerable<ChurchHeroDto>>> SearchHeroes([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return BadRequest("Query deve ter pelo menos 2 caracteres.");

        var searchTerm = query.ToLowerInvariant();

        var heroes = await _context.ChurchHeroes
            .Where(h => h.Name.ToLower().Contains(searchTerm) 
                     || h.Biography.ToLower().Contains(searchTerm)
                     || h.KeyContributions.ToLower().Contains(searchTerm))
            .ToListAsync();

        return Ok(heroes.Select(h => MapToDto(h)));
    }

    /// <summary>GET /api/history/heroes/categories — Lista categorias disponíveis</summary>
    [HttpGet("heroes/categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var categories = await _context.ChurchHeroes
            .Select(h => h.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return Ok(categories);
    }

    // ── AVIVAMENTOS ───────────────────────────────────────────────────────────

    /// <summary>GET /api/history/revivals — Lista avivamentos religiosos</summary>
    [HttpGet("revivals")]
    public async Task<ActionResult<IEnumerable<RevivalDto>>> GetRevivals()
    {
        var revivals = await _context.Revivals
            .OrderByDescending(r => r.Year)
            .ToListAsync();

        return Ok(revivals.Select(r => MapToDto(r)));
    }

    /// <summary>GET /api/history/revivals/{revivalId} — Detalhes de um avivamento</summary>
    [HttpGet("revivals/{revivalId}")]
    public async Task<ActionResult<RevivalDto>> GetRevival(int revivalId)
    {
        var revival = await _context.Revivals.FindAsync(revivalId);

        if (revival == null)
            return NotFound();

        return Ok(MapToDto(revival));
    }

    /// <summary>GET /api/history/revivals/location/{location} — Avivamentos por local</summary>
    [HttpGet("revivals/location/{location}")]
    public async Task<ActionResult<IEnumerable<RevivalDto>>> GetRevivalsByLocation(string location)
    {
        var revivals = await _context.Revivals
            .Where(r => r.Location.ToLower().Contains(location.ToLower()))
            .OrderByDescending(r => r.Year)
            .ToListAsync();

        return Ok(revivals.Select(r => MapToDto(r)));
    }

    /// <summary>GET /api/history/revivals/search?query=... — Busca avivamentos</summary>
    [HttpGet("revivals/search")]
    public async Task<ActionResult<IEnumerable<RevivalDto>>> SearchRevivals([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return BadRequest("Query deve ter pelo menos 2 caracteres.");

        var searchTerm = query.ToLowerInvariant();

        var revivals = await _context.Revivals
            .Where(r => r.Name.ToLower().Contains(searchTerm) 
                     || r.Location.ToLower().Contains(searchTerm)
                     || r.LeaderNames.ToLower().Contains(searchTerm)
                     || r.Description.ToLower().Contains(searchTerm))
            .ToListAsync();

        return Ok(revivals.Select(r => MapToDto(r)));
    }

    /// <summary>GET /api/history/timeline — Timeline de heróis e avivamentos</summary>
    [HttpGet("timeline")]
    public async Task<ActionResult<dynamic>> GetTimeline()
    {
        var heroes = await _context.ChurchHeroes
            .Where(h => h.BirthYear.HasValue)
            .OrderByDescending(h => h.BirthYear)
            .ToListAsync();

        var revivals = await _context.Revivals
            .OrderByDescending(r => r.Year)
            .ToListAsync();

        var timeline = new
        {
            Heroes = heroes.Select(h => new
            {
                Type = "Hero",
                h.Name,
                Year = h.BirthYear,
                h.Category,
                h.Period
            }),
            Revivals = revivals.Select(r => new
            {
                Type = "Revival",
                r.Name,
                Year = r.Year,
                r.Location,
                r.LeaderNames
            })
        };

        return Ok(timeline);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static ChurchHeroDto MapToDto(BíblIA.Api.Models.ChurchHero hero) =>
        new()
        {
            Id = hero.Id,
            Name = hero.Name,
            Period = hero.Period,
            BirthYear = hero.BirthYear,
            DeathYear = hero.DeathYear,
            Nationality = hero.Nationality,
            Biography = hero.Biography,
            KeyContributions = hero.KeyContributions,
            FavoriteVerse = hero.FavoriteVerse,
            Category = hero.Category,
            ImageUrl = hero.ImageUrl
        };

    private static RevivalDto MapToDto(BíblIA.Api.Models.Revival revival) =>
        new()
        {
            Id = revival.Id,
            Name = revival.Name,
            Year = revival.Year,
            EndYear = revival.EndYear,
            Location = revival.Location,
            LeaderNames = revival.LeaderNames,
            Description = revival.Description,
            KeyEvents = revival.KeyEvents,
            Impact = revival.Impact,
            EstimatedConversions = revival.EstimatedConversions
        };
}
