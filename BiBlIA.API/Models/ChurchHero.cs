namespace BíblIA.Api.Models;

public class ChurchHero
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public int? BirthYear { get; set; }
    public int? DeathYear { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string KeyContributions { get; set; } = string.Empty;
    public string FavoriteVerse { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // "Reformer", "Missionary", "Evangelist", "Revivalist", "Theologian", "Martyr"
    public string ImageUrl { get; set; } = string.Empty;
}
