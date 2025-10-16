using System.Text.Json.Serialization;

namespace Shared.Dto.UserDto;

public class UserDto
{
    [JsonPropertyName("user_id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }
}