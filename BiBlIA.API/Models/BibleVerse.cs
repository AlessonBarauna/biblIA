namespace BíblIA.Api.Models;

public class BibleVerse
{
    public int Id { get; set; }
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Version { get; set; } = "KJV"; // King James Version
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}