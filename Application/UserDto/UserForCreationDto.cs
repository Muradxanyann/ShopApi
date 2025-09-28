namespace Application.UserDto;

public class UserForCreationDto
{
    public required string Name { get; set; } 
    public int Age { get; set; }   
    public required string Phone { get; set; }
    public required string Email { get; set; }
}