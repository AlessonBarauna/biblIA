using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Controllers;

/// <summary>
/// Escatologia — interpretações de eventos futuros segundo diferentes tradições teológicas.
/// Endpoints: visões escatológicas, busca, comparação.
/// </summary>
[ApiController]
[Route("api/eschatology")]
public class EschatologyController : ControllerBase
{
    private readonly AppDbContext _context;

    public EschatologyController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /api/eschatology/views — Lista todas as visões escatológicas</summary>
    [HttpGet("views")]
    public async Task<ActionResult<IEnumerable<EschatologyViewDto>>> GetViews()
    {
        var views = await _context.EschatologyViews
            .OrderBy(v => v.Id)
            .ToListAsync();

        return Ok(views.Select(v => MapToDto(v)));
    }

    /// <summary>GET /api/eschatology/views/{viewId} — Detalhes de uma visão escatológica</summary>
    [HttpGet("views/{viewId}")]
    public async Task<ActionResult<EschatologyViewDto>> GetView(int viewId)
    {
        var view = await _context.EschatologyViews.FindAsync(viewId);

        if (view == null)
            return NotFound();

        return Ok(MapToDto(view));
    }

    /// <summary>GET /api/eschatology/compare?view1=1&view2=2 — Compara duas visões escatológicas</summary>
    [HttpGet("compare")]
    public async Task<ActionResult<dynamic>> CompareViews([FromQuery] int view1, [FromQuery] int view2)
    {
        if (view1 <= 0 || view2 <= 0)
            return BadRequest("view1 e view2 devem ser > 0.");

        if (view1 == view2)
            return BadRequest("Selecione duas visões diferentes.");

        var v1 = await _context.EschatologyViews.FindAsync(view1);
        var v2 = await _context.EschatologyViews.FindAsync(view2);

        if (v1 == null || v2 == null)
            return NotFound();

        return Ok(new
        {
            View1 = MapToDto(v1),
            View2 = MapToDto(v2),
            Comparison = new
            {
                MillenniumDifference = $"{v1.MillenniumView} vs {v2.MillenniumView}",
                RaptureDifference = $"{v1.RaptureView} vs {v2.RaptureView}",
                TribulationDifference = $"{v1.TribulationView} vs {v2.TribulationView}"
            }
        });
    }

    /// <summary>GET /api/eschatology/search?query=... — Busca visões escatológicas por nome</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<EschatologyViewDto>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return BadRequest("Query deve ter pelo menos 2 caracteres.");

        var searchTerm = query.ToLowerInvariant();

        var views = await _context.EschatologyViews
            .Where(v => v.Name.ToLower().Contains(searchTerm) 
                     || v.Summary.ToLower().Contains(searchTerm)
                     || v.MainTheologians.ToLower().Contains(searchTerm))
            .ToListAsync();

        return Ok(views.Select(v => MapToDto(v)));
    }

    /// <summary>GET /api/eschatology/by-theologian?theologian=... — Visões por teólogo</summary>
    [HttpGet("by-theologian")]
    public async Task<ActionResult<IEnumerable<EschatologyViewDto>>> GetByTheologian([FromQuery] string theologian)
    {
        if (string.IsNullOrWhiteSpace(theologian))
            return BadRequest("Theologian é obrigatório.");

        var searchTerm = theologian.ToLowerInvariant();

        var views = await _context.EschatologyViews
            .Where(v => v.MainTheologians.ToLower().Contains(searchTerm))
            .ToListAsync();

        return Ok(views.Select(v => MapToDto(v)));
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static EschatologyViewDto MapToDto(BíblIA.Api.Models.EschatologyView view) =>
        new()
        {
            Id = view.Id,
            Name = view.Name,
            Summary = view.Summary,
            DetailedExplanation = view.DetailedExplanation,
            MainTheologians = view.MainTheologians,
            KeyScriptures = view.KeyScriptures,
            Strengths = view.Strengths,
            Weaknesses = view.Weaknesses,
            MillenniumView = view.MillenniumView,
            RaptureView = view.RaptureView,
            TribulationView = view.TribulationView
        };
}
