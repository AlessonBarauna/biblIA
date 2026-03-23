namespace BíblIA.Api.Models;

public class BookmarkVerse
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string VerseText { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
