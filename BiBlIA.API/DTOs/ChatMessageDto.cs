namespace BíblIA.Api.DTOs;

public class ChatMessageDto
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateChatMessageDto
{
    public int ChatId { get; set; }
    public string Content { get; set; } = string.Empty;
}