namespace ShakeMe.Core.Dtos;

using System.Text.Json.Serialization;

public class UserDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("pseudo")]
    public string Pseudo { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("first_name")]
    public string first_name { get; set; }

    [JsonPropertyName("last_name")]
    public string last_name { get; set; }
}
