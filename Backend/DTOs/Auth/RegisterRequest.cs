
namespace RegisterDTO
{
    public class RegisterRequest
{
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; } // Student, Admin, Finance
    public required string Email { get; set;}

}

}