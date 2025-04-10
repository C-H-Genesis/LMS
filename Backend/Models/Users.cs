using RoleModel;
using EnrollmentModel;

namespace UsersModel{
    public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public required string FullName { get; set; }
    public required string Username { get; set; } 
    public required string PasswordHash { get; set; }
    public Guid RoleId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserType { get; set; }
    

    // Navigation Properties
    
    public Role? Role { get; set; }
    public ICollection<Enrollment>? Enrollments { get; set; }  = new List<Enrollment>();
    
}

}


