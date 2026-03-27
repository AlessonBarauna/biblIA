using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using BíblIA.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BíblIA.Api.Controllers;

[ApiController]
[Route("api/progress")]
[Authorize]
public class UserProgressController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserProgressController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Retorna todos os módulos concluídos pelo usuário.
    // O frontend usa essa lista para popular o progressSet/courseProgressMap.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProgressDto>>> GetProgress()
    {
        var userId = GetUserId();

        var progress = await _context.UserProgress
            .Where(p => p.UserId == userId && p.Completed)
            .ToListAsync();

        return Ok(progress.Select(MapToDto));
    }

    // Marca um módulo como concluído. Upsert: se já existir, atualiza score e data.
    [HttpPost]
    public async Task<ActionResult<UserProgressDto>> CompleteModule(CompleteModuleDto dto)
    {
        var userId = GetUserId();

        var existing = await _context.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == dto.ModuleId);

        if (existing != null)
        {
            existing.Completed = true;
            existing.CompletedAt = DateTime.UtcNow;
            existing.Score = dto.Score;
        }
        else
        {
            existing = new UserProgress
            {
                UserId = userId,
                CourseId = dto.CourseId,
                ModuleId = dto.ModuleId,
                Completed = true,
                CompletedAt = DateTime.UtcNow,
                Score = dto.Score,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserProgress.Add(existing);
        }

        await _context.SaveChangesAsync();
        return Ok(MapToDto(existing));
    }

    // Remove a conclusão de um módulo (permite "desmarcar").
    [HttpDelete("modules/{moduleId}")]
    public async Task<IActionResult> UndoModule(int moduleId)
    {
        var userId = GetUserId();

        var progress = await _context.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == moduleId);

        if (progress == null)
            return NotFound();

        _context.UserProgress.Remove(progress);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static UserProgressDto MapToDto(UserProgress p) => new()
    {
        Id = p.Id,
        CourseId = p.CourseId,
        ModuleId = p.ModuleId,
        Completed = p.Completed,
        CompletedAt = p.CompletedAt,
        Score = p.Score
    };
}
