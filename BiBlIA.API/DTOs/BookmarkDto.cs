using System.ComponentModel.DataAnnotations;

namespace BíblIA.Api.DTOs;

public class BookmarkDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public string BookName { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int Verse { get; set; }
    public string VerseText { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateBookmarkDto
{
    [Required]
    public int BookId { get; set; }

    [Required, Range(1, 150)]
    public int Chapter { get; set; }

    [Required, Range(1, 176)]
    public int Verse { get; set; }

    [MaxLength(2000)]
    public string VerseText { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Note { get; set; } = string.Empty;
}
