namespace BíblIA.Api.Models;

public class BookmarkVerse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int BookId { get; set; }
    public BibleBook Book { get; set; } = null!;
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string VerseText { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    // Tags armazenadas como string comma-separated: "fé,graça,salvação"
    public string Tags { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
