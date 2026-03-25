using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using BíblIA.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Controllers;

/// <summary>
/// Cursos e módulos de teologia — aprendizado estruturado sobre doutrina e interpretação bíblica.
/// Endpoints: cursos, módulos, buscas, quizzes.
/// </summary>
[ApiController]
[Route("api/theology")]
public class TheologyController : ControllerBase
{
    private readonly AppDbContext _context;

    public TheologyController(AppDbContext context)
    {
        _context = context;
    }

    // ── CURSOS ────────────────────────────────────────────────────────────────

    /// <summary>GET /api/theology/courses — Lista todos os cursos</summary>
    [HttpGet("courses")]
    public async Task<ActionResult<IEnumerable<TheologyCourseDto>>> GetCourses()
    {
        var courses = await _context.TheologyCourses
            .Include(c => c.Modules)
            .OrderBy(c => c.Id)
            .ToListAsync();

        return Ok(courses.Select(c => new TheologyCourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Category = c.Category,
            DurationHours = c.DurationHours,
            Level = c.Level,
            ImageIcon = c.ImageIcon,
            ModuleCount = c.Modules.Count,
            ExternalUrl = c.ExternalUrl,
            Provider    = c.Provider
        }));
    }

    /// <summary>GET /api/theology/courses/{courseId} — Detalhes de um curso</summary>
    [HttpGet("courses/{courseId}")]
    public async Task<ActionResult<TheologyCourseDto>> GetCourse(int courseId)
    {
        var course = await _context.TheologyCourses
            .Include(c => c.Modules)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
            return NotFound();

        return Ok(new TheologyCourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Category = course.Category,
            DurationHours = course.DurationHours,
            Level = course.Level,
            ImageIcon = course.ImageIcon,
            ModuleCount = course.Modules.Count
        });
    }

    // ── MÓDULOS ───────────────────────────────────────────────────────────────

    /// <summary>GET /api/theology/courses/{courseId}/modules — Módulos de um curso</summary>
    [HttpGet("courses/{courseId}/modules")]
    public async Task<ActionResult<IEnumerable<TheologyModuleDto>>> GetModules(int courseId)
    {
        var courseExists = await _context.TheologyCourses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return NotFound("Curso não encontrado.");

        var modules = await _context.TheologyModules
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.OrderIndex)
            .ToListAsync();

        return Ok(modules.Select(m => new TheologyModuleDto
        {
            Id = m.Id,
            CourseId = m.CourseId,
            Title = m.Title,
            Content = m.Content,
            References = m.References,
            OrderIndex = m.OrderIndex
        }));
    }

    /// <summary>GET /api/theology/modules/{moduleId} — Detalhes de um módulo</summary>
    [HttpGet("modules/{moduleId}")]
    public async Task<ActionResult<TheologyModuleDto>> GetModule(int moduleId)
    {
        var module = await _context.TheologyModules.FirstOrDefaultAsync(m => m.Id == moduleId);

        if (module == null)
            return NotFound();

        return Ok(new TheologyModuleDto
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Content = module.Content,
            References = module.References,
            OrderIndex = module.OrderIndex
        });
    }

    // ── QUIZZES ───────────────────────────────────────────────────────────────

    /// <summary>GET /api/theology/modules/{moduleId}/quizzes — Quizzes de um módulo</summary>
    [HttpGet("modules/{moduleId}/quizzes")]
    public async Task<ActionResult<IEnumerable<TheologyQuizDto>>> GetQuizzes(int moduleId)
    {
        var moduleExists = await _context.TheologyModules.AnyAsync(m => m.Id == moduleId);
        if (!moduleExists)
            return NotFound("Módulo não encontrado.");

        var quizzes = await _context.TheologyQuizzes
            .Where(q => q.ModuleId == moduleId)
            .ToListAsync();

        return Ok(quizzes.Select(q => new TheologyQuizDto
        {
            Id = q.Id,
            ModuleId = q.ModuleId,
            Question = q.Question,
            OptionA = q.OptionA,
            OptionB = q.OptionB,
            OptionC = q.OptionC,
            OptionD = q.OptionD,
            CorrectAnswer = q.CorrectAnswer,
            Explanation = q.Explanation
        }));
    }

    /// <summary>GET /api/theology/search?query=... — Busca cursos e módulos</summary>
    [HttpGet("search")]
    public async Task<ActionResult<dynamic>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return BadRequest("Query deve ter pelo menos 2 caracteres.");

        var searchTerm = query.ToLowerInvariant();

        var courses = await _context.TheologyCourses
            .Where(c => c.Title.ToLower().Contains(searchTerm) || c.Description.ToLower().Contains(searchTerm))
            .Select(c => new { Type = "Course", c.Id, c.Title, c.Description })
            .ToListAsync();

        var modules = await _context.TheologyModules
            .Where(m => m.Title.ToLower().Contains(searchTerm) || m.Content.ToLower().Contains(searchTerm))
            .Select(m => new { Type = "Module", m.Id, m.Title, m.Content })
            .ToListAsync();

        return Ok(new { courses, modules });
    }
}
