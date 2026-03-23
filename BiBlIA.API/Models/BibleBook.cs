namespace BíblIA.Api.Models;

public class BibleBook
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Testament { get; set; } = string.Empty; // "OT" or "NT"
    public int ChapterCount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public ICollection<BibleVerse> Verses { get; set; } = new List<BibleVerse>();
}
