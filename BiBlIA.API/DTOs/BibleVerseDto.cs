namespace BíblIA.Api.DTOs;

public class BibleVerseDto
{
    public int Id { get; set; }
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

public class CreateBibleVerseDto
{
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Version { get; set; } = "KJV";
}