namespace ShakeMe.Core.Models;

public class UserModel
{
    public int Id { get; set; }
    public string first_name { get; set; } = null!;
    public string last_name { get; set; } = null!;
    public string Pseudo { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime date_of_birth { get; set; }
    public DateTime LastActive { get; set; }
}