namespace BíblIA.Api.Models;

public class BibleVerse
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string TextACF { get; set; } = string.Empty; // Almeida Corrigida Fiel (português)
    public string TextKJV { get; set; } = string.Empty; // King James Version (inglês)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public BibleBook? Book { get; set; }
}