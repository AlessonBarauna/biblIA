namespace BíblIA.Api.DTOs;

public class VerseNoteDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string Note { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
}

public class UpsertVerseNoteDto
{
    public required string Note { get; set; }
}
