namespace BíblIA.Api.DTOs;

public class BibleBookDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Testament { get; set; } = string.Empty;
    public int ChapterCount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}
