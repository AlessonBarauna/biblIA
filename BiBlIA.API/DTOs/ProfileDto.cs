using System.ComponentModel.DataAnnotations;

namespace BíblIA.Api.DTOs;

public class ProfileDto
{
    public int      Id                   { get; set; }
    public string   Name                 { get; set; } = "";
    public string   Email                { get; set; } = "";
    public DateTime CreatedAt            { get; set; }
    public int      BookmarkCount        { get; set; }
    public int      CompletedModuleCount { get; set; }
    public int      ReadingDaysCount     { get; set; }
}

public class UpdateProfileDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = "";
}

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = "";

    [Required, MinLength(6)]
    public string NewPassword { get; set; } = "";

    [Required]
    public string ConfirmPassword { get; set; } = "";
}
