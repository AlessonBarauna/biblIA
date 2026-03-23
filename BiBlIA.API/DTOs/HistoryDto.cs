namespace BíblIA.Api.DTOs;

/// <summary>Visão escatológica (interpretação de eventos futuros)</summary>
public class EschatologyViewDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string DetailedExplanation { get; set; } = string.Empty;
    public string MainTheologians { get; set; } = string.Empty;
    public string KeyScriptures { get; set; } = string.Empty;
    public string Strengths { get; set; } = string.Empty;
    public string Weaknesses { get; set; } = string.Empty;
    public string MillenniumView { get; set; } = string.Empty;
    public string RaptureView { get; set; } = string.Empty;
    public string TribulationView { get; set; } = string.Empty;
}

/// <summary>Herói ou figura importante da história da Igreja</summary>
public class ChurchHeroDto
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
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

/// <summary>Movimento de avivamento religioso</summary>
public class RevivalDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public int? EndYear { get; set; }
    public string Location { get; set; } = string.Empty;
    public string LeaderNames { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string KeyEvents { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public string EstimatedConversions { get; set; } = string.Empty;
}
