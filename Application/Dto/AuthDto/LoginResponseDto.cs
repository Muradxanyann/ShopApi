namespace Application.Dto.AuthDto;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}