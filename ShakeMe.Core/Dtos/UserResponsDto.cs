namespace ShakeMe.Core.Dtos;

using System.Text.Json.Serialization;

public class UserResponseDto
{
    [JsonPropertyName("user")]
    public UserDto User { get; set; }
}