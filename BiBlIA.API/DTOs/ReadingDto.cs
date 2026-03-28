using System.ComponentModel.DataAnnotations;

namespace BíblIA.Api.DTOs;

public class ReadingPlanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int TotalDays { get; set; }
    public string Strategy { get; set; } = "";
    public string Icon { get; set; } = "";
    public int CompletedDays { get; set; }
}

public class ReadingLogDto
{
    public int PlanId { get; set; }
    public int DayNumber { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class MarkDayDto
{
    [Required] public int PlanId { get; set; }
    [Required, Range(1, 365)] public int DayNumber { get; set; }
}
