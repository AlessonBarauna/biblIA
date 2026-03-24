using BíblIA.Api.Data;
using BíblIA.Api.DTOs;
using BíblIA.Api.Models;
using BíblIA.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly GeminiService _geminiService;

    public ChatsController(AppDbContext context, GeminiService geminiService)
    {
        _context = context;
        _geminiService = geminiService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<ChatDto>>> GetUserChats(int userId)
    {
        var chats = await _context.Chats
            .Where(c => c.UserId == userId)
            .Include(c => c.Messages)
            .ToListAsync();

        return Ok(chats.Select(c => MapToDto(c)));
    }

    [HttpGet("detail/{id}")]
    public async Task<ActionResult<ChatDto>> GetChat(int id)
    {
        var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (chat == null)
            return NotFound();

        return Ok(MapToDto(chat));
    }

    [HttpPost]
    public async Task<ActionResult<ChatDto>> CreateChat(CreateChatDto dto)
    {
        var chat = new Chat
        {
            UserId = dto.UserId,
            Title = dto.Title,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChat), new { id = chat.Id }, MapToDto(chat));
    }

    [HttpPost("{chatId}/messages")]
    public async Task<ActionResult<ChatMessageDto>> SendMessage(int chatId, CreateChatMessageDto dto)
    {
        var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            return NotFound("Chat não encontrado");

        // Salvar mensagem do usuário
        var userMessage = new ChatMessage
        {
            ChatId = chatId,
            Role = "user",
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(userMessage);
        await _context.SaveChangesAsync();

        // Preparar histórico de conversa
        var history = chat.Messages
            .Select(m => (m.Role, m.Content))
            .ToList();

        // Obter resposta da IA
        var aiResponse = await _geminiService.SendMessageAsync(dto.Content, history);

        // Salvar resposta da IA
        var aiMessage = new ChatMessage
        {
            ChatId = chatId,
            Role = "assistant",
            Content = aiResponse,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(aiMessage);
        chat.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChat), new { id = chatId }, MapMessageToDto(userMessage));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateChat(int id, UpdateChatDto dto)
    {
        var chat = await _context.Chats.FindAsync(id);
        if (chat == null)
            return NotFound();

        chat.Title = dto.Title;
        chat.UpdatedAt = DateTime.UtcNow;

        _context.Chats.Update(chat);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChat(int id)
    {
        var chat = await _context.Chats.FindAsync(id);
        if (chat == null)
            return NotFound();

        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static ChatDto MapToDto(Chat chat)
    {
        return new ChatDto
        {
            Id = chat.Id,
            UserId = chat.UserId,
            Title = chat.Title,
            CreatedAt = chat.CreatedAt,
            UpdatedAt = chat.UpdatedAt,
            Messages = chat.Messages.Select(m => MapMessageToDto(m)).ToList()
        };
    }

    private static ChatMessageDto MapMessageToDto(ChatMessage message)
    {
        return new ChatMessageDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            Role = message.Role,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };
    }
}