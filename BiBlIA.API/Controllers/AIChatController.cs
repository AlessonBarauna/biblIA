using BíblIA.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BíblIA.Api.Controllers;

/// <summary>
/// Endpoint stateless de IA — usado pelos painéis auxiliares de cada aba.
/// Não persiste histórico no banco; cada requisição é independente.
///
/// Domínios válidos: general | bible | theology | history | eschatology
/// </summary>
[ApiController]
[Route("api/ai")]
public class AIChatController : ControllerBase
{
    private readonly GroqService _groq;

    public AIChatController(GroqService gemini)
    {
        _groq = gemini;
    }

    // POST /api/ai/ask
    [HttpPost("ask")]
    public async Task<ActionResult<AiAnswerDto>> Ask([FromBody] AiQuestionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Question) || dto.Question.Length < 3)
            return BadRequest("A pergunta deve ter pelo menos 3 caracteres.");

        var domain = dto.Domain ?? "general";
        var answer = await _groq.AskAsync(dto.Question, domain);

        return Ok(new AiAnswerDto { Answer = answer });
    }
}

public record AiQuestionDto(string Question, string? Domain, string? Context);
public record AiAnswerDto { public string Answer { get; init; } = string.Empty; }
