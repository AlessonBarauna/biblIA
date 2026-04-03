using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BíblIA.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /api/profile — Perfil do usuário autenticado com estatísticas de uso.</summary>
    [HttpGet]
    public async Task<ActionResult<ProfileDto>> GetProfile()
    {
        var userId = GetUserId();
        var user   = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        // EF Core DbContext não é thread-safe — queries devem ser sequenciais
        var bookmarkCount = await _context.BookmarkVerses.CountAsync(b => b.UserId == userId);
        var moduleCount   = await _context.UserProgress.CountAsync(p => p.UserId == userId && p.Completed);
        var readDaysCount = await _context.ReadingLogs.CountAsync(l => l.UserId == userId);

        return Ok(new ProfileDto
        {
            Id                   = user.Id,
            Name                 = user.Name,
            Email                = user.Email,
            CreatedAt            = user.CreatedAt,
            BookmarkCount        = bookmarkCount,
            CompletedModuleCount = moduleCount,
            ReadingDaysCount     = readDaysCount
        });
    }

    /// <summary>PUT /api/profile — Atualiza o nome do usuário.</summary>
    [HttpPut]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = GetUserId();
        var user   = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.Name = dto.Name.Trim();
        await _context.SaveChangesAsync();

        // Retorna o novo nome para o frontend atualizar o estado local
        return Ok(new { name = user.Name });
    }

    /// <summary>PUT /api/profile/password — Troca a senha do usuário.</summary>
    [HttpPut("password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return BadRequest(new { message = "A nova senha e a confirmação não conferem." });

        var userId = GetUserId();
        var user   = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Senha atual incorreta." });

        user.PasswordHash = HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Senha alterada com sucesso." });
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash) =>
        HashPassword(password) == hash;
}
