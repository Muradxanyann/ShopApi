namespace Application.Dto.AuthDto;

public class UserRegistrationDto
{
    public required string Name { get; init; } 
    public int Age { get; init; }   
    public required string Phone { get; init; }
    public required string Email { get; init; }   
    
    public required string Username { get; init; }     
    public required string Password { get; init; }  
    
}