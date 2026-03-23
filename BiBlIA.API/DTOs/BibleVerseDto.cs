using System.ComponentModel.DataAnnotations;

namespace BíblIA.Api.DTOs;

public class BibleVerseDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookName { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string TextACF { get; set; } = string.Empty;
    public string TextKJV { get; set; } = string.Empty;
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
