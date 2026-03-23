namespace BíblIA.Api.Models;

public class Revival
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
