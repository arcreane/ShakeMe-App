namespace ShakeMe.Core.Dtos;

using System.Text.Json.Serialization;

public class AuthenticatedUserDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("user")]
    public UserDto User { get; set; }
}
