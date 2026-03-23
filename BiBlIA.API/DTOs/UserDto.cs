namespace BíblIA.Api.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}