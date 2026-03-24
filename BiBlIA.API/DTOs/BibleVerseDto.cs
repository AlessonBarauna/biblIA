using System.ComponentModel.DataAnnotations;

namespace BíblIA.Api.DTOs;

public class BibleVerseDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookName { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string TextKJV { get; set; } = string.Empty;
    public string TextAA  { get; set; } = string.Empty;
    public string TextACF { get; set; } = string.Empty;
    public string TextNVI { get; set; } = string.Empty;
}

/// <summary>
/// DTO para importação em lote via POST /api/bible/import.
/// Usa bookOrderIndex (1=Gênesis … 66=Apocalipse) em vez de bookId interno,
/// para que scripts externos não precisem conhecer os IDs do banco.
/// </summary>
public class ImportVerseDto
{
    public int BookOrderIndex { get; set; }
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string TextKJV { get; set; } = string.Empty;
    public string TextAA  { get; set; } = string.Empty;
    public string TextACF { get; set; } = string.Empty;
    public string TextNVI { get; set; } = string.Empty;
}

public class ImportResultDto
{
    public int Total { get; set; }
    public int Imported { get; set; }
    public int Skipped { get; set; }
}

public class CreateBibleVerseDto
{
    [Required]
    public int BookId { get; set; }

    [Required, Range(1, 150)]
    public int Chapter { get; set; }

    [Required, Range(1, 176)]
    public int Verse { get; set; }

    [MaxLength(2000)]
    public string TextACF { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string TextKJV { get; set; } = string.Empty;
}
