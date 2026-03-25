namespace BíblIA.Api.Models;

/// <summary>
/// Nota de estudo bíblico no nível de capítulo — estilo Bíblia de Estudo.
/// Uma nota por (BookId, Chapter). Baseada em comentaristas clássicos de domínio público.
/// </summary>
public class BibleStudyNote
{
    public int Id { get; set; }

    // Qual capítulo esta nota cobre
    public int BookId { get; set; }
    public int Chapter { get; set; }

    public string Title { get; set; } = string.Empty;                 // ex: "A Criação — Fundamentos"
    public string Context { get; set; } = string.Empty;               // contexto histórico e literário
    public string TheologicalSignificance { get; set; } = string.Empty; // peso doutrinal
    public string KeyThemes { get; set; } = string.Empty;             // temas centrais (texto livre)
    public string CrossReferences { get; set; } = string.Empty;       // ex: "João 1:1-3; Hb 11:3"
    public string Commentary { get; set; } = string.Empty;            // notas adicionais
    public string AuthorNote { get; set; } = string.Empty;            // ex: "Baseado em Matthew Henry e Calvino"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public BibleBook? Book { get; set; }
}
