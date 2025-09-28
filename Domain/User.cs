namespace Domain;

public class User
{
    public int  Id { get; set; }
    public required string Name { get; set; } 
    public int Age { get; set; }   
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
}