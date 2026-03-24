namespace BíblIA.Api.Models;

public class BibleVerse
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string TextKJV { get; set; } = string.Empty; // King James Version (en)
    public string TextAA  { get; set; } = string.Empty; // Almeida Revisada (pt-BR)
    public string TextACF { get; set; } = string.Empty; // Almeida Corrigida e Fiel (pt-BR)
    public string TextNVI { get; set; } = string.Empty; // Nova Versão Internacional (pt-BR)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public BibleBook? Book { get; set; }
}