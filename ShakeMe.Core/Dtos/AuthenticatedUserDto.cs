namespace ShakeMe.Core.Dtos;

public class AuthenticatedUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Pseudo { get; set; } = null!;
    public string? Token { get; set; } // Pour plus tard si on veut faire du JWT
}