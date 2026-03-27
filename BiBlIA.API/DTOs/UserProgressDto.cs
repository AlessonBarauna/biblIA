using System.ComponentModel.DataAnnotations;

namespace BíblIA.Api.DTOs;

public class UserProgressDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int ModuleId { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Score { get; set; }
}

public class CompleteModuleDto
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    public int ModuleId { get; set; }

    [Range(0, 100)]
    public int Score { get; set; } = 0;
}
