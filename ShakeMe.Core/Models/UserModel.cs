namespace ShakeMe.Core.Models;

public class UserModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Pseudo { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public DateTime LastActive { get; set; }
}