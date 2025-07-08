using System.Text.Json.Serialization;

public class RegisterDto
{
    [JsonPropertyName("first_name")]
    public string first_name { get; set; } = null!;

    [JsonPropertyName("last_name")]
    public string last_name { get; set; } = null!;

    [JsonPropertyName("pseudo")]
    public string Pseudo { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("date_of_birth")]
    public DateOnly date_of_birth { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}