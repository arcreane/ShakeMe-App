namespace ShakeMe.Core.Dtos;

public class CreateUserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsAnonymous { get; set; }
    public string? Pseudo { get; set; }
    public string? AvatarUrl { get; set; }
}