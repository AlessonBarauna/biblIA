namespace BíblIA.Api.DTOs;

/// <summary>Representa um curso de teologia</summary>
public class TheologyCourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public string Level { get; set; } = string.Empty;
    public string ImageIcon { get; set; } = string.Empty;
    public int ModuleCount { get; set; }
}

/// <summary>Módulo dentro de um curso de teologia</summary>
public class TheologyModuleDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string References { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}

/// <summary>Quiz de teologia — avalia aprendizado</summary>
public class TheologyQuizDto
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}
