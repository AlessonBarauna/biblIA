namespace BíblIA.Api.Models;

public class TheologyCourse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public string Level { get; set; } = string.Empty; // "Básico", "Intermediário", "Avançado"
    public string ImageIcon { get; set; } = string.Empty;

    public string? ExternalUrl { get; set; }   // link para curso gratuito real na internet
    public string? Provider { get; set; }       // ex: "BiblicalTraining.org", "Ligonier Ministries"

    public ICollection<TheologyModule> Modules { get; set; } = new List<TheologyModule>();
}
