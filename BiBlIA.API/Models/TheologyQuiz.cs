namespace BíblIA.Api.Models;

public class TheologyQuiz
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty; // "A", "B", "C", or "D"
    public string Explanation { get; set; } = string.Empty;

    public TheologyModule? Module { get; set; }
}
