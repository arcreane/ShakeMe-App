namespace ShakeMe.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public bool IsAnonymous { get; set; }
    public string Pseudo { get; set; } = null!;
    public DateTime LastActive { get; set; }
    public string AvatarUrl { get; set; }
}