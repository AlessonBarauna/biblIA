namespace BíblIA.Api.DTOs;

public class ChatDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ChatMessageDto> Messages { get; set; } = new();
}

public class CreateChatDto
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class UpdateChatDto
{
    public string Title { get; set; } = string.Empty;
}