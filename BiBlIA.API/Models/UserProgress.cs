namespace BíblIA.Api.Models;

public class UserProgress
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public int ModuleId { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
