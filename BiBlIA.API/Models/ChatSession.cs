namespace BíblIA.Api.Models;

public class ChatSession
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty; // "theology", "bible", "history", "eschatology"
    public string MessagesJson { get; set; } = "[]";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
