namespace BíblIA.Api.Models;

public class ReadingPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int TotalDays { get; set; }

    /// <summary>Determina quais livros entram no plano: "full_bible" | "new_testament" | "gospels"</summary>
    public string Strategy { get; set; } = "";
    public string Icon { get; set; } = "";

    public ICollection<ReadingLog> Logs { get; set; } = [];
}
