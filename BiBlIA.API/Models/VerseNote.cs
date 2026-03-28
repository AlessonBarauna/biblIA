namespace BíblIA.Api.Models;

/// <summary>Anotação pessoal de um usuário em um versículo específico.</summary>
public class VerseNote
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int BookId { get; set; }
    public int Chapter { get; set; }
    public int Verse { get; set; }

    public string Note { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
