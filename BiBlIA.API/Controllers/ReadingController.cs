using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using BíblIA.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BíblIA.Api.Controllers;

[ApiController]
[Route("api/reading")]
public class ReadingController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReadingController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /api/reading/plans — Lista planos; se autenticado inclui progresso do usuário.</summary>
    [HttpGet("plans")]
    public async Task<ActionResult<IEnumerable<ReadingPlanDto>>> GetPlans()
    {
        var plans = await _context.ReadingPlans.OrderBy(p => p.Id).ToListAsync();

        var userId = GetUserId();
        var completedByPlan = new Dictionary<int, int>();

        if (userId.HasValue)
        {
            completedByPlan = await _context.ReadingLogs
                .Where(l => l.UserId == userId.Value)
                .GroupBy(l => l.PlanId)
                .Select(g => new { PlanId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PlanId, x => x.Count);
        }

        return Ok(plans.Select(p => new ReadingPlanDto
        {
            Id            = p.Id,
            Name          = p.Name,
            Description   = p.Description,
            TotalDays     = p.TotalDays,
            Strategy      = p.Strategy,
            Icon          = p.Icon,
            CompletedDays = completedByPlan.GetValueOrDefault(p.Id, 0)
        }));
    }

    /// <summary>GET /api/reading/logs — Dias concluídos do usuário em todos os planos.</summary>
    [HttpGet("logs")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ReadingLogDto>>> GetLogs()
    {
        var userId = GetUserIdRequired();
        var logs = await _context.ReadingLogs
            .Where(l => l.UserId == userId)
            .Select(l => new ReadingLogDto { PlanId = l.PlanId, DayNumber = l.DayNumber, CompletedAt = l.CompletedAt })
            .ToListAsync();

        return Ok(logs);
    }

    /// <summary>POST /api/reading/logs — Marca um dia como concluído (idempotente).</summary>
    [HttpPost("logs")]
    [Authorize]
    public async Task<ActionResult> MarkDay([FromBody] MarkDayDto dto)
    {
        var userId = GetUserIdRequired();

        var exists = await _context.ReadingLogs
            .AnyAsync(l => l.UserId == userId && l.PlanId == dto.PlanId && l.DayNumber == dto.DayNumber);

        if (!exists)
        {
            _context.ReadingLogs.Add(new ReadingLog
            {
                UserId      = userId,
                PlanId      = dto.PlanId,
                DayNumber   = dto.DayNumber,
                CompletedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    /// <summary>DELETE /api/reading/logs/{planId}/{dayNumber} — Desmarca um dia.</summary>
    [HttpDelete("logs/{planId}/{dayNumber}")]
    [Authorize]
    public async Task<ActionResult> UnmarkDay(int planId, int dayNumber)
    {
        var userId = GetUserIdRequired();
        var log = await _context.ReadingLogs
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PlanId == planId && l.DayNumber == dayNumber);

        if (log == null) return NotFound();

        _context.ReadingLogs.Remove(log);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private int? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim) : null;
    }

    private int GetUserIdRequired()
    {
        return GetUserId() ?? throw new UnauthorizedAccessException();
    }
}
