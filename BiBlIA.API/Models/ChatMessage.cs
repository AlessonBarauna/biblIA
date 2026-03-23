namespace BíblIA.Api.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public string Role { get; set; } = string.Empty; // "user" ou "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public Chat? Chat { get; set; }
}