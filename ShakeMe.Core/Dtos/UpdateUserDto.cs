namespace ShakeMe.Core.Dtos;

public class UpdateUserDto
{
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public string? Email { get; set; }
    public DateTime? date_of_birth { get; set; }
    public bool? IsAnonymous { get; set; }
    public string? Pseudo { get; set; }
    public string? AvatarUrl { get; set; }
}