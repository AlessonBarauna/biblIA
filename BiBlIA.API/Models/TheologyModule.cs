namespace BíblIA.Api.Models;

public class TheologyModule
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string References { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public TheologyCourse? Course { get; set; }
    public ICollection<TheologyQuiz> Quizzes { get; set; } = new List<TheologyQuiz>();
}
